using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerTrigger : MonoBehaviour
{
    [Tooltip("How long the speed boost is on")]
    [SerializeField] private float speedBoostLength = 3f;
    [Tooltip("Speed boost multiplicator")]
    [SerializeField] private float speedBoostMultiplicator = 2f;
    [Tooltip("Jump multiplicator")]
    [SerializeField] private float jumpMultiplicator = 3f;
    [Tooltip("Initial player state (must be 'NotBoosted')")]
    [SerializeField] SpeedBoostState playerState = SpeedBoostState.NotBoosted;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;
    private float previousForwardSpeed;
    private float previousJumpSpeed;

    void Start()
    {
        rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>(); // Getting the player movement component
    }

    public enum SpeedBoostState
    {
        Boosted,
        NotBoosted
    }

    private void OnTriggerEnter(Collider other)
    {
        // Boosts the player's speed using the parameters defined in the editor and then restores the initial speed
        if (playerState == SpeedBoostState.NotBoosted && other.CompareTag("SpeedBoostTrigger"))
        {
            previousForwardSpeed = rigidbodyFirstPersonController.movementSettings.ForwardSpeed;
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed *= speedBoostMultiplicator;
            playerState = SpeedBoostState.Boosted;
            StartCoroutine("SpeedBoostTimer");
        }
        // Makes the player to do reinforced jump using the multiplicator defined in the editor
        else if (other.CompareTag("JumpTrigger"))
        {
            previousJumpSpeed = rigidbodyFirstPersonController.movementSettings.JumpForce;
            rigidbodyFirstPersonController.movementSettings.JumpForce *= jumpMultiplicator;
            rigidbodyFirstPersonController.m_Jump = true; // modifies the bool in the RigidbodyFirstPersonController script to make the player jumping
            StartCoroutine("JumpTimer");
        }
    }

    // Restores the initial speed after speedBoostLength seconds
    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(speedBoostLength);
        rigidbodyFirstPersonController.movementSettings.ForwardSpeed = previousForwardSpeed;
        playerState = SpeedBoostState.NotBoosted;
    }

    // Restores the initial jump force after 1 second (without the timer, jumps after the first jump will not happen)
    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(1f);
        rigidbodyFirstPersonController.movementSettings.JumpForce = previousJumpSpeed;
    }
}
