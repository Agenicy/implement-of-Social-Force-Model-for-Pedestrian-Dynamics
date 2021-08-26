using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSide : MonoBehaviour
{
    [SerializeField] Transform t_AnotherSide;
    [SerializeField] Transform t_Pedestrains;

    static float sf_North => Terrain.sf_Wide - Terrain.sf_SaveGap;
    static float sf_South = Terrain.sf_SaveGap;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IUpdate());
    }

    IEnumerator IUpdate()
    {
        yield return new WaitForEndOfFrame();

        while(true)
        {
            float randPos = Random.Range(sf_South, sf_North);
            GameObject gobj = ModelPool.GetModel();
            gobj.transform.SetParent(t_Pedestrains);

            gobj.transform.position = new Vector3(transform.position.x, transform.position.y, randPos);

            PedestrainHandler ph = gobj.GetComponent<PedestrainHandler>();
            ph.SetTarget(t_AnotherSide);

            yield return new WaitForSeconds(1);
        }
    }
}
