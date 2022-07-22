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
    public GameObject engineAssemblyContent = null;
    private MainController mainController = null;

    // 主控制脚本的 组件装配序列 列表
    public List<GameObject> componentAssemblySequence;
    // 当前装配组件在 组件装配序列 中的下标
    public int curSN = -1;

    // 主控制脚本的 需装配设备 列表
    public List<GameObject> deviceList;

    // 主控制脚本的 需装配线束 列表
    public List<GameObject> waikeList;

    private void Awake()
    {
        deviceList = new List<GameObject>();

        //componentAssemblySequence = mainController.componentAssemblySequence;
        //deviceList = mainController.deviceList;
        //waikeList = mainController.waikeList;
    }

    private void Update()
    {
        // 查找
        if(engineAssemblyContent == null)
        {
            engineAssemblyContent = GameObject.FindGameObjectWithTag("EngineAssemblyContent");
            if (engineAssemblyContent)
            {
                Debug.Log("Assemblyinfo 已找到 engineAssemblyContent");
                mainController = engineAssemblyContent.GetComponent<MainController>();
                deviceList = mainController.deviceList;
            }
        }
    }
}
