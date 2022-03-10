using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class PlayerTrigger : MonoBehaviour
{
    [Tooltip("How long the speed boost is on")]
    [SerializeField] private float speedBoostLength = 3f;
    [Tooltip("Speed boost multiplier")]
    [SerializeField] private float speedBoostMultiplier = 2f;
    [Tooltip("Jump multiplier")]
    [SerializeField] private float jumpMultiplier = 3f;
    [Tooltip("Initial player state (must be 'NotBoosted')")]
    [SerializeField] SpeedBoostState playerState = SpeedBoostState.NotBoosted;
    [Tooltip("Add here a scene that will be loaded after the death")]
    [SerializeField] private Object scene;
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -20f;
    private Rigidbody rigidBody;
    private RigidbodyFirstPersonController rigidbodyFirstPersonController;
    private float previousForwardSpeed;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidbodyFirstPersonController = GetComponent<RigidbodyFirstPersonController>(); // Getting the player movement component
    }

    private void FixedUpdate()
    {
        if (transform.position.y < deathElevation)
            SceneManager.LoadScene($"{scene.name}");
    }

    public enum SpeedBoostState
    {
        Boosted,
        NotBoosted
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Boosts the player's speed using the parameters defined in the editor and then restores the initial speed
        if (playerState == SpeedBoostState.NotBoosted && collision.collider.CompareTag("SpeedBoostTrigger"))
        {
            previousForwardSpeed = rigidbodyFirstPersonController.movementSettings.ForwardSpeed;
            rigidbodyFirstPersonController.movementSettings.ForwardSpeed *= speedBoostMultiplier;
            playerState = SpeedBoostState.Boosted;
            StartCoroutine("SpeedBoostTimer");
        }
        // Makes the player to do a reinforced jump using the multiplicator defined in the editor
        // Speed boost is disabled after collision
        else if (collision.collider.CompareTag("JumpTrigger"))
        {
            if (playerState == SpeedBoostState.Boosted)
                rigidbodyFirstPersonController.movementSettings.ForwardSpeed = previousForwardSpeed;
            rigidbodyFirstPersonController.m_Flying = true;
            rigidBody.AddForce(collision.collider.transform.transform.up.normalized * rigidbodyFirstPersonController.movementSettings.JumpForce * jumpMultiplier, ForceMode.Impulse);
        }
    }

    // Restores the initial speed after speedBoostLength seconds
    IEnumerator SpeedBoostTimer()
    {
        yield return new WaitForSeconds(speedBoostLength);
        rigidbodyFirstPersonController.movementSettings.ForwardSpeed = previousForwardSpeed;
        playerState = SpeedBoostState.NotBoosted;
    }
}
