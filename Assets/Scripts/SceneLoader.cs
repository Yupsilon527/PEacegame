using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //[Tooltip("Add here a scene that will be loaded after the death")]
    //[SerializeField] private Object scene;
    [Tooltip("Elevation on which the player will die")]
    [SerializeField] private float deathElevation = -10f;
    private void FixedUpdate()
    {
        if (transform.position.y < deathElevation)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Input.GetKey(KeyCode.M))
        {
            SceneManager.LoadScene(0);            
        }
    }
}
