using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineAssistanceMRA : MonoBehaviour
{
    private MirrorControllerA mirrorController = null;
    private DPCPreSelect segmentPreSelect;
    private GameObject vrAvator = null;
    private VR_PlayerScript vr_playerScript = null;
    private RayInfo rightHand;

    public GameObject assistSegmentPrefab; // 需要在inspector中拖入预制体
    private GameObject assistSegmentObject; // 实例化后
    private LineRenderer assistLineRenderer; // 已经设置好线段的材质和粗细等属性

    public GameObject drawPointPrefab;
    private List<GameObject> drawPointList;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private PreSelectMode currentMode = PreSelectMode.CLOSE;



    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponent<MirrorControllerA>();
        assistSegmentObject = Instantiate(assistSegmentPrefab);
        assistLineRenderer = assistSegmentObject.GetComponent<LineRenderer>();
        assistLineRenderer.enabled = false;

        drawPointList = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (vrAvator == null)
        {
            vrAvator = GameObject.Find("VR_Avator(Clone)");
            if (vrAvator == null) { return; }
            vr_playerScript = vrAvator.GetComponent<VR_PlayerScript>();
        }

        rightHand = vr_playerScript.rightHand;
        segmentPreSelect = mirrorController.segmentPreSelect;

        if(currentMode==PreSelectMode.CLOSE && segmentPreSelect.state == PreSelectMode.STARTSELECTED)
        {
            Debug.Log("放置起点");
            // 放置点
            startPoint = segmentPreSelect.startPoint;

            GameObject drawPoint = Instantiate(drawPointPrefab);
            drawPoint.transform.position = startPoint;
            drawPoint.layer = LayerMask.NameToLayer("DepthCameraUnivisible");
            drawPoint.name = "segmentAssistSphere_start";
            drawPointList.Add(drawPoint);
            // 设置线段渲染器的起始点
            assistLineRenderer.enabled = true;
            assistLineRenderer.SetPosition(0, startPoint);

            currentMode = PreSelectMode.STARTSELECTED;
        }
        if (currentMode == PreSelectMode.STARTSELECTED && segmentPreSelect.state==PreSelectMode.STARTSELECTED)
        {
            // 绘制连线
            assistLineRenderer.SetPosition(1, rightHand.endPoint);
        }
        if(currentMode == PreSelectMode.STARTSELECTED && segmentPreSelect.state == PreSelectMode.ENDSELECTED)
        {
            Debug.Log("放置终点");
            endPoint = segmentPreSelect.endPoint;

            GameObject drawPoint = Instantiate(drawPointPrefab);
            drawPoint.transform.position = endPoint;
            drawPoint.layer = LayerMask.NameToLayer("DepthCameraUnivisible");
            drawPoint.name = "segmentAssistSphere_end";
            drawPointList.Add(drawPoint);

            assistLineRenderer.SetPosition(1, endPoint);
            currentMode = PreSelectMode.ENDSELECTED;
        }
        if (currentMode == PreSelectMode.ENDSELECTED && segmentPreSelect.state == PreSelectMode.CLOSE)
        {
            assistLineRenderer.enabled = false;
            currentMode = PreSelectMode.CLOSE;
            for (int i = 0; i < drawPointList.Count; i++)
            {
                Destroy(drawPointList[i]);
            }
            drawPointList.Clear();
        }
    }
}
