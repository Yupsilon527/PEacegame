using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorageController : MonoBehaviour
{
    public float RayRange = 10;
    public BlueprintController BlueprintIndicator;
    MovableObjectController stored;

    private void Start()
    {
        if (BlueprintIndicator != null)
            BlueprintIndicator.ClearMesh();
    }
    public void StoreObject(MovableObjectController target)
    {
        if (stored==null)
        {
            target.OnStore();
            if (BlueprintIndicator != null)
                BlueprintIndicator.LoadObject(target.gameObject);
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
            if (Input.GetMouseButtonUp(0))
        {
                Ray checkRay = new Ray(transform.position, transform.forward);
                TryStoreObjectRay(checkRay);
            }
        }
        else
        {
            Ray PlaceRay = new Ray(transform.position, transform.forward);
            if (Input.GetMouseButton(0))
            {
                if (BlueprintIndicator != null)
                {
                    if (Physics.Raycast(PlaceRay, out RaycastHit hit, RayRange))
                    {
                        BlueprintIndicator.ChangeState(stored.CanBePlacedThere(hit.point));
                        BlueprintIndicator.transform.position = hit.point - stored.CenterDelta;
                        BlueprintIndicator.gameObject.SetActive(true);
                    }
                    else
                    {
                        Vector3 point = PlaceRay.GetPoint(RayRange);
                        BlueprintIndicator.ChangeState(!stored.RequiresGround && stored.CanBePlacedThere(hit.point));
                        BlueprintIndicator.transform.position = point - stored.CenterDelta;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(PlaceRay, out RaycastHit hit, RayRange))
                {
                    TryRetrieveObject(hit.point);
                }
                else if (!stored.RequiresGround)
                {
                    TryRetrieveObject(PlaceRay.GetPoint(RayRange));
                }
                if (BlueprintIndicator != null)
                    BlueprintIndicator.gameObject.SetActive(false);
            }
        }
    }
}
