using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int maxHealth = 10;
    public int health;
    public float hitDuration = .2f;
    public float knockbackDistance = .2f;
    public float detectRange = 20f;
    public float neutralSpeed = 5f; // How fast the enemy moves along the path
    public float hostileSpeed = 7f;
    public float patrolDistance = 5f; // How far the enemy moves before going back
    public float wallCheckDistance = 1f;

    protected float currentPatrolDistance = 0; // How far the enemy has currently patrolled
    protected float directionFactor = 1; // Whether the enemy is currently moving forwards (1) or backwards (-1)

    protected Transform player;
    protected Coroutine currentRoutine;
    public EnemyState state = EnemyState.Neutral;

    public enum EnemyState
    {
        Neutral,
        Hostile,
        Hit,
        Stunned,
        Downed
    }

    protected virtual void Start() {
        health = maxHealth;
        player = PlayerInfo.Instance;

        currentRoutine = StartCoroutine(NeutralMovement());
    }

    protected virtual void Update() {
        
    }

    protected virtual IEnumerator NeutralMovement() 
    {
        StartCoroutine(SearchForPlayer());

        while (true)
        {
            Vector3 moveVector = new Vector3(neutralSpeed * directionFactor, 0, 0);

            transform.position += moveVector * Time.deltaTime;
            currentPatrolDistance += neutralSpeed * Time.deltaTime;

            // Get the layer number for "Environment"
            int layerNumber = LayerMask.NameToLayer("Environment");

            // Create a layer mask that includes only the "Environment" layer
            int layerMask = 1 << layerNumber;

            // Raycast in the move direction to check for barriers
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVector.normalized, wallCheckDistance, layerMask);

            if (currentPatrolDistance >= patrolDistance || hit.collider != null)
            {
                directionFactor *= -1; // Reverse direction
                currentPatrolDistance = 0f; // Reset patrol distance
            }

            yield return null;
        }
    }

    protected virtual IEnumerator HostileMovement()
    {
        while (true)
        {
            Vector3 moveVector = new Vector3(hostileSpeed * GetPlayerDirection(), 0, 0);

            transform.position += moveVector * Time.deltaTime;

            // Get the layer number for "Environment"
            int layerNumber = LayerMask.NameToLayer("Environment");

            // Create a layer mask that includes only the "Environment" layer
            int layerMask = 1 << layerNumber;

            // Raycast in the move direction to check for barriers
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVector.normalized, wallCheckDistance, layerMask);

            if (hit.collider != null) break;

            yield return null;
        }
    }

    public virtual void Attacked(float hitDirection)
    {
        StopCoroutine(currentRoutine);

        switch(state)
        {
            case EnemyState.Stunned:
            break;

            case EnemyState.Downed:
            break;

            default:
            currentRoutine = StartCoroutine(Hit());
            
            break;
        }
    }

    protected virtual IEnumerator SearchForPlayer()
    {
        while (state == EnemyState.Neutral)
        {
            yield return new WaitForSeconds(.5f);

            if (PlayerSearch())
            {
                state = EnemyState.Hostile;
                StopCoroutine(currentRoutine);
                currentRoutine = StartCoroutine(HostileMovement());
            }
        }
    }

    protected virtual bool PlayerSearch()
    {
        return Vector3.Distance(player.position, transform.position) < detectRange;
    }

    protected virtual int GetPlayerDirection()
    {
        return GetDirection(player.position.x);
    }

    protected virtual int GetDirection(float direction)
    {
        int d = 0;

        if (direction > transform.position.x) d = 1;

        else if (direction < transform.position.x) d = -1;

        return d;
    }

    protected virtual IEnumerator Hit()
    {
        state = EnemyState.Hit;

        yield return new WaitForSeconds(hitDuration);

        state = EnemyState.Hostile;
        currentRoutine = StartCoroutine(HostileMovement());
    }

    protected virtual IEnumerator Launch(float hitDirection)
    {
        yield return null;
    }
}