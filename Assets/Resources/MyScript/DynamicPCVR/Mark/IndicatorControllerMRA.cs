using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// 控制辅助标识相关功能
/// 1. 更新MirrorController中辅助标识信息auxiliaryIndicator
/// 2. 根据对应物体的位置更新辅助标识信息
/// 3. 根据辅助标识信息生成辅助标识
/// 4. 删除辅助标识
/// </summary>
public class IndicatorControllerMRA : MonoBehaviour
{
    private MirrorControllerA mirrorController;
    private List<GameObject> partsPrefabList;

    private GameObject realStartPart = null;
    private GameObject realEndPart = null;
    private GameObject indicatorPart = null;
    public float indicatorPart_offset_y = 0.1f;
    public Material indicatorPartMaterial; // from inspector
    private DPCIndicator currentIndicator;

    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponent<MirrorControllerA>();
        partsPrefabList = GetComponent<EngineAssemblyInfo>().EnginePartsList;
    }

    // Update is called once per frame
    void Update()
    {
        currentIndicator = mirrorController.auxiliaryIndicator;
        GenerateIndicator();
        DeleteIndicator();
    }

    public void UpdateIndicator(DPCIndicator indicator)
    {
        mirrorController.CmdUpdateDPCIndicator(indicator);
        // Debug.Log("【Indicator】向服务器发起更新请求.");
    }

    // 后续考虑将指示物做成网络同步的物体 https://mirror-networking.gitbook.io/docs/guides/gameobjects/custom-spawnfunctions

    public void GenerateIndicator()
    {
        // 当auxiliaryIndicator的name!=null且当前indicatorPart == null
        if(currentIndicator.state.Equals(IndicatorState.BORN) && indicatorPart == null)
        {
            realStartPart = GameObject.Find(currentIndicator.startPartName+"(Clone)");
            if (realStartPart == null) { Debug.LogError("【indicator】未能根据名称找到对应 起始 零件物体"); return; }
            GameObject partPrefab = FindPartPrefabByName(currentIndicator.startPartName);

            if (currentIndicator.endPartName == null)
            {
                // 说明终点是普通的点
                indicatorPart = Instantiate(partPrefab);
                indicatorPart.transform.localPosition = new Vector3(currentIndicator.endPosition.x, currentIndicator.endPosition.y+indicatorPart_offset_y, currentIndicator.endPosition.z); // 默认相对父物体在y上方
            }
            else
            {
                // 终点是其他零件
                realEndPart = GameObject.Find(currentIndicator.endPartName + "(Clone)");
                if (realEndPart == null) { Debug.LogError("【indicator】未能根据名称找到对应 目标 零件物体"); return; }

                indicatorPart = Instantiate(partPrefab, realEndPart.transform); // 将父物体设置为实时零件
                indicatorPart.transform.localPosition = new Vector3(0, indicatorPart_offset_y, 0); // 默认相对父物体在y上方
            }

            indicatorPart.name = currentIndicator.startPartName + "_indicator";
            indicatorPart.GetComponent<BoxCollider>().enabled = false; // 关闭碰撞盒
            ChangeLayer(indicatorPart.transform, LayerMask.NameToLayer("DepthCameraUnivisible")); // 更改Layer
            ChangeMaterial(indicatorPart.transform, indicatorPartMaterial); // 更改材质

            // 更新辅助指示物状态为活跃
            currentIndicator.state = IndicatorState.ALIVE;
            UpdateIndicator(currentIndicator);
        }
    }

    public void DeleteIndicator()
    {
        if(currentIndicator.state.Equals(IndicatorState.DEAD) && indicatorPart != null)
        {
            Destroy(indicatorPart);
            realStartPart = null;
            Debug.Log("客户端删除了当前指示物");
        }
    }


    private GameObject FindPartPrefabByName(string partName)
    {
        foreach(GameObject partPrefab in partsPrefabList)
        {
            if(partPrefab.name == partName) { return partPrefab; }
        }
        Debug.LogError("【indicator】未根据名称在列表中找到对应零件预制体");
        return null;
    }



    /// <summary>
    /// 同时修改物体及其所有子物体层级
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeLayer(Transform transform, int layer)
    {
        if (transform.childCount > 0)//如果子物体存在
        {
            for (int i = 0; i < transform.childCount; i++)//遍历子物体是否还有子物体
            {
                ChangeLayer(transform.GetChild(i), layer);//这里是只将最后一个无子物体的对象设置层级
            }
            transform.gameObject.layer = layer;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
        }
        else					//无子物体
        {
            transform.gameObject.layer = layer;
        }
    }


    /// <summary>
    /// 同时修改物体及其所有子物体材质
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeMaterial(Transform transform, Material material)
    {
        if (transform.childCount > 0)//如果子物体存在
        {
            for (int i = 0; i < transform.childCount; i++)//遍历子物体是否还有子物体
            {
                ChangeMaterial(transform.GetChild(i), material);//这里是只将最后一个无子物体的对象设置层级
            }
            if (transform.GetComponent<MeshRenderer>())
            {
                transform.GetComponent<MeshRenderer>().material = material;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
            }
        }
        else					//无子物体
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }
}
