using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyInfo : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    public GameObject parentDevice;

    /// <summary>
    /// �����ƽű��������˻�ģ����
    /// </summary>

    // �����ƽű��� ���װ������ �б�
    public List<GameObject> componentAssemblySequence;
    // ��ǰװ������� ���װ������ �е��±�
    public int curSN = -1;

    // �����ƽű��� ��װ���豸 �б�
    public List<GameObject> deviceList;

    // �����ƽű��� ��װ������ �б�
    public List<GameObject> waikeList;

    private EngineAssemblyInfo engineAssemblyInfo;
    public bool isPartsListInitialized = false;


    private void Awake()
    {
        deviceList = new List<GameObject>();

        engineAssemblyInfo = GetComponentInParent<EngineAssemblyInfo>();

    }

    private void Update()
    {
        if (!isPartsListInitialized)
        {
            foreach (GameObject partPrefab in engineAssemblyInfo.EnginePartsList)
            {
                GameObject realTimePart = GameObject.Find(partPrefab.name + "(Clone)");
                if (realTimePart)
                {
                    deviceList.Add(realTimePart);
                }
            }
            isPartsListInitialized = true;
            Debug.Log("һ���ҵ���:" + deviceList.Count + "�����");
        }
    }
}
