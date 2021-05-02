using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Alive: State
{
    public override void Think()
    {   
        Fighter fighterComp = owner.GetComponent<Fighter>();
        if (fighterComp.enemy == null) {
            
            if (fighterComp.checkEnemyInSight()) {
                owner.ChangeState(new AttackState());
                return;
            }
        }
    }
}

public class AttackState : State
{
    public override void Enter()
    {
        owner.GetComponent<Pursue>().target = owner.GetComponent<Fighter>().enemy.GetComponent<Boid>();
        owner.GetComponent<Pursue>().enabled = true;
        owner.GetComponent<Boid>().maxSpeed = 20.0f;
    }

    public override void Think()
    {
        Vector3 toEnemy = owner.GetComponent<Fighter>().enemy.transform.position - owner.transform.position; 
        // if (Vector3.Angle(owner.transform.forward, toEnemy) < 45 && toEnemy.magnitude < 30)
        // {
        //     GameObject bullet = GameObject.Instantiate(owner.GetComponent<Fighter>().bullet, owner.transform.position + owner.transform.forward * 2, owner.transform.rotation);
        // }
        if (Vector3.Distance(
            owner.GetComponent<Fighter>().enemy.transform.position,
            owner.transform.position) < 10)
        {
            owner.ChangeState(new FleeState());
        }

    }

    public override void Exit()
    {
        owner.GetComponent<Pursue>().enabled = false;
    }
}

public class FleeState : State
{
    public override void Enter()
    {
        owner.GetComponent<Flee>().targetGameObject = owner.GetComponent<Fighter>().enemy;
        owner.GetComponent<Flee>().enabled = true;
    }

    public override void Think()
    {
        if (Vector3.Distance(
            owner.GetComponent<Fighter>().enemy.transform.position,
            owner.transform.position) > 30)
        {
            owner.ChangeState(new AttackState());
        }
    }
    public override void Exit()
    {
        owner.GetComponent<Flee>().enabled = false;
    }
}

class IdleState: State
{
    public override void Enter()
    {

    }

    public override void Think()
    {
        // If not in a squad
        if (owner.transform.parent == null) {
            // Select all squad leaders
            GameObject[] squadLeaders = GameObject.FindGameObjectsWithTag("SquadLeader");
            for (int i = 0; i < squadLeaders.Length; i++) {
                GameObject squadLeader = squadLeaders[i];

                // Check if squad leader belongs to the same affilitation (in other words, is not enemy squad leader)
                string ownerAffiliation = owner.transform.Find("AffiliationTag").tag;
                string leaderAffiliation = squadLeader.transform.Find("AffiliationTag").tag;
                if (ownerAffiliation != leaderAffiliation) {
                    continue;
                }


                float distToLeader = Vector3.Distance(owner.transform.position, squadLeader.transform.position);

                // If squad leader within range check if there are troops needed
                if (distToLeader < 300.0f) {
                    Transform squadObject = squadLeader.transform.parent;
                    Squad squad = squadObject.GetComponent<Squad>();

                    // Join squad if there is space
                    bool joined = squad.joinSquad(owner.gameObject);
                    if (joined) {
                        OffsetPursue offsetPursue = owner.GetComponent<OffsetPursue>();
                        Vector3 offset = squad.getSquadPosition(owner.gameObject);
                        offsetPursue.predefinedOffset = true;
                        offsetPursue.leader = squadLeader.GetComponent<Boid>();
                        offsetPursue.offset = offset;
                        offsetPursue.enabled = true;
                        owner.GetComponent<ObstacleAvoidance>().enabled = true;
                    }
                }
            }
        }
    }

    public override void Exit()
    {

    }
}

class FollowerState : State
{
    public override void Enter()
    {
        owner.GetComponent<OffsetPursue>().enabled = true;
    }
    public override void Think()
    {
        
    }

    public override void Exit()
    {
        owner.GetComponent<OffsetPursue>().enabled = false;
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

        owner.GetComponent<FaceDestination>().destinationPos = location.transform.position;
        owner.GetComponent<FaceDestination>().enabled = true;

        // ------------ DEBUG ------------
        // owner.GetComponent<Boid>().maxSpeed = 50;
        // owner.GetComponent<FaceDestination>().faceDestinationComplete = true;
        // owner.transform.LookAt(location.transform.position);
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
                GameObject squadObject = new GameObject();
                squadObject.name = "Squad_1";
                owner.transform.SetParent(squadObject.transform);

                Squad squad = squadObject.AddComponent<Squad>();
                squad.leader = owner.gameObject;
                squad.maxMembers = 4;
                squad.squadFormation = "triangle";
                squad.enabled = true;

            } else if (owner.tag == "SquadLeader") {
                owner.ChangeState(new DeployToAsteroidField());
            }
        }
    }

    public override void Exit()
    {

    }
}


class DeployToAsteroidField : State
{
    public override void Enter()
    {
        GameObject[] locations = GameObject.FindGameObjectsWithTag("AsteroidFieldLocation");
        GameObject location = locations[0];
        owner.GetComponent<Boid>().maxSpeed = 5;
        owner.GetComponent<Seek>().targetGameObject = location;
        owner.GetComponent<Seek>().enabled = true;
    }

    public override void Think()
    {

    }

    public override void Exit()
    {
        Debug.Log("DeployToAsteroidField Exit");
        owner.GetComponent<Seek>().enabled = false;
    }
}