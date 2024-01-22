using UnityEngine;

public class Enemy : MonoBehaviour
{

    private CircleCollider2D vision;
    private Animator animator;
    private GameObject target;

    private int EnemyHealth;

    private void Start()
    {

        animator = GetComponent<Animator>();
        vision = GetComponent<CircleCollider2D>();
        EnemyHealth = 100;
    }

    private void Update()
    {
        targetPlayer();
    }

    private void targetPlayer()
    {
        if (target != null)
        {
            //Get coordinate of player, Close in until in "Action zone"
            //Action zone function
            enemyAction();
        }
        //else continue idle?
    }

    private void enemyAction()
    {
        //Have enemy decide an action here (can be updated later for DDA)
        //Random choice based on location 
        //Close: > % sword chance
        //Mid: > % spear chance
        //long: > % bow
        //how to simualte chance? Custom list randomly choose? Ex. [Sw, Sw, Sw, Sw, Sw, Sp, Sp, B] random choice

    }

    public void EnemyTakeDamage(int damage)
    {
        EnemyHealth -= damage;
        if (EnemyHealth <= 0)
        {
            EnemyDie();
        }
    }

    private void EnemyDie()
    {
        //kill the Enemy

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If Player enters collider
        //Target player
        //TODO: Check to make sure it enters vision NOT
        if (collision.gameObject.CompareTag("Player"))
        {
            target = collision.gameObject;

        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }



}