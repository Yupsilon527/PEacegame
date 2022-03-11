using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovableObjectController : MonoBehaviour
{
    public enum CastType
    {
        none,
        capsule,
        sphere,
        box
    }
    public Vector3 CenterDelta = Vector3.zero;
    public bool RequiresGround = true;
    public CastType mesh = CastType.none;

    public void OnStore()
    {
        if (AnimCoroutine == null)
        StartCoroutine(SwallowWhole(.25f));
    }
    public void OnRetrieve(Vector3 pos)
    {
        transform.position = pos - CenterDelta;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.SetActive(true);
        if (AnimCoroutine == null)
            StartCoroutine(SpitOut(.25f));
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.velocity *= 0;
            rigidbody.angularVelocity *= 0;
        }
    }

    public bool CanBePlacedThere(Vector3 point)
    {
        Ray castRay = new Ray(point - CenterDelta, Vector3.zero);
        switch (mesh)
        {
            case CastType.none:
                return true;
            case CastType.capsule:
                if (TryGetComponent(out CapsuleCollider capsuleC))
                {
                    return Physics.CapsuleCastAll(castRay.origin + capsuleC.height * capsuleC.transform.up, castRay.origin - capsuleC.height * capsuleC.transform.up, capsuleC.radius, Vector3.zero).Length==0;
                }
                break;
            case CastType.box:
                if (TryGetComponent(out BoxCollider boxC))
                {
                    return Physics.BoxCastAll(castRay.origin, transform.localScale, Vector3.down).Length == 0;
                }
                break;
            case CastType.sphere:
                if (TryGetComponent(out SphereCollider sphereC))
                {
                    return Physics.SphereCastAll(castRay,sphereC.radius,0).Length == 0;
                }
                break;
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + CenterDelta, .1f);

    }
    Coroutine AnimCoroutine;
    public IEnumerator SwallowWhole(float time)
    {
        Collider c = GetComponent<Collider>();
        c.enabled = false;
        Vector3 position = transform.position;
        Vector3 scale = transform.localScale;
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }

            float StartTime = Time.time + time;
        while (StartTime > Time.time)
        {
            transform.position -= (position - (Camera.main.transform.position + Vector3.down)) * Time.deltaTime / time;
            transform.localScale -= scale * Time.deltaTime / time  ;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        c.enabled = true;
        transform.position = position;
        transform.localScale = scale;
        AnimCoroutine = null;
        if (TryGetComponent(out  rigidbody))
        {
            rigidbody.isKinematic = false;
        }
    }
    public IEnumerator SpitOut(float time)
    {
        Collider c = GetComponent<Collider>();
        c.enabled = false;
        Vector3 position = transform.position;
        Vector3 scale = transform.localScale;
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }


        Vector3 origPos = Camera.main.transform.position + Vector3.down;
       transform.position = origPos;
        transform.localScale = Vector3.zero;

        float StartTime = Time.time + time;
        while (StartTime > Time.time)
        {
            transform.position += (position - origPos) * Time.deltaTime / time;
            transform.localScale += scale * Time.deltaTime / time;
            yield return new WaitForEndOfFrame();
        }
        c.enabled = true;
        transform.position = position;
        transform.localScale = scale;
        AnimCoroutine = null;
        if (TryGetComponent(out rigidbody))
        {
            rigidbody.isKinematic = false;
        }
    }
}
