using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int maxHealth = 10;
    public int health;
    public int damage = 2;
    public float hitStunDuration = .2f;
    public float hitRange = 1f;
    public float attackSpeed = .3f;
    public float knockbackDistance = 2f;
    public float knockbackHeight = 1f;
    public float knockbackSpeed = 2f;
    public float detectRange = 20f;
    public float neutralSpeed = 5f; // How fast the enemy moves along the path
    public float hostileSpeed = 7f;
    public float patrolDistance = 5f; // How far the enemy moves before going back
    public float wallCheckDistance = 1f;

    protected float currentPatrolDistance = 0; // How far the enemy has currently patrolled
    protected float directionFactor = 1; // Whether the enemy is currently moving forwards (1) or backwards (-1)
    protected bool isAttacking = false;

    protected Transform playerTransform;
    protected PlayerInfo player;
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
        playerTransform = PlayerInfo.playerTransform;

        currentRoutine = StartCoroutine(NeutralMovement());
    }

    protected virtual void Update() {
        
    }

    protected virtual IEnumerator NeutralMovement() 
    {
        StartCoroutine(SearchForPlayer());

        // Get the layer number for "Environment"
        int layerNumber = LayerMask.NameToLayer("Environment");

        // Create a layer mask that includes only the "Environment" layer
        int layerMask = 1 << layerNumber;

        while (true)
        {
            Vector3 moveVector = new (neutralSpeed * directionFactor, 0, 0);

            transform.localScale = new (directionFactor, 1);

            transform.position += moveVector * Time.deltaTime;
            currentPatrolDistance += neutralSpeed * Time.deltaTime;

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
        // Get the layer number for "Environment"
        int layerNumber = LayerMask.NameToLayer("Environment");

        // Create a layer mask that includes only the "Environment" layer
        int layerMask = 1 << layerNumber;

        while (true)
        {
            while (!isAttacking)
            {
                int playerDirection = GetPlayerDirection();

                Vector3 moveVector = new(hostileSpeed * playerDirection, 0, 0);

                transform.localScale = new(-playerDirection, 1);

                // Raycast in the move direction to check for barriers
                RaycastHit2D hit = Physics2D.Raycast(transform.position, moveVector.normalized, wallCheckDistance, layerMask);

                if (hit.collider != null) isAttacking = true;

                transform.position += moveVector * Time.deltaTime;

                if (Vector3.Distance(playerTransform.position, transform.position) <= hitRange) Attack();

                yield return null;
            }

            isAttacking = false;

            yield return new WaitForSeconds(attackSpeed);
        }
    }

    public virtual void Attack()
    {
        player.Attacked(damage, transform.position.x);

        isAttacking = true;
    }

    public virtual void Attacked(int damage, float hitDirection)
    {
        StopCoroutine(currentRoutine);

        switch(state)
        {
            case EnemyState.Stunned:
            break;

            case EnemyState.Downed:
            break;

            default:
            currentRoutine = StartCoroutine(NeutralHit(damage));
            
            break;
        }
    }

    public virtual void LaunchedUp(int damage, float hitDirection)
    {
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(KnockedBack(damage, -GetDirection(hitDirection)));
    }

    public virtual void LaunchedBack(int damage, float hitDirection)
    {
        StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(KnockedBack(damage, -GetDirection(hitDirection)));
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
        return Vector3.Distance(playerTransform.position, transform.position) <= detectRange;
    }

    protected virtual int GetPlayerDirection()
    {
        return GetDirection(playerTransform.position.x);
    }

    protected virtual int GetDirection(float direction)
    {
        return Math.Sign(direction - transform.position.x);
    }

    protected virtual void Hit(int damage)
    {
        health -= damage;

        if (health <= 0) Death();

        state = EnemyState.Hit;
    }

    protected virtual IEnumerator NeutralHit(int damage)
    {
        Hit(damage);

        yield return new WaitForSeconds(hitStunDuration);

        state = EnemyState.Hostile;
        currentRoutine = StartCoroutine(HostileMovement());
    }

    protected virtual IEnumerator KnockedBack(int damage, int hitDirection)
    {
        Hit(damage);

        float elapsed = 0;

        // Get the layer number for "Environment"
        int layerNumber = LayerMask.NameToLayer("Environment");
        
        // Create a layer mask that includes only the "Environment" layer
        int layerMask = 1 << layerNumber;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + new Vector3(knockbackDistance * hitDirection, 0, 0);
        Vector3 controlPoint = initialPosition + new Vector3(knockbackDistance * hitDirection / 2, knockbackHeight, 0);

        // Loop while moving the character
        while (true)
        {
            elapsed += Time.deltaTime * knockbackSpeed;

            float t = elapsed / Vector3.Distance(initialPosition, targetPosition);

            // Quadratic Bezier curve formula to move in an arc
            Vector3 newPosition = Mathf.Pow(1 - t, 2) * initialPosition + 2 * (1 - t) * t * controlPoint + Mathf.Pow(t, 2) * targetPosition;

            Vector2 diagonalDirection = new(hitDirection, Math.Sign(.5f - t));

            // Raycast in the move direction to check for barriers
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position, new Vector2(diagonalDirection.x, 0), wallCheckDistance, layerMask);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(0, diagonalDirection.y), wallCheckDistance, layerMask);
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position, diagonalDirection, wallCheckDistance, layerMask);
            
            // If raycast hits a wall, stop
            if (hit1.collider != null || hit2.collider != null || hit3.collider != null)
            {
                break;
            }
            
            // Move the character to the new position
            transform.position = newPosition;
            
            yield return null;
        }

        // Transition back to the Hostile state or any other appropriate state
        state = EnemyState.Hostile;
        currentRoutine = StartCoroutine(HostileMovement());
    }

    protected virtual void Death()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}