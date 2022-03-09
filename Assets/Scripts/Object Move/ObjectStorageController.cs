using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorageController : MonoBehaviour
{
    public float RayRange = 10;
    MovableObjectController stored;
    public void StoreObject(MovableObjectController target)
    {
        if (stored==null)
        {
            target.OnStore();
            stored = target;
        }
    }
    public void TryStoreObjectRay(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, RayRange) && hit.collider.TryGetComponent<MovableObjectController>(out MovableObjectController tgtObject))
        {
            StoreObject(tgtObject);

        }
    }
    public void TryRetrieveObject(Vector3 point)
    {
        Debug.Log("Try place object " + stored.name + " at point " + point);
        if (stored.CanBePlacedThere(point))
        {
            stored.OnRetrieve(point);
            stored = null;
        }
        else
        {
            Debug.Log("Invalid Location");
        }
    }
    protected void Update()
    {
        HandlePlayerInput();
    }
    protected void HandlePlayerInput()
    {
        if (stored == null)
        {
            if (Input.GetMouseButton(0))
        {
                Ray checkRay = new Ray(transform.position, transform.forward);
                TryStoreObjectRay(checkRay);
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray PlaceRay = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(PlaceRay, out RaycastHit hit, RayRange))
                {
                    TryRetrieveObject(hit.point);
                }
            }
        }
    }
}
