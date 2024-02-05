using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    private CircleCollider2D vision;
    private Animator animator;
    private GameObject target;

    private int EnemyHealth;
    [SerializeField]
    private int moveSpeed;

    private void Start()
    {

        animator = GetComponent<Animator>();
        vision = GetComponent<CircleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        EnemyHealth = 100;
        Debug.Log(gameManager);
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
            Vector3 direction = target.transform.position - transform.position; direction = Vector3.Normalize(direction);
            transform.Translate(direction * moveSpeed * Time.deltaTime);
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
        //kills the Enemy
        Debug.Log("Enemy is dead");
        gameManager.EnemyDeath();
        Destroy(gameObject);
        
        

    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //If Player enters collider
        //Target player
        //TODO: Check to make sure it enters vision NOT
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;

        }

    }
 
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }

}