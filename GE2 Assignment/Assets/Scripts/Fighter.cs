using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float health = 10;
    public GameObject bullet;
    public GameObject enemy;
    public string enemyAffiliation;
    public float viewRange = 100.0f;
    public float maxRetreatDistance = 150.0f;
    public float retreatDistance = 0;
    public int bulletCooldown = 10;
    private int bulletDelayCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool checkEnemyInSight() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyAffiliation);

        for (int i = 0; i < enemies.Length; i++) {
            float distToEnemy = Vector3.Distance(transform.position, enemies[i].transform.position);
            // print("Dist to enemy: " + enemyAffiliation + " " + distToEnemy);
            if (distToEnemy < viewRange) {
                // GameObject enemysTarget = enemies[i].transform.parent.transform.GetComponent<Fighter>().enemy;

                // if (enemysTarget == null || enemysTarget.transform != transform) {
                    enemy = enemies[i].transform.parent.gameObject;
                    return true;
                // }
            }
        }

        return false;
    }

    public void fire() 
    {
        bulletDelayCount += 1;
        if (bulletDelayCount >= bulletCooldown) {
            GameObject bulletInstance = GameObject.Instantiate(bullet, transform.position + transform.forward * 2, transform.rotation);
            bulletDelayCount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
