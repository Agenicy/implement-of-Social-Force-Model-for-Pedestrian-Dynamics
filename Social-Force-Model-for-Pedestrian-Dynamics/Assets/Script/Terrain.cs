using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [SerializeField] float f_WideSetting;
    public static float sf_Wide;
    public static float sf_SaveGap = 1;

    public static float sf_TDelta = 2;
    public static float sf_RelaxTime = 0.5f;
    public static float sf_PedstrainVision = 200f;
    public static float sf_PedstrainVision_OutsideRate = .5f;

    private void Awake()
    {
        sf_Wide = f_WideSetting;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
