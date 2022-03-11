using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSwitcher : MonoBehaviour
{
    // Enables cursor when the scene is loaded
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
