using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float power = 100.0f;
    public float radius = 50.0F;
    // Start is called before the first frame update
    void Start()
    {
        Transform objects = transform.Find("Objects").transform;
        foreach (Transform child in objects) {
            child.parent = null;
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb == null) {
                child.gameObject.AddComponent<Rigidbody>();
            }

            if (rb != null) {
                // Vector3 explosionPos = child.position;
                // rb.AddExplosionForce(power, explosionPos, radius, 3.0F);
                // rb.velocity = Random.onUnitSphere * 100.0f;
                rb.AddForce(child.forward * 100.0f, ForceMode.Impulse);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
