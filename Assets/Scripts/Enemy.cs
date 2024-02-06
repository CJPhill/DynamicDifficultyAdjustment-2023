using System.Collections;
using System.Collections.Generic;
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
    public float stopingDistance;

    private bool attackBlocked;
    public float delay = 0.3f;
    private List<string> closeRangeMoves = new List<string>();


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
        attackDistanceCheck();

        
    }

    private void moveTowardsPlayer()
    {
        if (target != null)
        {
            //Get coordinate of player, Close in until in "Action zone"
            //Action zone function
            Vector3 direction = target.transform.position - transform.position; direction = Vector3.Normalize(direction);
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        //else continue idle?
    }

    private void attackDistanceCheck()
    {
        
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            //Debug.Log(distanceToTarget);
            if (distanceToTarget > stopingDistance)
            {

                moveTowardsPlayer();

            }
            else
            {
                enemyAction();

            }
            if (distanceToTarget > 20)
            {
                transform.Translate(Vector2.zero);
                //Add a "wander/idle" function here later
            }
        }
        

    }

    private void enemyAction()
    {

        closeRangeMoves.Add("Sw");
        closeRangeMoves.Add("Sw");
        closeRangeMoves.Add("Sw");
        closeRangeMoves.Add("Sw");
        closeRangeMoves.Add("Sp");
        closeRangeMoves.Add("Sp");

        //Have enemy decide an action here (can be updated later for DDA)
        //Random choice based on location 
        //Close: > % sword chance
        //Mid: > % spear chance
        //long: > % bow
        //how to simualte chance? Custom list randomly choose? Ex. [Sw, Sw, Sw, Sw, Sw, Sp, Sp, B] random choice

        string chosenMove = GetRandomItem(closeRangeMoves);
        if (chosenMove == "Sw")
        {
            Attacks();
        } 
        else if (chosenMove == "Sp")
        {
            Throws();
        }

    }

    private string GetRandomItem(List<string> list)
    {
        // Check if the list is not empty
        if (list.Count > 0)
        {
            // Use UnityEngine.Random for Unity projects
            // If not using Unity, you can use System.Random
            // Random rand = new Random();

            // Unity-specific randomization
            int randomIndex = UnityEngine.Random.Range(0, list.Count);

            // Get the item at the random index
            string randomItem = list[randomIndex];

            return randomItem;

        }
        else
        {
            return null;
        }
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


    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }


    public void Attacks()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attacks");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }
    public void Throws()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Throws");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }
    public void BowShoot()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Bow");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    public void Rolls()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Rolls");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

}