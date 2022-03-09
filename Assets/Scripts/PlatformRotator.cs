using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotator : MonoBehaviour
{
    [SerializeField] Vector3 rotationAxis = new Vector3(0, 0, 1); // rotation axis
    [SerializeField] float rotationSpeed = 10f; // rotation speed

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime); // rotating an object smoothly around the axis
    }
}
