using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleTracker : MonoBehaviour
{
    public List<Text> fullWord;

    private void Start()
    {
        // Disables all UI letters
        foreach (Text letter in fullWord)
            letter.enabled = false;
    }
}
