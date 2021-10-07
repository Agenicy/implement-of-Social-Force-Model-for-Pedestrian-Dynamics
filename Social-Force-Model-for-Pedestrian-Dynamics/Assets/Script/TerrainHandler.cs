using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Text text;

    private void Awake()
    {
        sf_Wide = f_WideSetting;
        Camera.main.transform.position = new Vector3(75, 23, 2);
        Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        terrain.terrainData.size = new Vector3(f_LengthSetting, 256, f_WideSetting);
        transform.localScale = new Vector3(transform.localScale.x * f_LengthSetting / 16f, 1, transform.localScale.z * f_WideSetting / 16f);
        transform.position = new Vector3(f_LengthSetting, 0, f_WideSetting);

    }

    private void Update()
    {
        text.text =
            $"Road Length = {f_LengthSetting}\n" +
            $"Road Wide = {f_WideSetting}\n" +
            $"RelaxTime(Tou) = {sf_RelaxTime}\n" +
            $"PedstrainVision(2Phi) = {sf_PedstrainVision}\n" +
            $"PedstrainVision_OutsideRate = {sf_PedstrainVision_OutsideRate}\n" +
            PedestrainHandler.Info;
    }

    private void Reset()
    {
        terrain = GetComponent<Terrain>();
    }
}
