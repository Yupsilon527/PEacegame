using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public void LoadNewLevel(string LevelName)
    {
        SceneManager.LoadSceneAsync(LevelName);
    }
}
