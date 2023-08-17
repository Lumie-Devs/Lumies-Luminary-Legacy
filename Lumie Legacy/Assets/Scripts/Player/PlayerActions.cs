using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    [SerializeField] private PlayerMovement playerMovement;

    private void Start() {
        
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
        Debug.Log("Hammer Swing");
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
        Debug.Log("Aerial Hammer Swing");
    }

    private void HammerJump()
    {
        Debug.Log("Hammer Jump");
    }

    private void HammerSmash()
    {
        Debug.Log("Hammer Smash");
    }
}