using System.Collections;
using System.Collections.Generic;
using UnityEngine;



class IdleState: State
{
    public override void Enter()
    {

    }

    public override void Think()
    {
        if (owner.tag == "Untagged") {
            GameObject[] squadLeaders = GameObject.FindGameObjectsWithTag("SquadLeader");
            for (int i = 0; i < squadLeaders.Length; i++) {
                GameObject squadLeader = squadLeaders[i];
                float distToLeader = Vector3.Distance(owner.transform.position, squadLeader.transform.position);

                if (distToLeader < 300.0f) {
                    Transform squadObject = squadLeader.transform.parent;
                    Squad squad = squadObject.GetComponent<Squad>();

                    bool joined = squad.joinSquad(owner.gameObject);
                    if (joined) {
                        OffsetPursue offsetPursue = owner.GetComponent<OffsetPursue>();
                        Vector3 offset = squad.getSquadPosition(owner.gameObject);
                        offsetPursue.predefinedOffset = true;
                        offsetPursue.leader = squadLeader.GetComponent<Boid>();
                        offsetPursue.offset = offset;
                        offsetPursue.enabled = true;
                    }
                }
            }
        }
    }

    public override void Exit()
    {

    }
}

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

        owner.GetComponent<Arrive>().targetGameObject = stopPoint;
        owner.GetComponent<Arrive>().enabled = true;
    }

    public override void Think()
    { 
        GameObject seekTarget = owner.GetComponent<Arrive>().targetGameObject;
        Vector3 targetPos = seekTarget.transform.position;
        float dist = Vector3.Distance(owner.transform.position, targetPos);

        if (dist < 0.5f) {
            // Stop the Ship
            Vector3 stopVelocity = new Vector3(0, 0, 0);
            owner.GetComponent<Boid>().velocity = stopVelocity;
            owner.GetComponent<Arrive>().enabled = false;

            owner.ChangeState(new PrepareScoutDeployment());
        }
    }

    public override void Exit()
    {

    }
}

class PrepareScoutDeployment : State
{
    public override void Enter()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("AsteroidFieldLocation");
        GameObject location = locations[0];

        // owner.GetComponent<FaceDestination>().destinationPos = location.transform.position;
        // owner.GetComponent<FaceDestination>().enabled = true;

        // ------------ DEBUG ------------
        owner.GetComponent<Boid>().maxSpeed = 50;
        owner.GetComponent<FaceDestination>().faceDestinationComplete = true;
        owner.transform.LookAt(location.transform.position);
        // ------------
    }

    public override void Think()
    {
        FaceDestination faceDestComp = owner.GetComponent<FaceDestination>();
        if (faceDestComp.faceDestinationComplete == true) {
            if (faceDestComp.enabled == true) {
                faceDestComp.enabled = false;

            } else if (owner.tag != "SquadLeader") {
                // Promote to leader
                owner.tag = "SquadLeader";
                GameObject squadObject = new GameObject();
                squadObject.name = "Squad_1";
                owner.transform.SetParent(squadObject.transform);

                Squad squad = squadObject.AddComponent<Squad>();
                squad.leader = owner.gameObject;
                squad.maxMembers = 4;
                squad.squadFormation = "triangle";
                squad.enabled = true;
                
            }
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