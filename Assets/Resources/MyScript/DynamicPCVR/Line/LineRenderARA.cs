using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderARA : MonoBehaviour
{
    private MirrorControllerA mirrorController; // 网络中控
    private List<GameObject> lines;
    private int line_index;

    public Material straightLineMaterial;
    public float straightLineThickness = 0.01f;

    public bool origin_line = true;


    // 零件虚影指示相关变量
    private List<GameObject> enginePartsList;
    private DPCIndicator indicator;
    private GameObject virtualPart = null;
    public Material virtualPartMaterial; // from inspector

    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponentInParent<MirrorControllerA>();
        lines = new List<GameObject>();

        // 从SmartSignA获得零件列表
        enginePartsList = GetComponentInParent<EngineAssemblyInfo>().EnginePartsList;

    }

    // Update is called once per frame
    void Update()
    {
        line_index = 0;
        for (int i = 0; i < mirrorController.syncArrowList.Count; ++i)
        {
            DPCArrow current_line = mirrorController.syncArrowList[i];
            Debug.Log("!!! " + current_line.curvePointList.Count);
            DrawLine(ref current_line);
        }
        ClearLine();

        // qinwen
        if(mirrorController.syncArrowList.Count == 1 && virtualPart==null)
        {
            indicator = mirrorController.auxiliaryIndicator;
            if (indicator.name != null)
            {
                foreach (GameObject partsObj in enginePartsList)
                {
                    if (indicator.name == partsObj.name)
                    {
                        virtualPart = Instantiate(partsObj);
                        virtualPart.transform.position = indicator.position;
                        virtualPart.GetComponent<MeshRenderer>().material = virtualPartMaterial;
                        Debug.Log("AR客户端新增当前指示物");
                    }
                }
            }
        }
        if(mirrorController.syncArrowList.Count == 0 && virtualPart != null)
        {
            Destroy(virtualPart);
            Debug.Log("AR客户端删除了当前指示物");
        }
        

    }

    void DrawLine(ref DPCArrow currend_line)
    {
        if (origin_line)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (line_index >= lines.Count)
                {
                    lines.Add(CreateNewLine("line" + line_index.ToString()));
                }

                if (i == 2)
                {
                    lines[line_index].GetComponent<LineRenderer>().positionCount = 2;
                    lines[line_index].GetComponent<LineRenderer>().SetPosition(0, currend_line.startPoint);
                    lines[line_index].GetComponent<LineRenderer>().SetPosition(1, currend_line.endPoint);
                }
                else
                {
                    lines[line_index].GetComponent<LineRenderer>().positionCount = currend_line.curvePointList[i].Length;
                    lines[line_index].GetComponent<LineRenderer>().SetPositions(currend_line.curvePointList[i]);
                }
                ++line_index;
            }
        }
        else
        {
            foreach (var t in currend_line.curvePointList)
            {
                if (line_index >= lines.Count)
                {
                    lines.Add(CreateNewLine("line" + line_index.ToString()));
                }

                lines[line_index].GetComponent<LineRenderer>().positionCount = t.Length;
                lines[line_index].GetComponent<LineRenderer>().SetPositions(t);
                ++line_index;
            }
        }

    }

    void ClearLine()
    {
        while (line_index < lines.Count)
        {
            lines[line_index++].GetComponent<LineRenderer>().positionCount = 0;
        }
    }


    private GameObject CreateNewLine(string objName)
    {
        GameObject lineObj = new GameObject(objName);
        lineObj.transform.SetParent(this.transform);
        LineRenderer curveRender = lineObj.AddComponent<LineRenderer>();
        curveRender.material = straightLineMaterial;

        curveRender.startWidth = straightLineThickness;
        curveRender.endWidth = straightLineThickness;
        return lineObj;
    }
}
