using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

static class BordersRepulsivePotential
{
    static float Uab0 = 10f;
    static float R = 0.2f;

    public static Vector3 CountForce(Vector3 v3_Center, Vector3 point)
    {
        // magnitude of force
        float mag = Uab0 * Mathf.Pow(Mathf.Exp(1), -Vector3.Distance(v3_Center, point) / R);

        Vector3 dir = point - v3_Center;
        return dir * mag;

    }
}

public partial class PedestrainHandler : MonoBehaviour
{
    public Image image;
    [SerializeField] Rigidbody rigidbody_This;
    Collider collider_Target;
    Vector3 v3_Target;

    Vector3 v3_SpeedReality;
    static float sf_SpeedExpect = 1f;
    static float sf_SpeedMax = 1.34f * sf_SpeedExpect;

    static float sf_TDelta => TerrainHandler.sf_TDelta;
    static float sf_RelaxTime => TerrainHandler.sf_RelaxTime;
    static float sf_PedstrainVision => TerrainHandler.sf_PedstrainVision;
    static float sf_PedstrainVision_OutsideRate => TerrainHandler.sf_PedstrainVision_OutsideRate;

    static List<PedestrainHandler> list_ped = new List<PedestrainHandler>();

    [SerializeField] bool isDebug = false;

    // 吸引力(暫不實作)
    // Dictionary<object, float> list_int_ThingsThatAttracted = new List<int>();

    Vector3 GetDesiredDirection()
    {
        return (v3_Target - transform.position).normalized;
    }
    Vector3 GetTotalForce()
    {
        Vector3 selfAccel = AccelSelfSpeed();
        Vector3 pedAccel = Vector3.zero;
        foreach (var ped in list_ped)
        {
            if (ped != this)
                if (isWatching(ped.transform.position))
                    pedAccel += AwayFromOtherPedestrain(ped);
                else
                    pedAccel += AwayFromOtherPedestrain(ped) * sf_PedstrainVision_OutsideRate;
        }
        pedAccel = pedAccel.normalized;
        Debug.DrawLine(transform.position, transform.position + pedAccel, Color.yellow, sf_TDelta);
        Vector3 borderAccel = AwayFromBorder(BorderMarker.list_col_AllBorder);
        if (isDebug)
            Debug.Log($"{v3_SpeedReality} / {selfAccel} {pedAccel} + {borderAccel}");
        return selfAccel + pedAccel + borderAccel;
    }

    /// <summary>
    /// 自我提速
    /// </summary>
    Vector3 AccelSelfSpeed()
    {
        Vector3 ret = (1 / sf_RelaxTime) * (sf_SpeedExpect * GetDesiredDirection() - v3_SpeedReality);
        ret = Quaternion.Euler(0, UnityEngine.Random.Range(-.5f, .5f), 0) * ret;
        Debug.DrawLine(transform.position + v3_SpeedReality, transform.position + v3_SpeedReality + ret, Color.cyan, sf_TDelta);
        Debug.DrawLine(transform.position, transform.position + sf_SpeedExpect * GetDesiredDirection(), Color.magenta, sf_TDelta);
        return ret;
    }

    /// <summary>
    /// 遠離行人
    /// </summary>
    Vector3 AwayFromOtherPedestrain(PedestrainHandler beta)
    {
        Vector3 sBeta = beta.v3_SpeedReality * sf_TDelta;
        Vector3 rAB = transform.position - beta.transform.position;


        float b = (1 / 2f) * Mathf.Sqrt(Mathf.Pow(rAB.magnitude + (rAB - sBeta.magnitude * beta.GetDesiredDirection()).magnitude, 2) - Mathf.Pow(sBeta.magnitude, 2));

        float Vab0 = 2.1f;
        float sigma = 0.3f;
        // magnitude of force
        float mag = Vab0 * Mathf.Pow(Mathf.Exp(1), -b / sigma);
        Vector3 fAB = mag * ((transform.position - beta.transform.position) + (transform.position - (sBeta + beta.transform.position))) / 2f;
        
        if(fAB.magnitude > 1)
            Debug.Log(fAB, beta.gameObject);

        return fAB;
    }

    Vector3 AwayFromBorder(List<Collider> list_col_Border)
    {
        Vector3 pt_here = transform.position;

        var nearestBorder =
            list_col_Border.Aggregate(
                (a, b) => Vector3.Distance(a.ClosestPointOnBounds(pt_here), pt_here) < Vector3.Distance(b.ClosestPointOnBounds(pt_here), pt_here) ? a : b);
        var nearestPoint = nearestBorder.ClosestPointOnBounds(pt_here);
        var force = BordersRepulsivePotential.CountForce(nearestPoint, pt_here);

        Debug.DrawLine(transform.position, transform.position + force, Color.white, sf_TDelta);
        return force;
    }

    /// <summary>
    /// 被物件吸引
    /// </summary>
    Vector3 AttractBySomething()
    {
        return Vector3.zero;
    }

    bool isWatching(Vector3 point)
    {
        var angle = Mathf.Abs(Vector3.Angle(v3_SpeedReality, point));
        if (angle > 180)
            angle = 360 - angle;
        return angle < sf_PedstrainVision / 2;
    }

    //-------------------------------------------------------------------------//

    private void Awake()
    {
        if (list_ped.Count == 0)
            isDebug = true;
        list_ped.Add(this);
    }
    private void OnDestroy()
    {
        list_ped.Remove(this);
    }
    public void SetTarget(Transform t)
    {
        collider_Target = t.GetComponent<MeshCollider>();

        StartCoroutine(IUpdate());
    }

    IEnumerator IUpdate()
    {
        v3_Target = collider_Target.ClosestPointOnBounds(transform.position);
        v3_SpeedReality = rigidbody_This.velocity = GetDesiredDirection();
        yield return new WaitForFixedUpdate();
        while (true)
        {
            Vector3 old_next = transform.position + v3_SpeedReality;
            Debug.DrawLine(transform.position, old_next, Color.green, sf_TDelta);

            v3_Target = collider_Target.ClosestPointOnBounds(transform.position);

            Vector3 force = GetTotalForce();
            rigidbody_This.AddForce(force);

            yield return new WaitForFixedUpdate();

            v3_SpeedReality = rigidbody_This.velocity;
            if (v3_SpeedReality.magnitude > sf_SpeedMax)
                v3_SpeedReality = rigidbody_This.velocity *= sf_SpeedMax / v3_SpeedReality.magnitude;

            transform.LookAt(transform.position + v3_SpeedReality);

            Debug.DrawLine(transform.position, transform.position + v3_SpeedReality, Color.red, sf_TDelta);

            Debug.DrawLine(old_next, old_next + force, Color.blue, sf_TDelta);

            yield return new WaitForSeconds(sf_TDelta);
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