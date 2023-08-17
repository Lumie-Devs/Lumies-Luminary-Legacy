using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    [SerializeField] private PlayerMovement playerMovement;
    private Rigidbody2D rb;

    public float comboGap = .5f;
    public float comboMovementSpeed = 5f;
    public float hammerJumpForce = 15f;
    public float hammerSmashSpeed = 15f;
    public int airComboCount, groundComboCount = 0;

    private Coroutine combo;
    private bool hammerJump = false;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void DoAction(float inputY, bool isGrounded)
    {
        if (isGrounded) GroundCombos(inputY);

        else AirCombos(inputY);
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
            combo = StartCoroutine(Comboing());
            break;
            case 1:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing());
            break;
            case 2:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing());
            break;
            default:
            return;
        }

        groundComboCount++;
    }

    private void UppercutSwing()
    {
        Debug.Log("UpperCut Swing");
    }

    private void HammerPound()
    {
        Debug.Log("Hammer Pound");
    }

    private void AerialHammerSwing()
    {
        switch (airComboCount){
            case 0:
            combo = StartCoroutine(Comboing());
            break;
            case 1:
            StopCoroutine(combo);
            combo = StartCoroutine(Comboing());
            break;
            case 2:
            StopCoroutine(combo);
            playerMovement.StopComboing();
            HammerSmash();
            airComboCount++;
            break;
            default:
            return;
        }

        airComboCount++;
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
    }

    private IEnumerator Comboing()
    {
        playerMovement.Comboing(comboMovementSpeed);

        yield return new WaitForSeconds(comboGap);

        playerMovement.StopComboing();
        groundComboCount = 0;
    }
}