using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursue : SteeringBehaviour
{
    public Boid leader;
    Vector3 targetPos;
    Vector3 worldTarget;
    public Vector3 offset;
    public bool predefinedOffset = false;

    // Start is called before the first frame update
    void Start()
    {
        if (predefinedOffset == false) {
            offset = transform.position - leader.transform.position;
            offset = Quaternion.Inverse(leader.transform.rotation) * offset;
        }
    }

    public void setOffsetWithPosition(Vector3 offsetTarget) {
        Vector3 newOffset = offsetTarget - leader.transform.position;
        offset = Quaternion.Inverse(leader.transform.rotation) * newOffset;
    }

    // Update is called once per frame
    public override Vector3 Calculate()
    {
        worldTarget = leader.transform.TransformPoint(offset);
        float dist = Vector3.Distance(transform.position, worldTarget);
        float time = (dist / boid.maxSpeed) / 2;

        targetPos = worldTarget + (leader.velocity * time);
        return boid.ArriveForce(targetPos);
    }
}
