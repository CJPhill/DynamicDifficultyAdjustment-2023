using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameManager gameManager;

    private CircleCollider2D vision;
    private Animator animator;
    private SpriteRenderer _renderer;
    private GameObject target;

    private int EnemyHealth;
    [SerializeField]
    private int moveSpeed;
    public float stopingDistance;

    private bool attackBlocked;
    public float delay = 0.3f;
    private List<string> closeRangeMoves = new List<string>();

    private Vector2 movementInput;


    private void Start()
    {

        animator = GetComponent<Animator>();
        vision = GetComponent<CircleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        _renderer = GetComponent<SpriteRenderer>();
        movementInput = Vector2.zero;
        EnemyHealth = 100;
        

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
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            if (distanceToTarget > 10)
            {
                animator.SetBool("IsMoving", false);
                transform.Translate(Vector2.zero);
                //Add a "wander/idle" function here later
            }
            else
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                animator.SetBool("IsMoving", true);
            }
            
        }
        
        //else continue idle?
    }

    private void attackDistanceCheck()
    {
        Vector2 selfLocation = transform.position;
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
            Vector2 targetLocation = target.transform.position;

            //Debug.Log(distanceToTarget);

            //Calculate location
            float xDifference = Mathf.Abs(selfLocation.x - targetLocation.x);
            float yDifference = Mathf.Abs(selfLocation.y - targetLocation.y);
            
            if (xDifference > yDifference)
            {
                if (transform.position.x > targetLocation.x)
                {
                    movementInput.x = -1;
                    animator.SetInteger("Direction", 1);
                    _renderer.flipX = true;
                }
                else
                {
                    movementInput.x = 1;
                    animator.SetInteger("Direction", 0);
                    _renderer.flipX = false;
                }
                movementInput.y = 0;
                
            }
            else
            {
                if (transform.position.y > targetLocation.y)
                {
                    movementInput.y = -1;
                    animator.SetInteger("Direction", 3);
                }
                else
                {
                    movementInput.y = 1;
                    animator.SetInteger("Direction", 2);
                }
                movementInput.x = 0;
            
            }
            animator.SetFloat("XInput", movementInput.x);
            animator.SetFloat("YInput", movementInput.y);



            if (distanceToTarget > stopingDistance)
            {

                moveTowardsPlayer();

            }
            else
            {
                enemyAction();

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
            //target = null;
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