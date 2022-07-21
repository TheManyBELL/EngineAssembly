using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class MarkPlacementVRA : MonoBehaviour
{

    private MirrorControllerA myController;

    private SymbolMode currentSymbolMode = SymbolMode.ARROW; // default mode is arrow

    public SteamVR_Action_Boolean switchSymbolMode;
    public SteamVR_Action_Boolean confirmSelection;
    public SteamVR_Action_Boolean deleteLastSymbol;

    private List<Vector3> currentPointList = new List<Vector3>();

    private GameObject rightHand;

    public GameObject rotateSymbolPrefab;
    public GameObject pressSymbolPrefab;
    private GameObject rotateSymbol;
    private GameObject pressSymbol;

    private GlobalUtilsVR globalUtils; // VR�����࣬���������ײ

    public GameObject assistPlaceSpherePrefab;
    private GameObject assistPlaceSphere;
    public GameObject assistColliderSpherePrefab;
    private GameObject assistColliderSphere;

    // �����߶�ѡ��
    public GameObject drawpointprefab;
    public List<GameObject> drawpointList;

    // �����Ӱָʾ��ر���
    private List<GameObject> enginePartsList;
    private GameObject vrSelectedPart = null;
    private GameObject virtualPart = null;
    private float virtualPart_offset_y = 0.1f;
    public Material virtualPartMaterial; // from inspector

    private enum SymbolPRState
    {
        Inactive = 0, SelectPosition, SelectRotation
    }
    private SymbolPRState nowPRState = SymbolPRState.Inactive;

    private void Awake()
    {
        globalUtils = GetComponent<GlobalUtilsVR>();
        // ��ȡ��������б�
        enginePartsList = GetComponentInParent<EngineAssemblyInfo>().EnginePartsList;
    }

    // Start is called before the first frame update
    void Start()
    {
        myController = GetComponentInParent<MirrorControllerA>();

        rightHand = GameObject.Find("[CameraRig]/Controller (right)");

        assistPlaceSphere = Instantiate(assistPlaceSpherePrefab);
        assistPlaceSphere.layer = LayerMask.NameToLayer("AssitRotateSphere"); ;
        assistPlaceSphere.SetActive(false);

        assistColliderSphere = Instantiate(assistColliderSpherePrefab);
        assistColliderSphere.layer = LayerMask.NameToLayer("VRCameraUnvisible"); ;
        assistColliderSphere.SetActive(false);

        rotateSymbol = Instantiate(rotateSymbolPrefab);
        rotateSymbol.SetActive(false);

        pressSymbol = Instantiate(pressSymbolPrefab);
        pressSymbol.SetActive(false);

        drawpointList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (switchSymbolMode.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            SwitchSymbolMode();
            Debug.Log("switch symbol mode: " + currentSymbolMode);
        }
        if (currentSymbolMode.Equals(SymbolMode.ARROW))
        {
            if (confirmSelection.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("press the select button");
                AddArrowPoint();
            }
            if (deleteLastSymbol.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("press the delete button");
                //DeleteLastArrow();
                DeleteAllArrow();
            }
        }
        else if (currentSymbolMode.Equals(SymbolMode.PRESS))
        {
            AddPress();
        }
        else if (currentSymbolMode.Equals(SymbolMode.ROTATE))
        {
            AddRotation();
        }
    }

    /// <summary>
    /// clear environment, and then switch mode
    /// </summary>
    private void SwitchSymbolMode()
    {
        // clear environment
        if (currentSymbolMode.Equals(SymbolMode.ARROW))
        {
            currentPointList.Clear();
        }
        nowPRState = SymbolPRState.Inactive;

        // switch mode
        int n_symbol = System.Enum.GetNames(typeof(SymbolMode)).Length; // get symbol numbers
        currentSymbolMode = (SymbolMode)(((int)currentSymbolMode + 1) % n_symbol);


    }

    private void AddArrowPoint()
    {
        Vector3 newPoint = GetCollisionPoint();
        Debug.Log("select point is:" + newPoint.ToString());


        int currentPointNumber = currentPointList.Count;

        // �����ʱ�ǵ�һ���㣬���ٴη������߼����ײ���壬��ײ���������ʾ���������
        if(currentPointNumber == 0)
        {
            Ray raycast = new Ray(rightHand.transform.position, rightHand.transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit, LayerMask.NameToLayer("DepthCameraOnly")); //  
            if (!bHit)
            {
                Debug.Log("VR��û��ѡ�����");
            }
            else
            {
                vrSelectedPart = hit.collider.gameObject;
                Debug.Log("ѡ�е������: "+hit.collider.gameObject.name);
            }
        }

        if (currentPointNumber < 2)
        {
            GameObject pointobj = Instantiate(drawpointprefab);
            pointobj.transform.position = newPoint;
            pointobj.layer = LayerMask.NameToLayer("DepthCameraUnivisible");
            drawpointList.Add(pointobj);
            Debug.Log("current point number is:" + currentPointNumber + ", add new point");
            currentPointList.Add(newPoint);

        }
        if (currentPointNumber == 2)
        {
            Debug.Log("current point number is:" + currentPointNumber + ", update segment");
            myController.CmdAddDPCArrow(new DPCArrow()
            {
                index = myController.syncArrowList.Count,
                startPoint = currentPointList[0],
                endPoint = currentPointList[1],
                curvePointList = new List<Vector3[]>(),
            });

            // 2022.7.21 ��ѡ�������㲢ȷ�ϣ����յ�λ��������Ӱ
            if (vrSelectedPart)
            {
                virtualPart = Instantiate(vrSelectedPart);
                virtualPart.transform.position = new Vector3(currentPointList[1].x, currentPointList[1].y+0.4f, currentPointList[1].z);
                virtualPart.GetComponent<MeshRenderer>().material = virtualPartMaterial;
                myController.CmdUpdateDPCIndicator(new DPCIndicator()
                {
                    name = vrSelectedPart.name,
                    position = virtualPart.transform.position
                });
            }

            // �����ʱ����
            currentPointList.Clear();
            for (int i = 0; i < drawpointList.Count; i++)
            {
                Destroy(drawpointList[i]);
            }
            drawpointList.Clear();
        }
    }

    private void DeleteLastArrow()
    {

        Debug.Log("VR�ͻ��˷���ɾ���߶�����");
        //myController.CmdDeleteSegmentInfo();
    }

    /// <summary>
    /// date:2022.7.21
    /// author:qinwen
    /// introduction:��������߶��б�
    /// </summary>
    private void DeleteAllArrow()
    {
        myController.CmdDeleteAllArrow();
        Debug.Log("VR�ͻ��˷���ɾ�������߶ε�����");
        vrSelectedPart = null;
        Destroy(virtualPart);
        Debug.Log("VR�ͻ���ɾ���˵�ǰָʾ��");
    }

    private Vector3 GetCollisionPoint()
    {
        //TODO
        int MAXSTEP = 1000, stepCount = 0;
        float step = 0.01f;
        assistColliderSphere.transform.position = rightHand.transform.position;
        while (globalUtils.GameObjectVisible(assistColliderSphere))
        {
            assistColliderSphere.transform.position += step * rightHand.transform.forward;
            stepCount++;
            if (stepCount > MAXSTEP) break;
        }

        return (assistColliderSphere.transform.position - 3 * step * rightHand.transform.forward);
    }

    private void AddRotation()
    {
        if (nowPRState == SymbolPRState.Inactive)
        {
            nowPRState = SymbolPRState.SelectPosition;
        }

        Ray ray = new Ray(rightHand.transform.position, rightHand.transform.forward);
        RaycastHit hitInfo;
        int assitSphereLayer = LayerMask.NameToLayer("AssitRotateSphere");
        int onlyCastAssitSphere = 1 << (assitSphereLayer);

        // fisrt select assist sphere position
        if (nowPRState == SymbolPRState.SelectPosition)
        {
            if (confirmSelection.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("Rotation symbol, select assist sphere position");
                assistPlaceSphere.SetActive(true);
                assistPlaceSphere.transform.position = GetCollisionPoint();
                assistPlaceSphere.GetComponent<MeshRenderer>().enabled = true;

                nowPRState = SymbolPRState.SelectRotation;
                rotateSymbol.SetActive(true);
            }
        }

        // second select symbol rotation on surface, and confirm
        else if (nowPRState.Equals(SymbolPRState.SelectRotation))
        {
            Debug.Log("state is select rotation");
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, onlyCastAssitSphere))
            {
                Debug.Log("ray hit");
                rotateSymbol.transform.position = hitInfo.point;
                rotateSymbol.transform.forward = hitInfo.normal;
                if (confirmSelection.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    Debug.Log("Rotation symbol, select symbol position");
                    assistPlaceSphere.SetActive(false);
                    nowPRState = SymbolPRState.Inactive;
                    myController.CmdAddDPCRotation(new DPCSymbol()
                    {
                        index = myController.syncRotationList.Count,
                        up = hitInfo.normal,
                        position = rotateSymbol.transform.position,
                        up_new = new Vector3(),
                        position_new = new Vector3()
                    }) ;
                    rotateSymbol.SetActive(false);
                }

            }
        }

    }

    private void AddPress()
    {
        if (nowPRState == SymbolPRState.Inactive)
        {
            nowPRState = SymbolPRState.SelectPosition;
        }

        Ray ray = new Ray(rightHand.transform.position, rightHand.transform.forward);
        RaycastHit hitInfo;
        int assitSphereLayer = LayerMask.NameToLayer("AssitRotateSphere");
        int onlyCastAssitSphere = 1 << (assitSphereLayer);

        if (nowPRState.Equals(SymbolPRState.SelectPosition))
        {
            if (confirmSelection.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("Press symbol, select assist sphere position");
                assistPlaceSphere.SetActive(true);
                assistPlaceSphere.transform.position = GetCollisionPoint();
                assistPlaceSphere.GetComponent<MeshRenderer>().enabled = true;
                nowPRState = SymbolPRState.SelectRotation;
                pressSymbol.SetActive(true);
            }
        }

        else if (nowPRState.Equals(SymbolPRState.SelectRotation))
        {
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, onlyCastAssitSphere))
            {

                pressSymbol.transform.position = hitInfo.point + 0.05f * hitInfo.normal;
                pressSymbol.transform.right = hitInfo.normal;
                if (confirmSelection.GetStateDown(SteamVR_Input_Sources.RightHand))
                {
                    Debug.Log("Press symbol, select symbol position");
                    assistPlaceSphere.SetActive(false);
                    nowPRState = SymbolPRState.Inactive;
                    myController.CmdAddDPCPress(new DPCSymbol()
                    {
                        index = myController.syncPressList.Count,
                        up = hitInfo.normal,
                        position = pressSymbol.transform.position,
                        up_new = new Vector3(),
                        position_new = new Vector3()
                    });
                    pressSymbol.SetActive(false);
                }
            }
        }

    }

    


}
