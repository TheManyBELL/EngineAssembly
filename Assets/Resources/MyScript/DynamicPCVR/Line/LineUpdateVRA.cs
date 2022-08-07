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
                arrow.endPoint = intersectBox(arrow.startPoint, segmentEndPart);
                //arrow.endPoint = segmentEndPart.transform.position;
            }
            mirrorController.CmdUpdateDPCArrow(arrow);
        }
    }

    private Vector3 intersectBox(Vector3 p1, GameObject t)
    {
        Vector3 p2 = t.transform.position;
        Bounds tAABB = new Bounds();
        Renderer[] renderers = t.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            tAABB.Encapsulate(renderers[i].bounds);
        }

        float x = tAABB.extents.x, y = tAABB.extents.y, z = tAABB.extents.z;
        float scale = 1.0f;
        Vector3[] vAABB = new Vector3[]{
            tAABB.center + scale * new Vector3( x,  y,  z),
            tAABB.center + scale * new Vector3( x,  y, -z),
            tAABB.center + scale * new Vector3( x, -y,  z),
            tAABB.center + scale * new Vector3( x, -y, -z),
            tAABB.center + scale * new Vector3(-x,  y,  z),
            tAABB.center + scale * new Vector3(-x,  y, -z),
            tAABB.center + scale * new Vector3(-x, -y,  z),
            tAABB.center + scale * new Vector3(-x, -y, -z)
        };

        // sphere
        float dis = Vector3.Distance(p1, p2);
        Vector3 dir = (p2 - p1).normalized;
        float r = Mathf.Sqrt(x * x + y * y + z * z) * 0.8f;
        Vector3 intersect_sphere = p1 + (dis - r) * dir;

        // cube
        float x_min = Mathf.Min((vAABB[0].x - p1.x) / dir.x, (vAABB[7].x - p1.x) / dir.x);
        float y_min = Mathf.Min((vAABB[0].y - p1.y) / dir.y, (vAABB[7].y - p1.y) / dir.y);
        float z_min = Mathf.Min((vAABB[0].z - p1.z) / dir.z, (vAABB[7].z - p1.z) / dir.z);
        float t_max = Mathf.Max(Mathf.Max(x_min, y_min), z_min);
        Vector3 intersect_cube = p1 + t_max * dir;

        return intersect_cube;
    }
}
