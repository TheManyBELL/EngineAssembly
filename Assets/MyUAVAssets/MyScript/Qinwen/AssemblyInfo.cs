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
    public GameObject engineAssemblyContent = null;
    private MainController mainController = null;

    // �����ƽű��� ���װ������ �б�
    public List<GameObject> componentAssemblySequence;
    // ��ǰװ������� ���װ������ �е��±�
    public int curSN = -1;

    // �����ƽű��� ��װ���豸 �б�
    public List<GameObject> deviceList;

    // �����ƽű��� ��װ������ �б�
    public List<GameObject> waikeList;

    private void Awake()
    {
        deviceList = new List<GameObject>();

        //componentAssemblySequence = mainController.componentAssemblySequence;
        //deviceList = mainController.deviceList;
        //waikeList = mainController.waikeList;
    }

    private void Update()
    {
        // ����
        if(engineAssemblyContent == null)
        {
            engineAssemblyContent = GameObject.FindGameObjectWithTag("EngineAssemblyContent");
            if (engineAssemblyContent)
            {
                Debug.Log("Assemblyinfo ���ҵ� engineAssemblyContent");
                mainController = engineAssemblyContent.GetComponent<MainController>();
                deviceList = mainController.deviceList;
            }
        }
    }
}
