using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class HoloLabelGenerator : MonoBehaviour
{
    // 需要由控制器初始化
    private GameObject labelPrefab;

    /// <summary>
    /// 标签参数
    /// </summary>
    public Vector3 pivotDirect;
    public float pivotDistance;
    public string labelName = "Label";

    public bool isOnTop = false;
    public bool isOnLeft = false;
    public bool isOnRight = false;
    public bool isOnCenter = false;

    // 父亲物体
    private GameObject parentObject;

    private GameObject holoLabel;
    private ToolTip toolTip;
    private GameObject anchor;
    private GameObject pivot;

    void Start()
    {
        
    }

    private void Update()
    {
        anchor.transform.position = this.transform.position;
        pivot.transform.position = anchor.transform.position + pivotDirect * pivotDistance;
    }

    public void SetLabelPrefab(GameObject prefab)
    {
        labelPrefab = prefab;
    }

    public void SetLabelParentObject(GameObject parent)
    {
        parentObject = parent;
    }

    public void SetPivotPosition(Vector3 dir,float dis)
    {
        pivotDirect = dir;
        pivotDistance = dis;
    }

    public GameObject GenerateHoloLabel()
    {
        if (transform.Find(labelName))
        {
            Debug.LogError(this.name + "该物体已有标签");
            return null;
        }

        holoLabel = Instantiate(labelPrefab);
        holoLabel.name = labelName;

        // 设定文本
        toolTip = holoLabel.GetComponent<ToolTip>();
        // 脚本挂载的零件名会带上(clone)，因此需要删除末尾的“(clone)”
        toolTip.ToolTipText = this.name.Substring(0,this.name.Length-7);

        // 设定锚点（通过包围盒）
        anchor = holoLabel.transform.GetChild(0).gameObject;

        // anchor.transform.position = getAnchorPosition();
        anchor.transform.position = this.transform.position;

        pivot = holoLabel.transform.GetChild(1).gameObject;
        pivot.transform.position = anchor.transform.position + pivotDirect * pivotDistance;

        // 然后再设定一下父物体，先设定父物体会有偏差，原因不明
        //holoLabel.transform.parent = this.transform;
        holoLabel.transform.parent = parentObject.transform;

        holoLabel.SetActive(false);

        // 设置标签层级为深度相机不可见
        ChangeLayer(holoLabel.transform, LayerMask.NameToLayer("DepthCameraUnivisible"));

        return holoLabel;
    }

    private Vector3 getAnchorPosition()
    {
        GameObject meshCenter = this.transform.Find("MeshCenter").gameObject;
        Bounds meshCenterBounds = meshCenter.GetComponent<MeshCenterInfo>().componentBounds;
        Vector3 anchorPosition = new Vector3();
        if (isOnCenter) { anchorPosition = meshCenter.transform.position; }
        else if (isOnTop) { anchorPosition = meshCenter.transform.position + new Vector3(0, meshCenterBounds.size.y / 2f, 0); }
        else if (isOnLeft) { anchorPosition = meshCenter.transform.position + new Vector3(-meshCenterBounds.size.x / 2f, 0, 0); }
        else if (isOnRight) { anchorPosition = meshCenter.transform.position + new Vector3(meshCenterBounds.size.x / 2f, 0, 0); }
        return anchorPosition;
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
}
