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

        private Animator animator;

        private AgentMover agentMover;

        private Vector2 movementInput;

        private SpriteRenderer _renderer;

        [SerializeField]
        private InputActionReference movement, attack;

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
    
        }

        public void Attacks()
        {
            if (attackBlocked)
                return;
            animator.SetTrigger("Attacks");
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
        }

        private void OnDisable() {
            attack.action.performed -= PerformAttack;
        }

        private void PerformAttack(InputAction.CallbackContext obj)
        {
            Attacks();
        }
    
    }
}
