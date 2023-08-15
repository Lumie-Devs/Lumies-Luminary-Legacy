using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CustomRigidBody : MonoBehaviour
{
    public Vector2 velocity;
    public float mass = 1f;
    private PhysicsManager physicsManager;
    private Collider2D myCollider;

    private void Start()
    {
        physicsManager = FindObjectOfType<PhysicsManager>();
        myCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        velocity = physicsManager.ComputeVelocity(velocity, 0, Time.deltaTime);
        velocity = physicsManager.ComputeGravity(velocity);

        RaycastHit2D[] hits = new RaycastHit2D[1];

        Vector2 newPosition = transform.position + (Vector3)velocity * Time.deltaTime;
        int numHits = myCollider.Cast(velocity.normalized, hits, velocity.magnitude * Time.deltaTime);

        if (numHits > 0)
        {
            HandleCollision();
        }

        transform.position = newPosition;
    }

    private void HandleCollision()
    {
        velocity.y = 0;
    }
}