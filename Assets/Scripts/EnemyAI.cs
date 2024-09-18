using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class EnemyAI : MonoBehaviour
{
    private GameManager gameManager;
    private CircleCollider2D vision;
    public Transform target;

    private Animator animator;
    private SpriteRenderer _renderer;

    Seeker seeker;
    Rigidbody2D rb;

    public float speed = 200f;
    private int EnemyHealth;


    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    private bool attackBlocked;
    public float delay = 0.3f;
    private List<string> EnemyBehaviors = new List<string>();

    

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        vision = GetComponent<CircleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        EnemyHealth = 300;

        InvokeRepeating("UpdatePath", 0f, .5f);
        
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && (target))
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (path == null)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath=true;
            return;
        }
        else
        {
            reachedEndOfPath=false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If Player enters collider
        //TODO: Make it start when entering vision
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject.transform;

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

    // Items related to attacks and animations

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
        //MoveBackward();
    }
    public void Attacks()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attacks");
        attackBlocked = true;
        gameManager.recordEnemyAttack();
        StartCoroutine(DelayAttack());
    }
    public void Throws()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Throws");
        attackBlocked = true;
        gameManager.recordEnemyAttack();
        StartCoroutine(DelayAttack());
    }
    public void BowShoot()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Bow");
        attackBlocked = true;
        gameManager.recordEnemyAttack();
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
