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

    // Start is called before the first frame update
    void Start()
    {
        if (mainCamera) {
            initialCamera();
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
        
            Vector3 moveCamTo = new Vector3(13.27258f, 33.85609f, -54.28366f);
            lerpCamTo(moveCamTo, 0.1f);

            string scoutState = initialScoutFighter.GetComponent<StateMachine>().currentState.GetType().Name;
            if (scoutState == "DeployToAsteroidField") {
                sequenceNumber += 1;
            }
        }

        if (sequenceNumber == 4) {
            Vector3 moveCamTo = initialScoutFighter.transform.position;
            lerpCamTo(moveCamTo, 0.05f);
            lerpLookAt(initialScoutFighter, 0.3f);
        }
    }
}
