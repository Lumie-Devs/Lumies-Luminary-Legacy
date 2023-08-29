using UnityEngine;

public class PlayerInfo : MonoBehaviour {
    public static PlayerInfo Instance;
    public static Transform playerTransform;

    [SerializeField] private PlayerMovement playerMovement;

    public int maxHealth = 10;
    public int health;
    public float distanceAboveGround = .35f;

    private void Awake() {
        Instance = this;
        playerTransform = transform;
    }

    private void Start() {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        health = maxHealth;

        Vector2 spawnLocation = GameManager.Instance.GetPlayerData().spawnLocation;

        transform.position = new(spawnLocation.x, spawnLocation.y + distanceAboveGround);
    }

    public void SetActiveInput(bool b)
    {
        playerMovement.enabled = b;
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