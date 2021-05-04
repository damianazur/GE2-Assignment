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

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GameObject.FindGameObjectsWithTag("MusicPlayer")[0].GetComponent<AudioSource>();

        currentLookAt = initialScoutFighter;
        if (mainCamera && startOnScene == 1) {
            initialCamera();

        } else if (startOnScene == 2) {
            /* -----------------
                DEBUG NOTE:
                - Make sure to revert the initialScoutFighter to the original one.
            ----------------- */
            GameObject[] scene1Objects  = GameObject.FindGameObjectsWithTag("Scene1");
            if (scene1Objects.Length > 0) {
                scene1Objects[0].active = false;
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

        if (Time.fixedTime > 10.0f && sequenceNumber == 1) {
            setDynamicText("");
            sequenceNumber += 1;
        }

        else if (sequenceNumber == 2) {
            lerpLookAt(initialScoutFighter, 0.3f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "PrepareScoutDeployment") {
                sequenceNumber += 1;
            }
        }

        else if (sequenceNumber == 3) {
            lerpLookAt(initialScoutFighter, 0.3f);
        
            Vector3 moveCamTo = new Vector3(7.137265f, 41.48331f, -52.30124f);
            lerpCamTo(moveCamTo, 0.1f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "DeployToAsteroidField") {
                sequenceNumber += 1;
            }
        }

        else if (sequenceNumber == 4) {
            Vector3 moveCamTo = initialScoutFighter.transform.position;
            float camMoveSpeed = 0.05f;
            float shipToStationDist = Vector3.Distance(initialScoutFighter.transform.position, station.transform.position);
            if (shipToStationDist > 130) {
                camMoveSpeed = 0.2f;
            } else {
                moveCamTo.y += 25.0f;
            }
            lerpCamTo(moveCamTo, camMoveSpeed);
            lerpLookAt(initialScoutFighter, 0.4f);

            float shipToCamDist = Vector3.Distance(initialScoutFighter.transform.position, mainCamera.transform.position);
            if (shipToCamDist < 40.0f) {
                mainCamera.transform.LookAt(initialScoutFighter.transform);
                Vector3 newCamPos = initialScoutFighter.transform.position + initialScoutFighter.transform.forward * 40.0f;
                newCamPos.y -= 1.0f;
                mainCamera.transform.position = newCamPos;
                sequenceNumber += 1;
            }
        }

        else if (sequenceNumber == 5) {
            mainCamera.transform.LookAt(initialScoutFighter.transform);
            float xRot = mainCamera.transform.localRotation.eulerAngles.x;
            if (xRot < 275 && xRot > 165) {
                print("Loading New Scene");
                sequenceNumber += 1;
            }
        }

        else if (sequenceNumber == 6) {
            RenderSettings.skybox = scene2Skybox;
            sequenceNumber += 1;
        }

        else if (sequenceNumber == 7) {
            float camLookSpeed = 100f;
            lerpLookAt(initialScoutFighter, camLookSpeed);
            float shipToCamDist = Vector3.Distance(initialScoutFighter.transform.position, mainCamera.transform.position);
            if (shipToCamDist > 40.0f) {
                sequenceStartTime = 0;
                sequenceNumber += 1;
            }
        }

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

        else if (sequenceNumber == 9) {
            prevSequenceNum = 9;
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

            Vector3 moveCamTo = currentLookAt.transform.position - initialScoutFighter.transform.forward * 5.0f;
            // moveCamTo = moveCamTo - initialScoutFighter.transform.right * 20.0f;
            moveCamTo.y += 10.0f;
            lerpCamTo(moveCamTo, 0.4f);

            Vector3 newCamLook = currentLookAt.transform.position;
            newCamLook += currentLookAt.transform.forward * 10.0f;
            lerpLookAt(newCamLook, 10.0f);
        }

        else if (sequenceNumber == 10) {
            if (musicPlayer.clip != endingMusic) {
                musicPlayer.clip = endingMusic;
                musicPlayer.Play(0);
            }

            Vector3 newCamPos = currentLookAt.transform.position + currentLookAt.transform.forward * 30.0f;
            newCamPos.y += 10.0f;
            mainCamera.transform.position = newCamPos;

            Vector3 newCamLook = currentLookAt.transform.position;
            newCamLook -= currentLookAt.transform.forward * 10.0f;
            mainCamera.transform.LookAt(newCamLook);	
        }
        
        else if (sequenceNumber == 1100) {
            sequenceStartTime += Time.deltaTime;
            if (sequenceStartTime > 3.0f) {
                sequenceNumber = prevSequenceNum;
            }
        }
    }
}
