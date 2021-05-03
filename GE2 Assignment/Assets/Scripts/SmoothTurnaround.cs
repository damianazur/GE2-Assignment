using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurnaround : SteeringBehaviour
{
    public GameObject target;
    public int turnSide;
    public float turnSpeed = 0.7f;
    public Quaternion desiredRotation;
    public float marginOfErrorDegrees = 20;
    public Vector3 initPos;
    public bool complete;
    
    void OnEnable()
    {
        initPos = transform.position;
        desiredRotation = transform.localRotation * Quaternion.Euler(0,180f,0);

        target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        target.transform.GetComponent<SphereCollider>().enabled = false;
        target.transform.GetComponent<MeshRenderer>().enabled = false;
        target.transform.position = transform.forward * 100.0f;

        complete = false;
    }

    public void OnDrawGizmos()
    {
        if (isActiveAndEnabled && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }

    public override Vector3 Calculate()
    {
        return boid.SeekForce(target.transform.position);
    }

    public void Update()
    {
        if (!complete) {
            // Works by rotating a point around the ship to make it turn smoothly
            target.transform.RotateAround(initPos, Vector3.up, turnSpeed);
            
            // Check that current rotation is within the range
            // The mod is used because an example  10 degrees and 360 degrees are seperated by
            // 10 degrees instead of 350. (Loop around)
            float currYRot = transform.rotation.eulerAngles.y;
            float desYRot = desiredRotation.eulerAngles.y;
            float a = (currYRot - marginOfErrorDegrees) % 360;
            float b = (currYRot + marginOfErrorDegrees) % 360;
            float differentA = Mathf.Abs(desYRot - a);
            float differentB = Mathf.Abs(desYRot - b);
            if (differentA < marginOfErrorDegrees || differentB < marginOfErrorDegrees) {
                complete = true;
            }
        }
    }
}
