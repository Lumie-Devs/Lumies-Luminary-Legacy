using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    [SerializeField] private PlayerMovement playerMovement;
    private Animator anim;
    [SerializeField] private BoxCollider2D hammerRange;

    public float comboGap = .7f;
    public float comboMovementSpeed = 5f;
    public float comboCooldown = .3f;
    public int comboDamage = 4;
    public float hammerJumpForce = 15f;
    public float hammerSmashSpeed = 15f;
    public float throwDistance = 10f;
    public float throwCooldown = 2f; // Cooldown duration in seconds
    private float nextThrowTime = 0f; // The next time the player can throw
    private bool canThrow = true;
    private bool canCombo = true;
    public int airComboCount, groundComboCount = 0;

    public List<GameObject> enemiesInRange = new List<GameObject>();
    public bool wallInRange = false;

    private Coroutine combo;
    private bool hammerJump = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            wallInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            wallInRange = false;
        }
    }

    public void DoAction(float inputY, bool isGrounded)
    {
        if (!canCombo) return;

        if (isGrounded) GroundCombos(inputY);

        else AirCombos(inputY);

        StartCoroutine(ComboCooldown());
    }

    private void GroundCombos(float inputY)
    {
        if (inputY > 0) UppercutSwing();

        else if (inputY < 0) HammerPound();

        else HammerSwing();
    }

    private void AirCombos(float inputY)
    {
        if (inputY > 0) HammerJump();

        else if (inputY < 0) HammerSmash();

        else AerialHammerSwing();
    }

    private void HammerSwing()
    {
        switch (groundComboCount){
            case 0:
            combo = StartCoroutine(Comboing(0));
            break;
            case 1:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing(1));
            break;
            case 2:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing(2));
            break;
            default:
            return;
        }

        groundComboCount++;
        anim.SetInteger("Combo", groundComboCount);
    }

    private void UppercutSwing()
    {
        anim.SetTrigger("Uppercut");

        // Get all colliders that intersect with your BoxCollider2D
        Collider2D[] collidersInRange = EnemyCollidersInRange();

        foreach (Collider2D col in collidersInRange)
        {
            if (col.gameObject.CompareTag("Enemy"))
            {
                col.GetComponent<Enemy>().LaunchedUp(comboDamage, transform.position.x);
            }
        }
    }

    private void HammerPound()
    {
        Debug.Log("Hammer Pound");
    }

    private void AerialHammerSwing()
    {
        switch (airComboCount){
            case 0:
            combo = StartCoroutine(Comboing(0));
            break;
            case 1:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing(1));
            break;
            case 2:
            StopCoroutine(combo);
            playerMovement.StopComboing();
            HammerSmash();
            break;
            default:
            return;
        }

        airComboCount++;
        anim.SetInteger("Air Combo", airComboCount);
    }

    private void HammerJump()
    {
        if (hammerJump) return;

        playerMovement.ApplyJump(hammerJumpForce);
        hammerJump = true;
    }

    private void HammerSmash()
    {
        playerMovement.Smashing(hammerSmashSpeed);
    }

    public void ResetAirAttacks()
    {
        hammerJump = false;
        airComboCount = 0;
        canThrow = true;
    }

    public void ThrowHammer()
    {
        if (Time.time < nextThrowTime || !canThrow) return;

        // Determine the direction of the wall relative to the player
        Vector2 throwDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Cast a ray from the player in the direction of the wall
        RaycastHit2D hit = Physics2D.Raycast(transform.position, throwDirection, throwDistance, LayerMask.GetMask("Environment"));

        Vector2 rayEndpoint = (Vector2)transform.position + throwDirection * throwDistance;

        // The point where the hammer would contact the wall
        Vector2 contactPoint = hit.collider != null ? hit.point : rayEndpoint;

        // Define the specific distance you want the player to be from the wall
        float desiredDistanceFromWall = 1.5f;

        // Calculate the desired player position based on the contact point and specific distance
        Vector2 desiredPosition = contactPoint - throwDirection * desiredDistanceFromWall;

        // Draw lines for debugging
        Debug.DrawLine(transform.position, contactPoint, Color.red, 2f);
        Debug.DrawLine(contactPoint, desiredPosition, Color.blue, 2f);

        playerMovement.HammerThrow();

        // Set the player's position
        transform.position = desiredPosition;

        // If the ray hits the wall
        if (hit.collider != null)
        {
            playerMovement.Hang();
        }

        nextThrowTime = Time.time + throwCooldown;
        canThrow = false;
    }

    private Collider2D[] EnemyCollidersInRange()
    {
        // Get all colliders that intersect with your BoxCollider2D
        return Physics2D.OverlapBoxAll(hammerRange.transform.position, hammerRange.size, 0);
    }

    private IEnumerator Comboing(int comboHit)
    {
        // Get all colliders that intersect with your BoxCollider2D
        Collider2D[] collidersInRange = EnemyCollidersInRange();

        if (comboHit < 2)
        {
            foreach (Collider2D col in collidersInRange)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.GetComponent<Enemy>().Attacked(comboDamage, transform.position.x);
                }
            }
        } 
        else 
        {
            foreach (Collider2D col in collidersInRange)
            {
                if (col.gameObject.CompareTag("Enemy"))
                {
                    col.GetComponent<Enemy>().LaunchedBack(comboDamage, transform.position.x);
                }
            }
        }

        

        if (wallInRange)
        {
            
        }
        else 
        {

            playerMovement.Comboing(comboMovementSpeed);

            yield return new WaitForSeconds(comboGap);

            playerMovement.StopComboing();
        }

        
        groundComboCount = 0;
        anim.SetInteger("Combo", 0);
        anim.SetInteger("Air Combo", 0);

        yield return null;
    }

    void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = Color.red;

        // Retrieve the properties of your BoxCollider2D
        Vector2 boxPosition = hammerRange.transform.position;
        Vector2 boxSize = hammerRange.size;

        // Draw a wire cube at the position and dimensions of your BoxCollider2D
        Gizmos.DrawWireCube(boxPosition, boxSize);
    }

    private IEnumerator ComboCooldown()
    {
        canCombo = false; // Disable combo actions
        yield return new WaitForSeconds(comboCooldown); // Wait for cooldown time
        canCombo = true; // Re-enable combo actions
    }
}