using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public int minValue = 1;
    public int maxValue = 10000;
    private int randomValue;
    public float backwardDistance = 2f;
    private bool movingToAttack = false;
    private bool movingAway = false;

    public float strafeDistance = 3f;
    public float strafeSpeed = 3f;
    private bool isStrafing = false;
    private Vector3 strafeDirection;

    private bool attackBlocked;
    public float delay = 0.3f;
    private List<string> EnemyBehaviors = new List<string>();

    private Vector2 movementInput;


    private void Start()
    {

        animator = GetComponent<Animator>();
        vision = GetComponent<CircleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        _renderer = GetComponent<SpriteRenderer>();
        movementInput = Vector2.zero;
        EnemyHealth = 300;
        

    }

    private void Update()
    {
        attackDistanceCheck();

        
    }


    //*******************************************
    // Items related to enemy movement and states

    private void moveTowardsPlayer()
    {
        /**
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
        **/
        if (target != null)
        {
            
            Vector3 direction = target.transform.position - transform.position;
            float distanceToTarget = direction.magnitude; // Using magnitude for distance
            direction.Normalize();

            if (!movingToAttack)
            {
                randomValue = Random.Range(minValue, maxValue);
                // Check if within strafe distance
                if (distanceToTarget <= strafeDistance)
                {
                    isStrafing = true;
                    strafeDirection = Vector3.Cross(direction, Vector3.forward); // Get perpendicular direction for strafing
                }
                else
                {
                    isStrafing = false;
                }

                if (!isStrafing)
                {
                    // Regular movement towards the player
                    transform.Translate(direction * moveSpeed * Time.deltaTime);
                    animator.SetBool("IsMoving", true);
                }
                else
                {
                    // Strafing movement
                    transform.Translate(strafeDirection * strafeSpeed * Time.deltaTime);
                    animator.SetBool("IsMoving", true);
                }
            }
            


            if (randomValue == 5 && !movingAway)
            {
                movingToAttack = true;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                animator.SetBool("IsMoving", true);
            }
            if (movingAway & movingToAttack)
            {
                MoveBackward();
            }
        }

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

    
    private void MoveBackward()
    {
        movingAway = true;
        Vector3 direction = target.transform.position - transform.position;
        transform.Translate(-direction * moveSpeed * Time.deltaTime);
        animator.SetBool("IsMoving", true);
        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        if (distanceToTarget > 3)
        {
            movingToAttack = false;
            movingAway = false;
        }



    }
    



    //*******************************************
    // Items related to the Enemies status (Health, Damage, etc)

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

    //********************************************
    // Items related to Enemy Collision

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




    //****************************************************
    // Items related to attacks and animations

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
        MoveBackward();
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

    //*********************************************
    // Items related to Behaviors and choosing moves

    public void getBehavior(List<string> behaviors)
    {
        EnemyBehaviors = behaviors;
    }

    private void enemyAction()
    {
        /**
        bool shouldStrafe = UnityEngine.Random.value < 0.3f; // Adjust probability as needed

        if (shouldStrafe)
        {
            // Randomize strafe direction
            strafeDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            isStrafing = true;
        }

        if (isStrafing)
        {
            // Strafe movementw
            transform.Translate(strafeDirection * strafeSpeed * Time.deltaTime);
            animator.SetBool("IsMoving", true);
        }
        **/
        //else
        //{
            string chosenMove = GetRandomItem(EnemyBehaviors);
            if (chosenMove == "Sw")
            {
                Attacks();
                //MoveBackward();
            
            }
            else if (chosenMove == "Sp")
            {
                Throws();
                //MoveBackward();
        }
        // }

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

}