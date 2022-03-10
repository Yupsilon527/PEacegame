using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Tooltip("Rotation vector")]
    public Vector3 rotationVector = new Vector3(0f, 1f ,0f);
    [Tooltip("Rotation speed")]
    [SerializeField] private float rotationSpeed = 50f;
    [Tooltip("Movement vector")]
    [SerializeField] Vector3 movementVector = new Vector3(0f, 0.4f, 0f);
    [Tooltip("Movement period")]
    [SerializeField] float period = 5f;
    Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;
    }

    void Update()
    {
        RotateObject();
        OscillateObject();
    }

    private void RotateObject()
    {
        transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
    }

    private void OscillateObject()
    {
        if (period <= Mathf.Epsilon) { return; } // protect against zero period 
                                                 // set movement factor
        float cycles = Time.time / period; // grows contimually from 0
        const float tau = Mathf.PI * 2; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to 1
        float movementFactor = rawSinWave / 2f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }

    // Destroys the gameObject if collided with a player object
    // Add the gameObject name to the list attached to a player object
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            List <string> letters = other.GetComponent<CollectibleTracker>().letters;
            letters.Insert(letters.Count, gameObject.name);
            Destroy(gameObject);
        }
    }
}