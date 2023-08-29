using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    public static PlayerInfo Instance;
    public static Transform playerTransform;

    [SerializeField] private PlayerMovement playerMovement;

    public int maxHealth = 10;
    public int health;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            playerTransform = transform;
        } else {
            Destroy(gameObject);
        }

        health = maxHealth;
    }

    public void Attacked(int damage, float hitDirection)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        } else 
        {
            playerMovement.Hit(hitDirection);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}