using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderMarker : MonoBehaviour
{
    [SerializeField] Collider collider;

    public static List<Collider> list_col_AllBorder = new List<Collider>();

    private void Awake()
    {
        list_col_AllBorder.Add(collider);
    }

    private void Reset()
    {
        collider = GetComponent<Collider>();
    }
}
