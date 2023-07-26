using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyRecieveDamage : MonoBehaviour
{
    public float currentHealth;
    [SerializeField] private float maxHealth = 100f;
    public float knockbackForce = 3f; // Force of the knockback

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    public void DealDamage(float damage, Vector3 attackerPosition){
        currentHealth -= damage;

        Vector2 knockbackDirection = transform.position - attackerPosition;
        knockbackDirection.Normalize(); // Make the length of the vector 1

        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
        Debug.Log("Enemy: " + currentHealth);
        
    }

}
