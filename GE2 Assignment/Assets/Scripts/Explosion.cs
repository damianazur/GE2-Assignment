using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float power = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        // Disable the mesh of the ship if it has one
        foreach(Transform child in transform)
        {
            if (child.tag == "Mesh") {
                child.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        // Play explosion animation
        explode();

        // Iterate over movable objects in the explosion and give them a force in a random direction
        Transform objects = transform.Find("Objects").transform;
        objects.gameObject.active = true;
        foreach (Transform child in objects) {
            // Assign a rigidbody if it doesn't have one
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb == null) {
                rb = child.gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;

            // Assign a box collider
            BoxCollider boxc = child.gameObject.AddComponent<BoxCollider>();
            boxc.size = new Vector3(1, 1, 1);

            // Raycast layer so it does not interfere with obstacle avoidance
            child.gameObject.layer = 2;

            // Double check to ensure that the rigidbody is there (should be)
            if (rb != null) {
                Vector3 explosionPos = child.position;
                
                // Force in random direction
                child.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                rb.AddForce(child.forward * power, ForceMode.Impulse);
            }
        }
    }

    void explode() {
        ParticleSystem exp = transform.Find("Particle System").transform.GetComponent<ParticleSystem>();
        exp.Play();
        // Destroy(gameObject, exp.main.duration * 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
