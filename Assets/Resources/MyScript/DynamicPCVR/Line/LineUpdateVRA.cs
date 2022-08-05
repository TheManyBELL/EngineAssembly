using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpdateVRA : MonoBehaviour
{

    private MirrorControllerA mirrorController;

    private List<GameObject> segmentEndPartList; // �߶��յ�󶨵����
    private List<GameObject> segmentStartPartList; // �߶����󶨵����

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
            Debug.Log("ɾ��part");
            segmentStartPartList.RemoveAt(n_curPartList - 1 - i);
            segmentEndPartList.RemoveAt(n_curPartList - 1 - i);
        }
        for (int i = 0; i < delta; ++i)
        {
            Debug.Log("����part");
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
                //Debug.Log("�յ�����:"+segmentEndPart.name);
                //// ���������һ��������ײ�����յ��������������������߷�����
                //Vector3 rayDicretion =  segmentStartPart.transform.position - segmentEndPart.transform.position;
                //Ray raycast = new Ray(segmentEndPart.transform.position, rayDicretion);
                //RaycastHit hit;
                //Physics.Raycast(raycast, out hit, LayerMask.NameToLayer("DepthCameraOnly"));
                //Debug.Log("ʵʱ���ߣ�" + hit.collider.name);
                //arrow.endPoint = hit.point;
                arrow.endPoint = segmentEndPart.transform.position;
            }
            mirrorController.CmdUpdateDPCArrow(arrow);
        }
    }
}
