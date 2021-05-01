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

    private List<int> followerIds = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        if (maxMembers != 0) {
            generateSquadPositions();
        }

    }

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
                
                print(Mathf.Ceil((i + 1.0f) / 2.0f));
                float appliedGap = gapDist + (Mathf.Ceil((i + 1.0f) / 2.0f) -1) * gapDist;

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "squad_pos_" + i.ToString();
                sphere.transform.position = leader.transform.position + leader.transform.forward * (appliedGap * -1);
                sphere.transform.position = sphere.transform.position + leader.transform.right * (appliedGap * side);

                squadPositions.Add(sphere.transform.position);
            }

            leader.tag = "SquadLeader";
        }
    }

    public Vector3 getSquadPosition(GameObject follower) {
        int instanceID = follower.GetInstanceID();
        int index = followerIds.IndexOf(instanceID);
        print("INDEX: " + index);
        return squadPositions[index];
    }

    public bool joinSquad(GameObject follower) {
        if (numSquadMembers < maxMembers) {
            numSquadMembers += 1;
            follower.tag = "Follower";
            follower.transform.parent = transform;
            followerIds.Add(follower.GetInstanceID());

            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        // Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        // foreach (Transform child in allChildren)
        // {
        //     print(child.name);
        // }

        // int childCount =  gameObject.transform.childCount;
        // if (numSquadMembers != childCount) {
        //     print("No. in Squad: " + gameObject.transform.childCount);
        //     numSquadMembers = childCount;
        //     generateSquadPositions();
        // }
    }
}
