using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDestination : MonoBehaviour
{
    public Vector3 destinationPos;
    private List<Vector3> pathPoints = new List<Vector3>();
    private Quaternion desiredRotation;
    private Path path;
    public bool faceDestinationComplete;
    public float radius = 15.0f;
    private Dictionary <string, float> behaviourState = new Dictionary<string, float>();

    public GameObject debugSpheres;
    // Start is called before the first frame update
    void OnEnable()
    {
        pathPoints.Clear();
        behaviourState.Clear();
        faceDestinationComplete = false;

        // Rotation = clockwise
        // Minimum angle travelled = 90 degrees         | This is the angle before the final point can be selected
        Vector3 originPos = transform.position;

        int numberOfCirclePoints = 8;
        int minimumDegrees = 90;
        float degreeBetweenPoint = 360 / numberOfCirclePoints;
        
        Vector3 relativePos = destinationPos - transform.position;
        desiredRotation = Quaternion.LookRotation(relativePos, Vector3.up);
        // Opposite rotation is needed to get the second last point (the last point is the origin)
        Quaternion oppositeRotation = desiredRotation * Quaternion.Euler(0,180f,0);
        
        
        // First point is directly in front of the ship
        GameObject sphere = Instantiate(debugSpheres);
        sphere.transform.position = transform.position + transform.forward * radius / 2;
        sphere.name = "first";
        Vector3 firstPoint = sphere.transform.position;

        // Second last to the last is the direction of the destination
        sphere = Instantiate(debugSpheres);
        sphere.transform.rotation = oppositeRotation;
        sphere.transform.position = transform.position + sphere.transform.forward * radius;
        sphere.name = "secondLast";
        Vector3 secondLastPoint = sphere.transform.position;

        pathPoints.Add(firstPoint);

        // The other points on the circle are added
        for (int i = 1; i < numberOfCirclePoints * 2 - 1; i++) {
            // Create point
            sphere = Instantiate(debugSpheres);
            sphere.transform.rotation = transform.rotation;
            sphere.transform.position = originPos;
            sphere.name = i.ToString();

            // Rotate point
            float rotDegree =  degreeBetweenPoint * i;
            sphere.transform.RotateAround(originPos, Vector3.up, rotDegree);
            Vector3 newPos = sphere.transform.position;
            newPos.y = transform.position.y;
            sphere.transform.position = newPos + sphere.transform.forward * radius;

            pathPoints.Add(sphere.transform.position);

            // Check if point is close to the second last point, break if so
            if (rotDegree >= minimumDegrees) {
                // Get distance between two points to know if this point is close enough to the second last
                float distBetweenPoints = Vector3.Distance(pathPoints[1], pathPoints[2]);
                float distToSecondLast = Vector3.Distance(sphere.transform.position, secondLastPoint);
                if (distToSecondLast <= distBetweenPoints) {
                    break;
                }
            }

        }

        Vector3 lastPoint = originPos;

        pathPoints.Add(secondLastPoint);
        pathPoints.Add(lastPoint);

        // Create path for the boid
        GameObject pathObj = new GameObject();
        path = pathObj.AddComponent<Path>();
        print(pathPoints.Count);
        path.waypoints = pathPoints;
        path.looped = false;
        path.preDefinedWaypoints = true;
        gameObject.GetComponent<FollowPath>().path = path;
        gameObject.GetComponent<FollowPath>().enabled = true;
        behaviourState.Add("Boid.maxSpeed",  gameObject.GetComponent<Boid>().maxSpeed);
        gameObject.GetComponent<Boid>().maxSpeed = 10;
    }

    void onDisable() {
        // Restore the max speed of the boid
        gameObject.GetComponent<Boid>().maxSpeed = behaviourState["Boid.maxSpeed"];
        gameObject.GetComponent<FollowPath>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (path.next == path.waypoints.Count - 1) {
            gameObject.GetComponent<Boid>().maxSpeed = 5;
            
            float dist = Vector3.Distance(transform.position, pathPoints[pathPoints.Count - 1]);
            // Stop the ship if it is at the destination
            if (dist < 1.0f) {
                gameObject.GetComponent<Boid>().maxSpeed = behaviourState["Boid.maxSpeed"];
                gameObject.GetComponent<Boid>().velocity = new Vector3(0, 0, 0);
                gameObject.GetComponent<FollowPath>().enabled = false;
                faceDestinationComplete = true;

            // Speed up the ship after it made the bend
            } else if (dist < radius/1.5f) {
                gameObject.GetComponent<Boid>().maxSpeed = 10;
            }

        } 
    }
}
