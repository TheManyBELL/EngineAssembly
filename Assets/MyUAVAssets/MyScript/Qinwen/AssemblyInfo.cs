using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyInfo : MonoBehaviour
{
    /// <summary>
    /// 即将废弃
    /// </summary>
    public GameObject parentDevice;

    /// <summary>
    /// 主控制脚本，在无人机模型上
    /// </summary>

    // 主控制脚本的 组件装配序列 列表
    public List<GameObject> componentAssemblySequence;
    // 当前装配组件在 组件装配序列 中的下标
    public int curSN = -1;

    // 主控制脚本的 需装配设备 列表
    public List<GameObject> deviceList;

    // 主控制脚本的 需装配线束 列表
    public List<GameObject> waikeList;

    private EngineAssemblyInfo engineAssemblyInfo;
    public bool isPartsListInitialized = false;


    private void Awake()
    {
        deviceList = new List<GameObject>();

        engineAssemblyInfo = GetComponentInParent<EngineAssemblyInfo>();

    }

    private void Update()
    {
        if (!isPartsListInitialized)
        {
            foreach (GameObject partPrefab in engineAssemblyInfo.EnginePartsList)
            {
                GameObject realTimePart = GameObject.Find(partPrefab.name + "(Clone)");
                if (realTimePart)
                {
                    deviceList.Add(realTimePart);
                }
            }
            isPartsListInitialized = true;
            Debug.Log("一共找到了:" + deviceList.Count + "个零件");
        }
    }
}
