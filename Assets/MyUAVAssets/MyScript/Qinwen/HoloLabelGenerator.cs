using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class HoloLabelGenerator : MonoBehaviour
{
    // ��Ҫ�ɿ�������ʼ��
    private GameObject labelPrefab;

    /// <summary>
    /// ��ǩ����
    /// </summary>
    public Vector3 pivotDirect;
    public float pivotDistance;
    public string labelName = "Label";

    public bool isOnTop = false;
    public bool isOnLeft = false;
    public bool isOnRight = false;
    public bool isOnCenter = false;

    // ��������
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
            Debug.LogError(this.name + "���������б�ǩ");
            return null;
        }

        holoLabel = Instantiate(labelPrefab);
        holoLabel.name = labelName;

        // �趨�ı�
        toolTip = holoLabel.GetComponent<ToolTip>();
        // �ű����ص�����������(clone)�������Ҫɾ��ĩβ�ġ�(clone)��
        toolTip.ToolTipText = this.name.Substring(0,this.name.Length-7);

        // �趨ê�㣨ͨ����Χ�У�
        anchor = holoLabel.transform.GetChild(0).gameObject;

        // anchor.transform.position = getAnchorPosition();
        anchor.transform.position = this.transform.position;

        pivot = holoLabel.transform.GetChild(1).gameObject;
        pivot.transform.position = anchor.transform.position + pivotDirect * pivotDistance;

        // Ȼ�����趨һ�¸����壬���趨���������ƫ�ԭ����
        //holoLabel.transform.parent = this.transform;
        holoLabel.transform.parent = parentObject.transform;

        holoLabel.SetActive(false);

        // ���ñ�ǩ�㼶Ϊ���������ɼ�
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
    /// ͬʱ�޸����弰������������㼶
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeLayer(Transform transform, int layer)
    {
        if (transform.childCount > 0)//������������
        {
            for (int i = 0; i < transform.childCount; i++)//�����������Ƿ���������
            {
                ChangeLayer(transform.GetChild(i), layer);//������ֻ�����һ����������Ķ������ò㼶
            }
            transform.gameObject.layer = layer;//�����ڵ������������������Ҫ�ѵ�ǰ������ڵ���в㼶����
        }
        else					//��������
        {
            transform.gameObject.layer = layer;
        }
    }
}
