using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorageController : MonoBehaviour
{
    MovableObjectController stored;
    public void StoreObject(MovableObjectController target)
    {
        if (stored!=null)
        {
            stored.OnStore(false);
            stored = target;
        }
    }
    public void TryStoreObjectRay(Ray ray,float range)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, range) && hit.transform.TryGetComponent<MovableObjectController>(out MovableObjectController tgtObject))
        {
            StoreObject(tgtObject);
        }
    }
}
