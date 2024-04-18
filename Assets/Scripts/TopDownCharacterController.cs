using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class TopDownCharacterController : MonoBehaviour
{
    public float speed;
    public float delay = 0.3f;
    private Vector2 movementInput;
    private bool invert = false;

    private GameManager gameManager;

    //Check
    public GameObject Hitbox;
    private bool attacking = false; // Delete later
    private bool attackBlocked;
    public float attackCooldown = 1f; // 1 second cooldown between attacks
    private float lastAttackTime;

    private Transform characterTransform;

    public float attackRange = 5f; // The range of the player's melee attack
    private string lastAttack = "";


    public LayerMask enemyLayer;
    private Animator animator;
    private SpriteRenderer _renderer;

    private AgentMover agentMover;

    [SerializeField]
    private InputActionReference movement, attack, Throw, Roll, Bow;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Player Sprite is missing a renderer");
        }
        characterTransform = GetComponent<Transform>();
    }

    private void Awake()
    {
        agentMover = GetComponent<AgentMover>();
    }


    private void Update()
    {
        checkForQuit();
        movementInput = movement.action.ReadValue<Vector2>();
        agentMover.MovementInput = movementInput;

        //Checks players direction
        //TODO: add hitbox to this
        if (movementInput.y == -1)
        {
            animator.SetInteger("Direction", 3);
        }
        else if (movementInput.y == 1)
        {
            animator.SetInteger("Direction", 2);
        }
        if (movementInput.x == -1)
        {
            animator.SetInteger("Direction", 1);
            invert = true;
            invertPlayer();
            _renderer.flipX = true;
        }
        else if (movementInput.x == 1)
        {
            animator.SetInteger("Direction", 0);
            invert = false;
            invertPlayer();
            _renderer.flipX = false;
        }
        animator.SetBool("IsMoving", movementInput.magnitude > 0);

        //sets X and Y in the animator tool
        if (movementInput != Vector2.zero)
        {
            animator.SetFloat("XInput", movementInput.x);
            animator.SetFloat("YInput", movementInput.y);
        }

    }
    
    private void invertPlayer()
    {
        Vector2 theScale = transform.localScale;
        if (invert)
        {
            theScale.x *= -1;
            
        }
        else
        {
            theScale.x *= 1;
            
        }
        //Debug.Log(attacking);
    }

    public void Attacks()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attacks");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
        gameManager.recordPlayerAttack();
        lastAttack = "Sw";
    }
    public void Throws()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Throws");
        attackBlocked = true;
        attacking = true;
        StartCoroutine(DelayAttack());
        gameManager.recordPlayerAttack();
        lastAttack = "Sp";
    }
    public void BowShoot()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Bow");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
        gameManager.recordPlayerAttack();
        lastAttack = "B";
    }

    public void Rolls()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Rolls");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
        attacking = false;
    }

    //Player input for attacking 
    private void OnEnable()
    {
        attack.action.performed += PerformAttack;
        Throw.action.performed += PerformThrow;
        Roll.action.performed += PerformRoll;
        Bow.action.performed += PerformBow;
    }

    private void OnDisable()
    {
        attack.action.performed -= PerformAttack;
        Throw.action.performed -= PerformThrow;
        Roll.action.performed -= PerformRoll;
        Bow.action.performed -= PerformBow;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        Attacks();

    }
    private void PerformThrow(InputAction.CallbackContext obj)
    {
        Throws();
    }

    private void PerformRoll(InputAction.CallbackContext obj)
    {
        Rolls();
    }
    private void PerformBow(InputAction.CallbackContext obj)
    {
        BowShoot();
    }

    //recieved from HitboxCollision.cs
    public void SendAttack()
    {
        SendUpdate();
    }

    private void SendUpdate()
    {
        gameManager.receiveData(lastAttack);
    }

    private void checkForQuit()
    {
        if (Input.GetKey("p"))
        {
            Application.Quit();
        }
    }

}

