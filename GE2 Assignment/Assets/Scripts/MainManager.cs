using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Camera mainCamera;
    public Canvas mainCanvas;
    public float delayBetweenTextLetter = 0.3f;
    public string stationTitle = "Station A01";
    public int sequenceNumber = 0;
    private int prevSequenceNum = 0;
    public GameObject initialScoutFighter;
    public GameObject station;
    public Material scene1Skybox;
    public Material scene2Skybox;
    private float sequenceStartTime = 0;
    private GameObject currentLookAt;
    public int startOnScene = 1;
    public AudioClip beginningMusic;
    public AudioClip battleMusic;
    public AudioClip endingMusic;
    private AudioSource musicPlayer;
    private List<GameObject> scenes = new List<GameObject>();

    void Awake() {
        // Get music player to play music (beginning, battle, end)
        musicPlayer = GameObject.FindGameObjectsWithTag("MusicPlayer")[0].GetComponent<AudioSource>();

        // Get scenes so that they can be enabled/disabled as needed
        GameObject[] scene1Objects  = GameObject.FindGameObjectsWithTag("Scene1");
        if (scene1Objects.Length > 0) {
            scenes.Add(scene1Objects[0]);
            scene1Objects[0].active = false;
        }
        GameObject[] scene2Objects  = GameObject.FindGameObjectsWithTag("Scene2");
        if (scene2Objects.Length > 0) {
           scenes.Add(scene2Objects[0]);
           scene2Objects[0].active = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // The camera will begin by looking at the scout heading for the station
        currentLookAt = initialScoutFighter;
        if (mainCamera && startOnScene == 1) {
            scenes[0].active = true;
            initialCamera();

        } else if (startOnScene == 2) {
            /* -----------------
                DEBUG NOTE:
                - Make sure to revert the initialScoutFighter to the original one.
            ----------------- */
            if (scenes.Count > 0) {
                scenes[0].active = false;
            }

            sequenceNumber = 4;
            Vector3 newCamPos = initialScoutFighter.transform.position + initialScoutFighter.transform.forward * 100.0f;;
            mainCamera.transform.position = newCamPos;
        }

        musicPlayer.clip = beginningMusic;
        musicPlayer.Play(0);
    }

    void setDynamicText(string text) {
        Text title = mainCanvas.transform.Find("Title").transform.GetComponent<Text>();
        title.text = text;
    }
    
    // Set camera for first scene (looking at the station)
    void initialCamera() {
        setDynamicText(stationTitle);
        mainCamera.transform.position = new Vector3(23.0f, 22.0f, 27.0f);
        mainCamera.transform.rotation = Quaternion.Euler(1.755f, -135.174f, 0);
        sequenceNumber += 1;
    }

    void lerpLookAt(GameObject target, float lookSpeed) {
        Quaternion lookOnLook = Quaternion.LookRotation(target.transform.position - mainCamera.transform.position);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, lookOnLook, Time.deltaTime * lookSpeed);
    }
    void lerpLookAt(Vector3 target, float lookSpeed) {
        Quaternion lookOnLook = Quaternion.LookRotation(target - mainCamera.transform.position);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, lookOnLook, Time.deltaTime * lookSpeed);
    }

    void lerpCamTo(Vector3 moveCamTo, float moveSpeed) {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, moveCamTo, Time.deltaTime * moveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // This is the check for the last scene. If the enemies have been killed and the 
        // ship is exiting the asteroid field then play the ending music
        GameObject[] squadObj = GameObject.FindGameObjectsWithTag("Squad");
        if (squadObj.Length > 0) {
            Squad squad = squadObj[0].transform.GetComponent<Squad>();
            GameObject squadLeader = squad.leader;
            string leaderState = squadLeader.GetComponent<StateMachine>().currentState.GetType().Name;
            if (leaderState == "ExitAsteroidField") {
                sequenceNumber = 10;

            } else if (leaderState == "AttackState") {
                if (musicPlayer.clip != battleMusic) {
                    musicPlayer.clip = battleMusic;
                    musicPlayer.Play(0);
                }
            }
        }

        // Clear the text after 10 seconds
        if (Time.fixedTime > 10.0f && sequenceNumber == 1) {
            setDynamicText("");
            sequenceNumber += 1;
        }

        // Look at the incoming scout
        else if (sequenceNumber == 2) {
            lerpLookAt(initialScoutFighter, 0.3f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "PrepareScoutDeployment") {
                sequenceNumber += 1;
            }
        }

        // Move the camera behind the stationed ships
        // When the scout is deploying to asteroid field go to next sequence
        else if (sequenceNumber == 3) {
            lerpLookAt(initialScoutFighter, 0.3f);
        
            Vector3 moveCamTo = new Vector3(7.137265f, 41.48331f, -52.30124f);
            lerpCamTo(moveCamTo, 0.1f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "DeployToAsteroidField") {
                sequenceNumber += 1;
            }
        }

        // Increase the camera speed when folling the ship as it is leaving the station 
        // and heading for the asteroid field
        else if (sequenceNumber == 4) {
            Vector3 moveCamTo = initialScoutFighter.transform.position;
            float camMoveSpeed = 0.05f;
            float shipToStationDist = Vector3.Distance(initialScoutFighter.transform.position, station.transform.position);
            if (shipToStationDist > 130) {
                camMoveSpeed = 0.4f;
            } else {
                moveCamTo.y += 25.0f;
            }
            lerpCamTo(moveCamTo, camMoveSpeed);
            lerpLookAt(initialScoutFighter, 0.4f);

            // If the camera has almost reached the ship then change the camera position to be infront of the ship
            // and change the sequence (this is the part right before the transition to the asteroid scene)
            float shipToCamDist = Vector3.Distance(initialScoutFighter.transform.position, mainCamera.transform.position);
            if (shipToCamDist < 40.0f) {
                mainCamera.transform.LookAt(initialScoutFighter.transform);
                Vector3 newCamPos = initialScoutFighter.transform.position + initialScoutFighter.transform.forward * 80.0f;
                newCamPos.y -= 1.0f;
                mainCamera.transform.position = newCamPos;
                sequenceNumber += 1;
            }
        }

        // Wait until the camera is looking directly upwards and change the scene to the asteroid scene
        else if (sequenceNumber == 5) {
            mainCamera.transform.LookAt(initialScoutFighter.transform);
            float xRot = mainCamera.transform.localRotation.eulerAngles.x;
            if (xRot < 275 && xRot > 165) {
                print("Loading New Scene");

                scenes[0].active = false;
                scenes[1].active = true;

                sequenceNumber += 1;
            }
        }

        // Render the scene 2 skybox
        else if (sequenceNumber == 6) {
            RenderSettings.skybox = scene2Skybox;
            sequenceNumber += 1;
        }

        // Keep looking at the ship but don't move until the ship is a certain distance away
        else if (sequenceNumber == 7) {
            float camLookSpeed = 100f;
            lerpLookAt(initialScoutFighter, camLookSpeed);
            float shipToCamDist = Vector3.Distance(initialScoutFighter.transform.position, mainCamera.transform.position);
            if (shipToCamDist > 40.0f) {
                sequenceStartTime = 0;
                sequenceNumber += 1;
            }
        }

        // Move the camera right above the scout and maintain position for a number of seconds 
        // before moving to next sequence
        else if (sequenceNumber == 8) {
            sequenceStartTime += Time.deltaTime;
            Vector3 moveCamTo = initialScoutFighter.transform.position;
            moveCamTo.y += 5.0f;
            lerpCamTo(moveCamTo, 0.4f);
            lerpLookAt(initialScoutFighter, 0.4f);

            if (sequenceStartTime > 5.0f) {
                sequenceNumber += 1;
            }
        }

        // Follow behind the ship and change to a different ship if that ship dies
        else if (sequenceNumber == 9) {
            prevSequenceNum = 9;
            // if the current ship that was looked at died find a different ship in the squad
            if (currentLookAt.transform.GetComponent<StateMachine>().currentState.GetType().Name == "Dead") {
                
                GameObject lightUnitsSquad = GameObject.FindGameObjectsWithTag("Squad")[0];
                foreach (Transform unit in lightUnitsSquad.transform) {
                    if (unit.GetComponent<StateMachine>().currentState.GetType().Name != "Dead") {
                        currentLookAt = unit.gameObject;
                        break;
                    }
                }

                sequenceNumber = 1100;
                sequenceStartTime = 0;
                return;
            }
            
            // Camera looking in front of the ship and camera is above ship
            Vector3 moveCamTo = currentLookAt.transform.position - initialScoutFighter.transform.forward * 5.0f;
            moveCamTo.y += 10.0f;
            lerpCamTo(moveCamTo, 0.4f);

            Vector3 newCamLook = currentLookAt.transform.position;
            newCamLook += currentLookAt.transform.forward * 10.0f;
            lerpLookAt(newCamLook, 10.0f);
        }
        
        // The battle has ended, play battle ending music and show the "THE END" text etc.
        else if (sequenceNumber == 10) {
            if (musicPlayer.clip != endingMusic) {
                musicPlayer.clip = endingMusic;
                musicPlayer.Play(0);
            }
            
            string currentLookAtState = currentLookAt.GetComponent<StateMachine>().currentState.GetType().Name;
            if (currentLookAtState == "ReturnToStation") {
                GameObject theEndTextObj = GameObject.FindGameObjectsWithTag("TheEndText")[0];
                Canvas theEndCanvas = theEndTextObj.transform.Find("Canvas").transform.GetComponent<Canvas>();
                theEndCanvas.enabled = true;
            }

            Vector3 newCamPos = currentLookAt.transform.position + currentLookAt.transform.forward * 30.0f;
            newCamPos.y += 10.0f;
            mainCamera.transform.position = newCamPos;

            Vector3 newCamLook = currentLookAt.transform.position;
            newCamLook -= currentLookAt.transform.forward * 10.0f;
            mainCamera.transform.LookAt(newCamLook);	
        }
        
        // This sequence number is a code for a 3 second delay
        else if (sequenceNumber == 1100) {
            sequenceStartTime += Time.deltaTime;
            if (sequenceStartTime > 3.0f) {
                sequenceNumber = prevSequenceNum;
            }
        }
    }
}
