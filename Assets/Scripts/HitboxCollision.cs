using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HitboxCollision : MonoBehaviour
{
    [SerializeField]
    private int damage = 100;
    private GameObject parentGameObject;
    private Animator animator;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        parentGameObject = gameObject.transform.parent.gameObject;
        animator = parentGameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        changePosition();
    }

    public void changeDamage(int damageChange)
    {
        damage = damageChange;
    }

    //Change position of hitbox
    
    private void changePosition()
    {
        bool normal = true;
        Vector3 theScale = transform.localScale;
        Vector3 hitboxPosition = transform.position;
        if (animator.GetInteger("Direction") == 1 && !normal)
        {
            //Debug.Log("working");
            normal = true;
            theScale.x *= -1; 
        }
        else if (animator.GetInteger("Direction") == 0 && normal)
        {
            //Debug.Log("working2");
            normal = false;
            theScale *= -1;
        }
        transform.localScale = theScale;
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision is BoxCollider2D && parentGameObject.CompareTag("Player"))
        {
            //Harm enemy
            //Debug.Log("you hit an enemy!");
            
            //Fix to get component of script to call "EnemyTakeDamage"
            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            TopDownCharacterController player = parentGameObject.GetComponent<TopDownCharacterController>();
            if (enemyScript != null)
            {
                player.SendAttack();
                enemyScript.EnemyTakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Player") && collision is BoxCollider2D && parentGameObject.CompareTag("Enemy"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                gameManager.increaseHit();

            }
        }
    }
}
