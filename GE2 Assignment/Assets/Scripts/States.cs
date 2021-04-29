using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class PatrolState : State
{
    public override void Enter()
    {
        owner.GetComponent<FollowPath>().enabled = true;
    }

    public override void Think()
    {
        
    }

    public override void Exit()
    {
        owner.GetComponent<FollowPath>().enabled = false;
    }
}

class DeliverMessage : State
{
    public override void Enter()
    {
        GameObject[] stopPoints = GameObject.FindGameObjectsWithTag("ScoutStopPoint");
        GameObject stopPoint = stopPoints[0];

        owner.GetComponent<Seek>().targetGameObject = stopPoint;
        owner.GetComponent<Seek>().enabled = true;
    }

    public override void Think()
    { 
        GameObject seekTarget = owner.GetComponent<Seek>().targetGameObject;
        Vector3 targetPos = seekTarget.transform.position;
        float dist = Vector3.Distance(owner.transform.position, targetPos);

        if (dist < 15.0f) {
            Debug.Log(seekTarget.transform.position);
            owner.GetComponent<Arrive>().targetGameObject = seekTarget;
            owner.GetComponent<Arrive>().enabled = true;
            owner.GetComponent<Seek>().enabled = false;
        }

        if (dist < 0.7f) {
            owner.GetComponent<Arrive>().enabled = false;
        }
    }

    public override void Exit()
    {

    }
}


class GoToDestination : State
{
    public override void Enter()
    {

    }

    public override void Think()
    {
        
    }

    public override void Exit()
    {

    }
}