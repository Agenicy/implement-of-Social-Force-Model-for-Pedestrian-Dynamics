using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHandler : MonoBehaviour
{
    [SerializeField] Terrain terrain;
    [SerializeField] float f_LengthSetting;
    [SerializeField] float f_WideSetting;
    public static float sf_Wide;
    public static float sf_SaveGap = 2;

    public static float sf_TDelta = 1f;
    public static float sf_RelaxTime = .5f;
    public static float sf_PedstrainVision = 200f;
    public static float sf_PedstrainVision_OutsideRate = .5f;

    private void Awake()
    {
        sf_Wide = f_WideSetting;
    }

    // Start is called before the first frame update
    void Start()
    {
        terrain.terrainData.size = new Vector3(f_LengthSetting, 256, f_WideSetting);
        transform.localScale = new Vector3(transform.localScale.x * f_LengthSetting / 16f, 1, transform.localScale.z * f_WideSetting / 16f);
        transform.position = new Vector3(f_LengthSetting, 0, f_WideSetting);
    }

    private void Reset()
    {
        terrain = GetComponent<Terrain>();
    }
}
