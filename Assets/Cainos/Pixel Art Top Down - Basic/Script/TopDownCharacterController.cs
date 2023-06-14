using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float speed;

        private Animator animator;

        private SpriteRenderer _renderer;

        private void Start()
        {
            animator = GetComponent<Animator>();

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null)
            {
                Debug.LogError("Player Sprite is missing a renderer");
            }
        }


        private void Update()
        {
            
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
                _renderer.flipX = true;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
                _renderer.flipX = false;
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }
            if (Input.GetKey(KeyCode.J))
            {
                animator.SetBool("Attack", true);
            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);
            animator.SetBool("Attack", false);

            GetComponent<Rigidbody2D>().velocity = speed * dir;
        }
    }
}
