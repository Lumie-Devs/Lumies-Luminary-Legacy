using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float gravity = -9.8f;

    public Vector2 ComputeVelocity(Vector2 initialVelocity, float acceleration, float deltaTime)
    {
        float deltaVelocity = acceleration * deltaTime;
        return new Vector2(initialVelocity.x, initialVelocity.y + deltaVelocity);
    }

    public Vector2 ComputeGravity(Vector2 initialVelocity)
    {
        return initialVelocity + new Vector2(0, gravity) * Time.deltaTime;
    }
}
