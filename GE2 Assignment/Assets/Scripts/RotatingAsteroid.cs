using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingAsteroid : MonoBehaviour
{
    public float rotateSpeedX = .5f;
    public float rotateSpeedY = .1f;
    public float rotateSpeedZ = 0f;

    void Start() {
        rotateSpeedX = Random.Range(-rotateSpeedX, rotateSpeedX);
        rotateSpeedY = Random.Range(-rotateSpeedY, rotateSpeedY);
        rotateSpeedZ = Random.Range(-rotateSpeedZ, rotateSpeedZ);
    }
    
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-rotateSpeedX * Time.deltaTime ,-rotateSpeedY * Time.deltaTime,-rotateSpeedZ * Time.deltaTime);
    }
}
