using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrainHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody_This;
    Transform t_Target;
    Collider collider_Target;
    Vector3 v3_Target;

    float hz = 60;
    public void SetTarget(Transform t)
    {
        t_Target = t;
        collider_Target = t.GetComponent<MeshCollider>();
        StartCoroutine(IUpdate());
    }


    IEnumerator IUpdate()
    {
        v3_Target = collider_Target.ClosestPointOnBounds(transform.position);

        while(true)
        {
            rigidbody_This.AddForce(2*(v3_Target - transform.position).normalized);
            yield return new WaitForSeconds(1/ hz);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // End
        if (collision.collider.gameObject.name == "(Plane)EndSide")
        {
            StopAllCoroutines();
            ModelPool.Recycle(gameObject);
        }
    }
}
