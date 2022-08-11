using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkRenderVRA : MonoBehaviour
{
    private MirrorControllerA mirrorController;

    private List<GameObject> segmentObjectList;
    private List<GameObject> rotationObjectList;
    private List<GameObject> pressObjectList;
    

    public Material segmentMaterial;
    public float segmentThickness = 0.01f;

    public GameObject RotateSymbol;
    public GameObject PressSymbol;

    // 是否允许对标识进行实时更新
    public bool isEnableRealTimeUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponentInParent<MirrorControllerA>();
        segmentObjectList = new List<GameObject>();
        rotationObjectList = new List<GameObject>();
        pressObjectList = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        RenderArrow();
        RenderRotation();
        RenderPress();

        // 动态更新
        if (isEnableRealTimeUpdate)
        {
            UpdateSegment();
        }
        
    }

    /// <summary>
    /// render segmetn on VR client per frame
    /// </summary>
    private void RenderArrow()
    {
        // 1 arrow contained with 3 segment
        int n_curSegmentObj = segmentObjectList.Count; // number of segment object
        int n_curArrow = n_curSegmentObj / 3; // number of segment
        int n_serverArrow = mirrorController.syncArrowList.Count; // number of latest segment list

        int delta = n_serverArrow - n_curArrow;
        // delete segment
        for (int i = 0; i > delta; --i)
        {
            for (int j = 0; j < 3; ++j)
            {
                GameObject tempObj = segmentObjectList[n_curSegmentObj - 1 - i * 3 - j ];
                segmentObjectList.Remove(tempObj);
                Destroy(tempObj);
            }
        }
        // add new segment
        for (int i = 0; i < delta; ++i)
        {
            DPCArrow arrow = mirrorController.syncArrowList[n_curArrow + i];
            Debug.Log("point1:" + arrow.startPoint.ToString() + ",point2:" + arrow.endPoint.ToString());


            SegmentInfo segment = new SegmentInfo()
            {
                startPoint = arrow.startPoint,
                endPoint = arrow.endPoint
            };

            DrawSegment(segment);
            DrawArrow(segment);
        }



    }

    private void UpdateSegment()
    {
        int n_serverArrow = mirrorController.syncArrowList.Count; // number of latest segment list
        for(int i = 0; i < n_serverArrow; i++)
        {
            DPCArrow curArrow = mirrorController.syncArrowList[i];

            // 根据server上最新的箭头指示线信息curArrow更新本地
            GameObject curSegment = segmentObjectList[i * 3 + 0]; // 组成箭头指示线的三条线段中的主干线段
            LineRenderer curLineRenderer = curSegment.GetComponent<LineRenderer>();
            curLineRenderer.SetPosition(0, curArrow.startPoint);
            curLineRenderer.SetPosition(1, curArrow.endPoint);

            Vector3 screenP1 = Camera.main.WorldToScreenPoint(curArrow.startPoint),
            screenP2 = Camera.main.WorldToScreenPoint(curArrow.endPoint);
            Vector2 dir = (screenP1 - screenP2).normalized;
            Vector2 verticalDir = new Vector2(-dir.y, dir.x);

            int length = 20;
            Vector3 screenArrowP1 = screenP2 + length * new Vector3(verticalDir.x, verticalDir.y) + length * new Vector3(dir.x, dir.y),
                screenArrowP2 = screenP2 - length * new Vector3(verticalDir.x, verticalDir.y) + length * new Vector3(dir.x, dir.y);

            Vector3 arrowP1 = Camera.main.ScreenToWorldPoint(screenArrowP1),
                arrowP2 = Camera.main.ScreenToWorldPoint(screenArrowP2);

            GameObject curSegmentArrow1 = segmentObjectList[i * 3 + 1];
            GameObject curSegmentArrow2 = segmentObjectList[i * 3 + 2];


            curSegmentArrow1.GetComponent<LineRenderer>().SetPosition(0, arrowP1);
            curSegmentArrow1.GetComponent<LineRenderer>().SetPosition(1, curArrow.endPoint);

            curSegmentArrow2.GetComponent<LineRenderer>().SetPosition(0, arrowP2);
            curSegmentArrow2.GetComponent<LineRenderer>().SetPosition(1, curArrow.endPoint);

        }
    }


    private void RenderRotation()
    {
        int n_curRotationObj = rotationObjectList.Count;
        int n_clientRotation = mirrorController.syncRotationList.Count;

        int delta = n_clientRotation - n_curRotationObj;
        // delete rotation 
        for (int i = 0; i > delta; --i)
        {
            GameObject tempObj = segmentObjectList[n_clientRotation - 1 + i];
            rotationObjectList.Remove(tempObj);
            Destroy(tempObj);
        }
        // add new rotation
        for (int i = 0; i < delta; ++i)
        {
            DPCSymbol newRotation = mirrorController.syncRotationList[n_curRotationObj + i];

            GameObject tempObj = Instantiate(RotateSymbol);
            tempObj.transform.parent = transform;
            tempObj.transform.position = newRotation.position;
            tempObj.transform.forward = newRotation.up;
            rotationObjectList.Add(tempObj);
        }
    }

    private void RenderPress()
    {
        int n_curPressObj = pressObjectList.Count;
        int n_clientPress = mirrorController.syncPressList.Count;

        int delta = n_clientPress - n_curPressObj;
        // delete press 
        for (int i = 0; i > delta; --i)
        {
            GameObject tempObj = pressObjectList[n_clientPress - 1+i];
            pressObjectList.Remove(tempObj);
            Destroy(tempObj);
        }
        // add new press
        for (int i = 0; i < delta; ++i)
        {
            DPCSymbol newPress = mirrorController.syncPressList[n_curPressObj + i];
            GameObject tempObj = Instantiate(PressSymbol);
            tempObj.transform.parent = transform;
            tempObj.transform.position = newPress.position;
            tempObj.transform.right = newPress.up;
            pressObjectList.Add(tempObj);
        }
    }

    /// <summary>
    /// draw an segment
    /// </summary>
    /// <param name="segmentInfo"></param>
    private void DrawSegment(SegmentInfo segmentInfo)
    {
        GameObject segmentObj = new GameObject();
        segmentObj.transform.SetParent(this.transform);
        segmentObj.layer = LayerMask.NameToLayer("DepthCameraUnivisible");
        LineRenderer segmentRender = segmentObj.AddComponent<LineRenderer>();
        segmentRender.material = segmentMaterial;
        segmentRender.startWidth = segmentThickness;
        segmentRender.endWidth = segmentThickness;
        segmentRender.numCapVertices = 2;
        segmentRender.positionCount = 2;
        segmentRender.SetPosition(0, segmentInfo.startPoint);
        segmentRender.SetPosition(1, segmentInfo.endPoint);

        segmentObjectList.Add(segmentObj);
    }

    /// <summary>
    /// draw two segment as an arrow
    /// </summary>
    /// <param name="segmentInfo"></param>
    private void DrawArrow(SegmentInfo segmentInfo)
    {
        Vector3 screenP1 = Camera.main.WorldToScreenPoint(segmentInfo.startPoint),
            screenP2 = Camera.main.WorldToScreenPoint(segmentInfo.endPoint);
        Vector2 dir = (screenP1 - screenP2).normalized;
        Vector2 verticalDir = new Vector2(-dir.y, dir.x);

        int length = 20;
        Vector3 screenArrowP1 = screenP2 + length * new Vector3(verticalDir.x, verticalDir.y) + length * new Vector3(dir.x, dir.y),
            screenArrowP2 = screenP2 - length * new Vector3(verticalDir.x, verticalDir.y) + length * new Vector3(dir.x, dir.y);

        Vector3 arrowP1 = Camera.main.ScreenToWorldPoint(screenArrowP1),
            arrowP2 = Camera.main.ScreenToWorldPoint(screenArrowP2);

        DrawSegment(new SegmentInfo()
        {
            startPoint = arrowP1,
            endPoint = segmentInfo.endPoint
        });

        DrawSegment(new SegmentInfo()
        {
            startPoint = arrowP2,
            endPoint = segmentInfo.endPoint
        });
    }

    private Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent = 0.5f)
    {
        Vector3 normal = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        return normal * (distance * percent) + start;
    }
}
