using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollision : MonoBehaviour
{
    [SerializeField]
    private int damage = 100;
    private GameObject parentGameObject;


    private void Start()
    {
        parentGameObject = gameObject.transform.parent.gameObject;

    }

    public void changeDamage(int damageChange)
    {
        damage = damageChange;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision is BoxCollider2D && parentGameObject.CompareTag("Player"))
        {
            //Harm enemy
            Debug.Log("you hit an enemy!");

            //Fix to get component of script to call "EnemyTakeDamage"
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.EnemyTakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Player") && collision is BoxCollider2D && parentGameObject.CompareTag("Enemy"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
