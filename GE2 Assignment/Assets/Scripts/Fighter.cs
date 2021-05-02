using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float health = 10;
    public GameObject bullet;
    public GameObject enemy;
    public GameObject enemyAffiliation;
    public float viewRange = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool checkEnemyInSight() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(fighterComp.enemyAffiliation);

        for (int i = 0; i < enemies.Length; i++) {
            float distToEnemy = Vector3.Distance(transform.position, enemies[i].transform.position);
            if (distToEnemy < viewRange) {
                enemy = enemies[i].transform.parent;
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
