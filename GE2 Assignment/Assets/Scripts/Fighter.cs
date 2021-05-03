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
            // print(enemies[i].name);
            StateMachine enemyStateMachine = enemies[i].transform.parent.GetComponent<StateMachine>();
            
            if (enemyStateMachine != null) {
                // print(enemyStateMachine.currentState.GetType().Name);
                string enemyState = enemyStateMachine.currentState.GetType().Name;

                // print("Dist to enemy: " + enemyAffiliation + " " + distToEnemy);
                if (distToEnemy < viewRange && enemyState != "Dead") {
                    enemy = enemies[i].transform.parent.gameObject;
                    return true;
                }
            }
        }

        return false;
    }

    public void OnTriggerEnter(Collider c)
    {
        // print(c.tag);
        if (c.tag == "Bullet")
        {
            string bulletAffiliation = c.gameObject.transform.Find("AffiliationTag").tag;
            print(bulletAffiliation + " " + enemyAffiliation);
            if (bulletAffiliation + "Unit" == enemyAffiliation) {
                if (health > 0)
                {            
                    health --;
                }
                Destroy(c.gameObject);
            }
            // if (GetComponent<StateMachine>().currentState.GetType() != typeof(Dead))
            // {
            //     GetComponent<StateMachine>().ChangeState(new DefendState());    
            // }
        } 
        else if (c.tag == "Asteroid") {
            // health = 0;
        }
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
        if (enemy != null) {
            string enemyState = enemy.transform.GetComponent<StateMachine>().currentState.GetType().Name;
            if (enemyState == "Dead") {
                enemy = null;
            }
        }
    }
}
