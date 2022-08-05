using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// ���Ƹ�����ʶ��ع���
/// 1. ����MirrorController�и�����ʶ��ϢauxiliaryIndicator
/// 2. ���ݶ�Ӧ�����λ�ø��¸�����ʶ��Ϣ
/// 3. ���ݸ�����ʶ��Ϣ���ɸ�����ʶ
/// 4. ɾ��������ʶ
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
        // Debug.Log("��Indicator��������������������.");
    }

    // �������ǽ�ָʾ����������ͬ�������� https://mirror-networking.gitbook.io/docs/guides/gameobjects/custom-spawnfunctions

    public void GenerateIndicator()
    {
        // ��auxiliaryIndicator��name!=null�ҵ�ǰindicatorPart == null
        if(currentIndicator.state.Equals(IndicatorState.BORN) && indicatorPart == null)
        {
            realStartPart = GameObject.Find(currentIndicator.startPartName+"(Clone)");
            if (realStartPart == null) { Debug.LogError("��indicator��δ�ܸ��������ҵ���Ӧ ��ʼ �������"); return; }
            GameObject partPrefab = FindPartPrefabByName(currentIndicator.startPartName);

            if (currentIndicator.endPartName == null)
            {
                // ˵���յ�����ͨ�ĵ�
                indicatorPart = Instantiate(partPrefab);
                indicatorPart.transform.localPosition = new Vector3(currentIndicator.endPosition.x, currentIndicator.endPosition.y+indicatorPart_offset_y, currentIndicator.endPosition.z); // Ĭ����Ը�������y�Ϸ�
            }
            else
            {
                // �յ����������
                realEndPart = GameObject.Find(currentIndicator.endPartName + "(Clone)");
                if (realEndPart == null) { Debug.LogError("��indicator��δ�ܸ��������ҵ���Ӧ Ŀ�� �������"); return; }

                indicatorPart = Instantiate(partPrefab, realEndPart.transform); // ������������Ϊʵʱ���
                indicatorPart.transform.localPosition = new Vector3(0, indicatorPart_offset_y, 0); // Ĭ����Ը�������y�Ϸ�
            }

            indicatorPart.name = currentIndicator.startPartName + "_indicator";
            indicatorPart.GetComponent<BoxCollider>().enabled = false; // �ر���ײ��
            ChangeLayer(indicatorPart.transform, LayerMask.NameToLayer("DepthCameraUnivisible")); // ����Layer
            ChangeMaterial(indicatorPart.transform, indicatorPartMaterial); // ���Ĳ���

            // ���¸���ָʾ��״̬Ϊ��Ծ
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
            Debug.Log("�ͻ���ɾ���˵�ǰָʾ��");
        }
    }


    private GameObject FindPartPrefabByName(string partName)
    {
        foreach(GameObject partPrefab in partsPrefabList)
        {
            if(partPrefab.name == partName) { return partPrefab; }
        }
        Debug.LogError("��indicator��δ�����������б����ҵ���Ӧ���Ԥ����");
        return null;
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
