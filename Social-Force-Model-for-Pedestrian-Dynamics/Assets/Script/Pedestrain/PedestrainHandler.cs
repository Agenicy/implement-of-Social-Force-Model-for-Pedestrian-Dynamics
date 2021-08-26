using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RepulsivePotential // Vab
{
    static float Vab0 = 2.1f;
    static float sigma = 0.3f;

    Vector3 v3_CenterA, v3_CenterB;

    public RepulsivePotential(Vector3 v3_CenterA, Vector3 v3_CenterB)
    {
        this.v3_CenterA = v3_CenterA;
        this.v3_CenterB = v3_CenterB;
    }

    /// <summary>
    /// 計算給定點在排斥場所受的斥力
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 CountForce(Vector3 point)
    {
        return CircularCount(v3_CenterA, point) + CircularCount(v3_CenterB, point);
    }

    public static Vector3 CircularCount(Vector3 v3_Center, Vector3 point)
    {
        // magnitude of force
        float mag = Vab0 * Mathf.Exp(-Vector3.Distance(v3_Center, point) / sigma);

        Vector3 dir = point - v3_Center;
        return dir * mag;

    }
}

public partial class PedestrainHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody_This;
    Transform t_Target;
    Collider collider_Target;
    Vector3 v3_Target;

    Vector3 v3_SpeedReality;
    static float sf_SpeedExpect = 1.34f;
    static float sf_SpeedMax = 1.34f * sf_SpeedExpect;

    static float sf_TDelta => Terrain.sf_TDelta;
    static float sf_RelaxTime => Terrain.sf_RelaxTime;
    static float sf_PedstrainVision => Terrain.sf_PedstrainVision;
    static float sf_PedstrainVision_OutsideRate => Terrain.sf_PedstrainVision_OutsideRate;

    RepulsivePotential repulsivePotential; 

    Vector3 GetDesiredDirection()
    {
        return (v3_Target - transform.position).normalized;
    }
    Vector3 GetTotalForce()
    {
        return AccelSelfSpeed() + AwayFromOtherPedestrain();
    }

    Vector3 AccelSelfSpeed()
    {
        Vector3 ret = (1 / sf_RelaxTime) * (sf_SpeedExpect * GetDesiredDirection() - v3_SpeedReality);
        return ret;
    }


    Vector3 AwayFromOtherPedestrain(PedestrainHandler beta)
    {
        Vector3 sBeta = beta.v3_SpeedReality * sf_TDelta;
        Vector3 rAB = v3_SpeedReality - beta.v3_SpeedReality;


        float b = (1 / 2f) * Vector3.Distance(rAB + (rAB - sBeta.magnitude * beta.GetDesiredDirection()), sBeta);

        Vector3 fAB = -beta.repulsivePotential.CountForce(b * rAB);
        return fAB;
    }

    //-------------------------------------------------------------------------//

    public void SetTarget(Transform t)
    {
        t_Target = t;
        collider_Target = t.GetComponent<MeshCollider>();
        StartCoroutine(IUpdate());
    }

    IEnumerator IUpdate()
    {
        v3_Target = collider_Target.ClosestPointOnBounds(transform.position);

        while (true)
        {
            repulsivePotential = new RepulsivePotential(transform.position, transform.position + v3_SpeedReality * sf_TDelta);

            rigidbody_This.AddForce(GetTotalForce());
            yield return new WaitForSeconds(1);
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