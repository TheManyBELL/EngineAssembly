using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpdateVRA : MonoBehaviour
{

    private MirrorControllerA mirrorController;

    private List<GameObject> segmentEndPartList; // 线段终点绑定的零件
    private List<GameObject> segmentStartPartList; // 线段起点绑定的零件

    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponentInParent<MirrorControllerA>();

        segmentStartPartList = new List<GameObject>();
        segmentEndPartList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePartList();
        UpdateArrowStartAndEnd();
    }


    void UpdatePartList()
    {
        int n_curPartList = segmentEndPartList.Count;
        int n_curServerArrow = mirrorController.syncArrowList.Count;
        int delta = n_curServerArrow - n_curPartList;

        for (int i = 0; i > delta; --i)
        {
            Debug.Log("删除part");
            segmentStartPartList.RemoveAt(n_curPartList - 1 - i);
            segmentEndPartList.RemoveAt(n_curPartList - 1 - i);
        }
        for (int i = 0; i < delta; ++i)
        {
            Debug.Log("增加part");
            DPCArrow arrow = mirrorController.syncArrowList[n_curPartList + i];
            segmentStartPartList.Add(arrow.startPartName == null ? null : GameObject.Find(arrow.startPartName + "(Clone)"));
            segmentEndPartList.Add(arrow.endPartName == null ? null : GameObject.Find(arrow.endPartName + "(Clone)"));
        }
    }

    void UpdateArrowStartAndEnd()
    {
        int n_curServerArrow = mirrorController.syncArrowList.Count;
        for (int i = 0; i < n_curServerArrow; i++)
        {
            DPCArrow arrow = mirrorController.syncArrowList[i];
            GameObject segmentStartPart = segmentStartPartList[i];
            GameObject segmentEndPart = segmentEndPartList[i];
            if(segmentStartPart==null && segmentEndPart == null) { return; }
            if (segmentStartPart != null)
            {
                arrow.startPoint = segmentStartPart.transform.position;
            }
            if(segmentEndPart != null)
            {
                //Debug.Log("终点物体:"+segmentEndPart.name);
                //// 在这里进行一次射线碰撞，从终点物体中心沿着两点连线方向发射
                //Vector3 rayDicretion =  segmentStartPart.transform.position - segmentEndPart.transform.position;
                //Ray raycast = new Ray(segmentEndPart.transform.position, rayDicretion);
                //RaycastHit hit;
                //Physics.Raycast(raycast, out hit, LayerMask.NameToLayer("DepthCameraOnly"));
                //Debug.Log("实时射线：" + hit.collider.name);
                //arrow.endPoint = hit.point;
                arrow.endPoint = segmentEndPart.transform.position;
            }
            mirrorController.CmdUpdateDPCArrow(arrow);
        }
    }
}
