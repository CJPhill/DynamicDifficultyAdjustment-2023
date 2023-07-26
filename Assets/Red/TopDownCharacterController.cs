using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        public float delay = 0.3f;

        private bool attackBlocked;

        public float meleeDamage = 10f; // Damage dealt by the player's melee attack

        public float attackRange = 5f; // The range of the player's melee attack

        public LayerMask enemyLayer;

        private Animator animator;

        private AgentMover agentMover;

        private Vector2 movementInput;

        private SpriteRenderer _renderer;

        public float attackCooldown = 1f; // 1 second cooldown between attacks

        private float lastAttackTime;

        [SerializeField]
        private InputActionReference movement, attack, Throw, Roll, Bow;

        private void Start()
        {
            animator = GetComponent<Animator>();

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null)
            {
                Debug.LogError("Player Sprite is missing a renderer");
            }
        }

        private void Awake() {
            agentMover = GetComponent<AgentMover>();
        }


        private void Update()
        {
            
            movementInput = movement.action.ReadValue<Vector2>();

            agentMover.MovementInput = movementInput;
            
            
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
                _renderer.flipX = true;
            }
            else if (movementInput.x == 1)
            {
                animator.SetInteger("Direction", 0);
                _renderer.flipX = false;
            }
            animator.SetBool("IsMoving", movementInput.magnitude > 0);

            //sets X and Y in the animator tool
            if (movementInput != Vector2.zero){
                animator.SetFloat("XInput", movementInput.x);
                animator.SetFloat("YInput", movementInput.y);
            }
    
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
        public void BowShoot() {
             if (attackBlocked)
                return;
            animator.SetTrigger("Bow");
            attackBlocked = true;
            StartCoroutine(DelayAttack());
        }

        public void Rolls() {
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
        }

        //Player input for attacking 
        private void OnEnable() {
            attack.action.performed += PerformAttack;
            Throw.action.performed += PerformThrow;
            Roll.action.performed += PerformRoll;
            Bow.action.performed += PerformBow;
        }

        private void OnDisable() {
            attack.action.performed -= PerformAttack;
            Throw.action.performed -= PerformThrow;
            Roll.action.performed -= PerformRoll;
            Bow.action.performed -= PerformBow;
        }

        private void PerformAttack(InputAction.CallbackContext obj)
        {
            if (Time.time >= lastAttackTime + attackCooldown && Input.GetButtonDown("Fire1")) // Let's assume "Fire1" is your attack button
            {
                Attacks();
                PerformMeleeAttack();
                //Debug.Log("Melee attack!");
                lastAttackTime = Time.time;
            }
            
        }

        private void PerformThrow(InputAction.CallbackContext obj) {
            Throws();
        }
        
        private void PerformRoll(InputAction.CallbackContext obj) {
            Rolls();
        }
        private void PerformBow(InputAction.CallbackContext obj) {
            BowShoot();
        }

        void PerformMeleeAttack()
    {
        // Detect enemies in range of the attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
 
        // Damage them
        foreach (Collider2D enemy in hitEnemies)
        {
            BoxCollider2D enemyBoxCollider = enemy.GetComponent<BoxCollider2D>();

        // Only hit enemies with a BoxCollider2D
            if (enemyBoxCollider != null)
            {
                EnemyRecieveDamage enemyDamage = enemy.GetComponent<EnemyRecieveDamage>();
                if (enemyDamage != null)
                {
                    enemyDamage.DealDamage(meleeDamage, transform.position);
                }
            }
        } 
    }
    
    }
}
