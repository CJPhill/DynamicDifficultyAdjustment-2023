using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    private float health = 0f;
    [SerializeField] private float maxHealth = 100f;
    private Collider2D playerCollider;

    private void Start() {
        health = maxHealth;
        playerCollider = GetComponent<Collider2D>();
    }

    public void UpdateHealth(float mod) {
        health += mod;
        if (health > maxHealth) {
            health = maxHealth;
        } else if (health <= 0f) {
            health = 0f;
            Debug.Log("Player Respawn");
        }
    }

    public void TakeDamage(float damage) {
        health -= damage;
        Debug.Log("My Health:" + health);
        if (health <= 0) {
            Die();
        }
    }
    public void Die() {
        Destroy(gameObject);
        Debug.Log("die");
    }
    

}