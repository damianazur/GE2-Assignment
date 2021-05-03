using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider c)
    {
        // print(c.tag);
        if (c.tag == "Bullet")
        {
            Destroy(c.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
