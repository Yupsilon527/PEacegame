using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectController : MonoBehaviour
{
    public void OnStore()
    {
        gameObject.SetActive(false);
    }
    public void OnRetrieve()
    {
        gameObject.SetActive(true);
    }
}
