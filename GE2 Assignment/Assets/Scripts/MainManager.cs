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
    public GameObject initialScoutFighter;
    public GameObject station;
    public Material scene1Skybox;
    public Material scene2Skybox;

    private float sequenceStartTime = 0;

    public int startOnScene = 1;

    // Start is called before the first frame update
    void Start()
    {
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

    void lerpCamTo(Vector3 moveCamTo, float moveSpeed) {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, moveCamTo, Time.deltaTime * moveSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime > 10.0f && sequenceNumber == 1) {
            setDynamicText("");
            sequenceNumber += 1;
        }

        if (sequenceNumber == 2) {
            lerpLookAt(initialScoutFighter, 0.3f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "PrepareScoutDeployment") {
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 3) {
            lerpLookAt(initialScoutFighter, 0.3f);
        
            Vector3 moveCamTo = new Vector3(7.137265f, 41.48331f, -52.30124f);
            lerpCamTo(moveCamTo, 0.1f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "DeployToAsteroidField") {
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 4) {
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

        if (sequenceNumber == 5) {
            mainCamera.transform.LookAt(initialScoutFighter.transform);
            float xRot = mainCamera.transform.localRotation.eulerAngles.x;
            if (xRot < 275 && xRot > 165) {
                print("Loading New Scene");
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 6) {
            RenderSettings.skybox = scene2Skybox;
            sequenceNumber += 1;
        }

        if (sequenceNumber == 7) {
            float camLookSpeed = 100f;
            lerpLookAt(initialScoutFighter, camLookSpeed);
            float shipToCamDist = Vector3.Distance(initialScoutFighter.transform.position, mainCamera.transform.position);
            if (shipToCamDist > 40.0f) {
                sequenceStartTime = 0;
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 8) {
            sequenceStartTime += Time.deltaTime;
            Vector3 moveCamTo = initialScoutFighter.transform.position;
            moveCamTo.y += 5.0f;
            lerpCamTo(moveCamTo, 0.4f);
            lerpLookAt(initialScoutFighter, 0.4f);

            if (sequenceStartTime > 5.0f) {
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 9) {
            Vector3 moveCamTo = initialScoutFighter.transform.position - initialScoutFighter.transform.forward * 15.0f;
            // moveCamTo = moveCamTo - initialScoutFighter.transform.right * 20.0f;
            moveCamTo.y += 20.0f;
            lerpCamTo(moveCamTo, 0.4f);
            lerpLookAt(initialScoutFighter, 10.0f);
        }
    }
}
