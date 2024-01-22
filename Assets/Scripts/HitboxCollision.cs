using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollision : MonoBehaviour
{
    private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision is BoxCollider2D)
        {
            //Harm enemy
            Debug.Log("you hit an enemy!");

            //Fix to get component of script to call "EnemyTakeDamage"
            Enemy enemyScript = collision.GetComponent<Enemy>;
            if (enemyScript != null)
            {
                enemyScript.EnemyTakeDamage(damage);
            }
        }
    }
}
