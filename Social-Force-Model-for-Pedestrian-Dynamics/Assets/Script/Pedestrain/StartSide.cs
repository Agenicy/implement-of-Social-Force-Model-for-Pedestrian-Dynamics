using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSide : MonoBehaviour
{
    [SerializeField] Transform t_AnotherSide;
    [SerializeField] Transform t_Pedestrains;
    [SerializeField] Color color;

    static float sf_North => 5.0f;
    static float sf_South => -5.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IUpdate());
    }

    IEnumerator IUpdate()
    {
        yield return new WaitForEndOfFrame();

        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                float randPos = Random.Range(sf_South, sf_North);
                GameObject gobj = ModelPool.GetModel();

                gobj.transform.SetParent(transform);
                gobj.transform.localPosition = new Vector3(0, 0, randPos);

                gobj.transform.SetParent(t_Pedestrains);

                PedestrainHandler ph = gobj.GetComponent<PedestrainHandler>();
                ph.SetTarget(t_AnotherSide);
                ph.image.color = color;
            }
            yield return new WaitForSeconds(Random.Range(2,3));
        }
    }
}
