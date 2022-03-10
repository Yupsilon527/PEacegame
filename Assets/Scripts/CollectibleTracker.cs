using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleTracker : MonoBehaviour
{
    public List<GameObject> fullWord;

    private void Start()
    {
        // Disables all UI letters
        foreach (GameObject letter in fullWord)
            letter.SetActive(false);
    }
}
