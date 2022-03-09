using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObjectController : MonoBehaviour
{
    public Vector3 CenterDelta = Vector3.zero;
    public void OnStore()
    {
        gameObject.SetActive(false);
    }
    public void OnRetrieve(Vector3 pos)
    {
        transform.position = pos - CenterDelta;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.SetActive(true);
    }
    public bool CanBePlacedThere(Vector3 point)
    {
        Ray castRay = new Ray(point - CenterDelta, Vector3.down);
        return !GetComponent<Collider>().Raycast(castRay, out RaycastHit hitInfo, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + CenterDelta,.1f);
    }
}
