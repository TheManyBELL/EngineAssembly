using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderARA : MonoBehaviour
{
    private MirrorControllerA mirrorController; // �����п�
    private List<GameObject> lines;
    private int line_index;

    public Material straightLineMaterial;
    public float straightLineThickness = 0.01f;

    public bool origin_line = true;


    // �����Ӱָʾ��ر���
    private List<GameObject> enginePartsList;
    private DPCIndicator indicator;
    private GameObject virtualPart = null;
    public Material virtualPartMaterial; // from inspector

    public GameObject engineAssemblyContent = null;
    private MainController mainController = null;

    // Start is called before the first frame update
    void Start()
    {
        mirrorController = GetComponentInParent<MirrorControllerA>();
        lines = new List<GameObject>();

        // ��SmartSignA�������б�
        // enginePartsList = GetComponentInParent<EngineAssemblyInfo>().EnginePartsList;

    }

    // Update is called once per frame
    void Update()
    {

        // ����
        if (engineAssemblyContent == null)
        {
            engineAssemblyContent = GameObject.FindGameObjectWithTag("EngineAssemblyContent");
            if (engineAssemblyContent)
            {
                Debug.Log("lineRenderARA ���ҵ� engineAssemblyContent");
                mainController = engineAssemblyContent.GetComponent<MainController>();
                enginePartsList = mainController.deviceList;
            }
        }


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

                        ChangeLayer(virtualPart.transform, LayerMask.NameToLayer("DepthCameraUnivisible"));
                        ChangeMaterial(virtualPart.transform, virtualPartMaterial);

                        virtualPart.transform.Find("Label").gameObject.SetActive(false);// ָʾ�ﲻ��ʾ��ǩ
                        
                        Debug.Log("AR�ͻ���������ǰָʾ��");
                    }
                }
            }
        }
        if(mirrorController.syncArrowList.Count == 0 && virtualPart != null)
        {
            Destroy(virtualPart);
            Debug.Log("AR�ͻ���ɾ���˵�ǰָʾ��");
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

    /// <summary>
    /// ͬʱ�޸����弰���������������
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeMaterial(Transform transform, Material material)
    {
        if (transform.childCount > 0)//������������
        {
            for (int i = 0; i < transform.childCount; i++)//�����������Ƿ���������
            {
                ChangeMaterial(transform.GetChild(i), material);//������ֻ�����һ����������Ķ������ò㼶
            }
            if (transform.GetComponent<MeshRenderer>())
            {
                transform.GetComponent<MeshRenderer>().material = material;//�����ڵ������������������Ҫ�ѵ�ǰ������ڵ���в㼶����
            }
        }
        else					//��������
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }
}
