using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDissolver : MonoBehaviour
{
    [SerializeField] GameObject platform; // reference to the platform
    [SerializeField] float interval = 2f; // waiting for 2 seconds
    [SerializeField] float pause = 2f; // waiting for 2 seconds

    IEnumerator Start()
    {
        while (true) // infinite loop
        {
            yield return new WaitForSeconds(interval); // waiting time
            platform.SetActive(!platform.activeSelf); // if the object was active, set as inactive, and vice versa
            yield return new WaitForSeconds(pause); // waiting time
        }

    }
    // important note: don't attach the script to the object you're disabling
}
