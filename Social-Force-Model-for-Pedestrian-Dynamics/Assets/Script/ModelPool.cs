using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控管角色模型的池
/// </summary>
public class ModelPool : MonoBehaviour
{
    public static ModelPool Get;

    /// <summary>
    /// 存放角色用的空物件
    /// </summary>
    [SerializeField] Transform t_Pool;

    /// <summary>
    /// 玩家與怪物模型所在位置
    /// </summary>
    [SerializeField] GameObject prefab_Pedestrain;

    private void Awake()
    {
        Get = this;
    }
    private void OnDestroy()
    {
        Get = null;
    }

    Queue<GameObject> queue_gobj_ObjList = new Queue<GameObject>();

    /// <summary>
    /// 根據指定類型新建對應 Model；如果該Model已被生成，則回傳生成過的內容
    /// </summary>
    /// <param name="targetModel">指定的 CreatureModel</param>
    /// <returns>targetModel.gobj_Model</returns>
    public static GameObject GetModel()
    {
        // Check
        if (Get.queue_gobj_ObjList.Count > 0)
        {
            GameObject gobj = Get.queue_gobj_ObjList.Dequeue();
            gobj.SetActive(true);
            return gobj;
        }
        else
        {
            return Instantiate(Get.prefab_Pedestrain);
        }
    }

    public static void Recycle(GameObject gobj)
    {
        gobj.transform.SetParent(Get.t_Pool);
        gobj.SetActive(false);
        gobj.transform.localPosition = Vector3.zero;
    }
}
