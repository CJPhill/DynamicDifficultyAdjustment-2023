using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float speed = 1f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;
    private float canAttack = 0f;
    private Transform target;

    public GameObject projectilePrefab; // The projectile that will be shot
    public float projectileSpeed = 6f; // The speed of the projectile

    
    private void Update(){
        if(target != null){
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        
    }

    private void OnCollisionStay2D(Collision2D other) {
        if(other.gameObject.tag == "Player"){
            if(attackSpeed <= canAttack){
                 other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
                canAttack = 0f;

            }else{
                canAttack += Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            target = other.transform;
            //ShootProjectileAtTarget(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            target = null;

            // When the player exits the trigger, shoot a projectile at the player
            //ShootProjectileAtTarget(other.transform);
        }
    }

    private void ShootProjectileAtTarget(Transform target){
        // Instantiate the projectile at the current position and with the same rotation as the enemy
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if(rb != null){
            // Calculate the direction to the target
            Vector2 direction = (target.position - transform.position).normalized;
            // Apply a force in the direction of the target
            rb.velocity = direction * projectileSpeed;
        }
    }
}