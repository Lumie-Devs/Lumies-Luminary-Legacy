using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public int maxHealth = 10;
    public int health;
    public float hitDuration = .2f;
    public float detectRange = 20f;
    public float neutralSpeed = 5f; // How fast the enemy moves along the path
    public float patrolDistance = 5f; // How far the enemy moves before going back

    protected float currentPatrolDistance = 0; // How far the enemy has currently patrolled
    protected int directionFactor = 1; // Whether the enemy is currently moving forwards (1) or backwards (-1)

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

            if (currentPatrolDistance >= patrolDistance)
            {
                directionFactor *= -1; // Reverse direction
                currentPatrolDistance = 0f; // Reset patrol distance
            }

            yield return null;
        }
    }

    protected virtual IEnumerator HostileMovement()
    {
        yield return null;
    }

    public virtual void Attacked()
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

    protected virtual IEnumerator Hit()
    {
        state = EnemyState.Hit;

        yield return new WaitForSeconds(hitDuration);

        state = EnemyState.Hostile;
    }
}