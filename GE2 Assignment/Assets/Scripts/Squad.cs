using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public string squadFormation;
    public int maxMembers = 0;
    public int numSquadMembers = 0;
    public GameObject leader;
    private List<Vector3> squadPositions = new List<Vector3>();
    private List<Vector3> squadOffsets = new List<Vector3>();
    public List<GameObject> squadMembers = new List<GameObject>();
    // public List<int> followerIds = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        if (maxMembers != 0) {
            generateSquadPositions();
        }

    }

    // Positions only need to be generated once
    void generateSquadPositions() {
        print("Generating Squad Pos: " +  maxMembers);
        if (squadFormation == "triangle") {
            /* --------------------------
                    ^
                  ^   ^
                ^       ^
            -------------------------- */

            float gapDist = 10.0f;

            for (int i = 0; i < maxMembers; i++) {
                // Pick left or right side
                float side = (i % 2);
                if (side == 0) {
                    side = -1;
                } else {
                    side = 1;
                }
                
                // print(Mathf.Ceil((i + 1.0f) / 2.0f));
                float appliedGap = gapDist + (Mathf.Ceil((i + 1.0f) / 2.0f) -1) * gapDist;

                // Position
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "squad_pos_" + i.ToString();
                sphere.transform.GetComponent<SphereCollider>().enabled = false;
                sphere.transform.GetComponent<MeshRenderer>().enabled = true;
                sphere.transform.position = leader.transform.position + leader.transform.forward * (appliedGap * -1);
                sphere.transform.position = sphere.transform.position + leader.transform.right * (appliedGap * side);
                squadPositions.Add(sphere.transform.position);

                // Offset
                Vector3 offset = sphere.transform.position - leader.transform.position;
                offset = Quaternion.Inverse(leader.transform.rotation) * offset;
                squadOffsets.Add(offset);
            }

            leader.tag = "SquadLeader";
        }
    }

    public Vector3 getSquadPosition(GameObject follower) {
        int index = squadMembers.IndexOf(follower);
        // print("INDEX: " + index + " " + squadPositions[index]);
        return squadPositions[index];
    }
    public Vector3 getSquadOffset(GameObject follower) {
        int index = squadMembers.IndexOf(follower);
        // print("INDEX: " + index + " " + squadPositions[index]);
        return squadOffsets[index];
    }

    public bool joinSquad(GameObject follower) {
        if (numSquadMembers < maxMembers) {
            numSquadMembers += 1;
            follower.transform.parent = transform;
            squadMembers.Add(follower);

            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (squadMembers.Count > 0) {
            for (int i = squadMembers.Count - 1; i > 0; i--) {
                GameObject member = squadMembers[i];
                if (member.transform.GetComponent<StateMachine>().currentState.GetType().Name == "Dead" && member.transform.tag != "SquadLeader") {
                squadMembers.RemoveAt(i);
                numSquadMembers -= 1;
                }
            }
        }

        if (leader != null) {
            string leaderStatus = leader.GetComponent<StateMachine>().currentState.GetType().Name;
            
            if (leaderStatus == "Dead") {
                foreach (Transform unit in transform) {
                    if (unit.GetComponent<StateMachine>().currentState.GetType().Name != "Dead") {
                        leader = unit.gameObject;
                        leader.tag = "SquadLeader";
                        break;
                    }
                }
            }
        }
       
    }
}
