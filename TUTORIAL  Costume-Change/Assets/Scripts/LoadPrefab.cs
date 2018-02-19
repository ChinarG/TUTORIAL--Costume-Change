using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 第二个场景加上个场景所保存的数据人物
/// </summary>
public class LoadPrefab : MonoBehaviour
{
    void Start()
    {
        if (MainChange.Instance.SexCount == 0)
        {
            MainChange.Instance.GirlClone().transform.position = new Vector3(0,0,-7);//赋值新的位置

        }
        else
        {
            MainChange.Instance.BoyClone().transform.position = new Vector3(0, 0, -7);
        }
    }
}