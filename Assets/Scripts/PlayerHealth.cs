using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    private float health = 0f;
    [SerializeField] private float maxHealth = 100f;

    private void Start() {
        health = maxHealth;
    }

    public void RestoreHealth(float heal) {
        health += heal;
        if (health > maxHealth) {
            health = maxHealth;
        } else if (health <= 0f) {
            health = 0f;
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
        //TODO: End game cimematic thing*
        Debug.Log("die");
    }
    

}