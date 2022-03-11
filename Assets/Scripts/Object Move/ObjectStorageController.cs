using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStorageController : MonoBehaviour
{
    public bool EnableOutline = true;
    public float RetrieveRange = 20;
    public float PlaceRange = 6;
    public BlueprintController BlueprintIndicator;
    MovableObjectController stored;
    public AudioSource source;
    public AudioClip StoreSound;
    public AudioClip RetrieveSound;
    private void Start()
    {
        if (BlueprintIndicator != null)
        {
            BlueprintIndicator.ClearMesh();
            BlueprintIndicator.gameObject.SetActive(false);
        }
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
        if (Physics.Raycast(ray, out RaycastHit hit, RetrieveRange) && hit.transform.tag == "CanBeStored" && hit.collider.TryGetComponent<MovableObjectController>(out MovableObjectController tgtObject))
        {
            StoreObject(tgtObject);
            source.PlayOneShot(StoreSound);
        }
    }
    public void TryRetrieveObject(Vector3 point)
    {
        Debug.Log("Try place object " + stored.name + " at point " + point);
        if (stored.CanBePlacedThere(point))
        {
            stored.OnRetrieve(point);
            stored = null;
            if (source != null)
                source.PlayOneShot(RetrieveSound);
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
                ClearOutline();
            }
            else if (EnableOutline)
            {
                Ray checkRay = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(checkRay, out RaycastHit hit, RetrieveRange) && hit.transform.tag == "CanBeStored")
                {
                    OutlineObject(hit.transform);
                }
                else
                {
                    ClearOutline();
                }
            }
        }
        else
        {
            Ray PlaceRay = new Ray(transform.position, transform.forward);
            if (Input.GetMouseButton(0))
            {
                if (BlueprintIndicator != null)
                {
                    if (Physics.Raycast(PlaceRay, out RaycastHit hit, PlaceRange))
                    {
                        BlueprintIndicator.ChangeState(stored.CanBePlacedThere(hit.point));
                        BlueprintIndicator.transform.position = hit.point - stored.CenterDelta;
                    }
                    else
                    {
                        Vector3 point = PlaceRay.GetPoint(PlaceRange);
                        BlueprintIndicator.ChangeState(!stored.RequiresGround && stored.CanBePlacedThere(hit.point));
                        BlueprintIndicator.transform.position = point - stored.CenterDelta;
                    }
                    BlueprintIndicator.gameObject.SetActive(true);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(PlaceRay, out RaycastHit hit, PlaceRange))
                {
                    TryRetrieveObject(hit.point);
                }
                else if (!stored.RequiresGround)
                {
                    TryRetrieveObject(PlaceRay.GetPoint(PlaceRange));
                }
                if (BlueprintIndicator != null)
                    BlueprintIndicator.gameObject.SetActive(false);
            }
        }
    }
    Outline outlinedObject;
    public void OutlineObject(Transform target)
    {
        Outline outLine = target.GetComponentInChildren<Outline>();
                    if (outLine!=null)
        {
            if (outlinedObject == outLine)
                return;
            ClearOutline();
            outlinedObject = outLine;
            outLine.enabled = true;
        }
    }
    public void ClearOutline()
    {
        if (outlinedObject != null)
            outlinedObject.enabled = false;
        outlinedObject = null;
    }
}
