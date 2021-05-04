using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Alive: State
{
    public override void Think()
    {   
        Fighter fighterComp = owner.GetComponent<Fighter>();

         if (fighterComp.health <= 0) {
            Dead dead = new Dead();
            owner.ChangeState(dead);
            owner.SetGlobalState(dead);
            return;
        }

        if (fighterComp.enemy == null) {
            if (fighterComp.checkEnemyInSight()) {
                owner.ChangeState(new AttackState());
                return;
            }
        }
    }
}
public class Dead:State
{
    public override void Enter()
    {
        // Explode ship
        owner.GetComponent<Explosion>().enabled = true;
        // Disable collider
        owner.GetComponent<BoxCollider>().enabled = false;

        // Disable all steering behaviours
        SteeringBehaviour[] steeringBehaviours = owner.GetComponent<Boid>().GetComponents<SteeringBehaviour>();
        foreach(SteeringBehaviour sb in steeringBehaviours)
        {
            sb.enabled = false;
        }
        owner.GetComponent<StateMachine>().enabled = false;  
    }         
}

public class AttackState : State
{
    public override void Enter()
    {
        owner.GetComponent<Seek>().targetGameObject = owner.GetComponent<Fighter>().enemy;//.GetComponent<Boid>();
        owner.GetComponent<Seek>().enabled = true;
        owner.GetComponent<Boid>().maxSpeed = 15.0f;
    }

    public override void Think()
    {
        if (owner.GetComponent<Fighter>().enemy == null) {
            owner.ChangeState(new IdleState());
            return;
        }

        Vector3 toEnemy = owner.GetComponent<Fighter>().enemy.transform.position - owner.transform.position; 
        if (Vector3.Angle(owner.transform.forward, toEnemy) < 45 && toEnemy.magnitude < 100.0f)
        {
            owner.GetComponent<Fighter>().fire();
        }

        float distToEnemy = Vector3.Distance(
            owner.GetComponent<Fighter>().enemy.transform.position, 
            owner.transform.position
        );

        if (distToEnemy < 20.0f) {
            owner.ChangeState(new TemporaryRetreatState());
        } 
        else if (distToEnemy < 50.0f)
        {
            owner.GetComponent<Seek>().enabled = false;
            owner.GetComponent<Pursue>().target = owner.GetComponent<Fighter>().enemy.GetComponent<Boid>();
            owner.GetComponent<Pursue>().enabled = true;

        }

    }

    public override void Exit()
    {
        owner.GetComponent<Seek>().enabled = false;
        owner.GetComponent<Pursue>().enabled = false;
    }
}

// This state is set when the unit gets too close to the enemy
public class TemporaryRetreatState : State
{
    public override void Enter()
    {
        owner.GetComponent<Boid>().maxSpeed = 15.0f;
        owner.GetComponent<Flee>().targetGameObject = owner.GetComponent<Fighter>().enemy;
        owner.GetComponent<Flee>().enabled = true;

        float retreatDistance = Random.Range(20.0f, owner.GetComponent<Fighter>().maxRetreatDistance);
        owner.GetComponent<Fighter>().retreatDistance = retreatDistance;
    }

    public override void Think()
    {
        if (owner.GetComponent<Fighter>().enemy) {
            float distToEnemy = Vector3.Distance(
                owner.GetComponent<Fighter>().enemy.transform.position, 
                owner.transform.position
            );

            if (distToEnemy > owner.GetComponent<Fighter>().retreatDistance)
            {
                owner.GetComponent<Flee>().enabled = false;
                owner.GetComponent<SmoothTurnaround>().enabled = true;
            }

            if (owner.GetComponent<SmoothTurnaround>().enabled == true && owner.GetComponent<SmoothTurnaround>().complete == true) {
                owner.GetComponent<SmoothTurnaround>().enabled = false;
                owner.ChangeState(new AttackState());
            }
        } else {
            owner.ChangeState(new IdleState());
        }
    }
    public override void Exit()
    {
        owner.GetComponent<SmoothTurnaround>().enabled = false;
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
                        owner.ChangeState(new FollowerState());
                    }
                }
            }
        }
        else if (owner.transform.parent.transform.tag == "Squad") {
            if (owner.transform.tag == "SquadLeader") {
                owner.ChangeState(new ExitAsteroidField());
            } else {
                owner.ChangeState(new FollowerState());
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
        OffsetPursue offsetPursue = owner.GetComponent<OffsetPursue>();
        Squad squad = owner.transform.parent.transform.GetComponent<Squad>();
        Vector3 offset = squad.getSquadOffset(owner.gameObject);

        offsetPursue.predefinedOffset = true;
        offsetPursue.leader = squad.leader.GetComponent<Boid>();
        // offsetPursue.setOffset(offset);
        offsetPursue.offset = offset;

        owner.GetComponent<ObstacleAvoidance>().enabled = true;
        owner.GetComponent<OffsetPursue>().enabled = true;
    }
    public override void Think()
    {
        OffsetPursue offsetPursue = owner.GetComponent<OffsetPursue>();
        Squad squad = owner.transform.parent.transform.GetComponent<Squad>();
        Vector3 offset = squad.getSquadOffset(owner.gameObject);

        offsetPursue.leader = squad.leader.GetComponent<Boid>();
        offsetPursue.offset = offset;
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

class ExitAsteroidField: State
{
    public override void Enter()
    {
        owner.GetComponent<Boid>().maxSpeed = 10.0f;
        GameObject[] stopPoints = GameObject.FindGameObjectsWithTag("AsteroidFieldEntry");
        GameObject stopPoint = stopPoints[0];

        owner.GetComponent<Arrive>().targetGameObject = stopPoint;
        owner.GetComponent<Arrive>().enabled = true;
    }

    public override void Think()
    { 
        GameObject seekTarget = owner.GetComponent<Arrive>().targetGameObject;
        Vector3 targetPos = seekTarget.transform.position;
        float dist = Vector3.Distance(owner.transform.position, targetPos);

        if (dist < 5.0f) {
            // Stop the Ship
            owner.GetComponent<Arrive>().enabled = false;

            owner.ChangeState(new ReturnToStation());
        }
    }

    public override void Exit()
    {

    }
}

class ReturnToStation: State
{
    public override void Enter()
    {
        GameObject[] stopPoints = GameObject.FindGameObjectsWithTag("EndingPoint");
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

            // owner.ChangeState(new PrepareScoutDeployment());
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