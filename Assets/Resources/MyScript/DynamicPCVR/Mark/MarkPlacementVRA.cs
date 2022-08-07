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

    private GlobalUtilsVR globalUtils; // VR工具类，用于深度碰撞

    public GameObject assistPlaceSpherePrefab;
    private GameObject assistPlaceSphere;
    public GameObject assistColliderSpherePrefab;
    private GameObject assistColliderSphere;

    // 辅助线段选点
    public GameObject drawpointprefab;
    public List<GameObject> drawpointList;

    // 零件虚影指示相关变量
    private GameObject startPointPart = null; // 起点处零件名称
    private GameObject endPointPart = null; // 终点处零件名称
    private IndicatorControllerMRA indicatorController;

    private enum SymbolPRState
    {
        Inactive = 0, SelectPosition, SelectRotation
    }
    private SymbolPRState nowPRState = SymbolPRState.Inactive;

    private void Awake()
    {
        globalUtils = GetComponent<GlobalUtilsVR>();
        // 获取引擎零件列表
        // enginePartsList = GetComponentInParent<EngineAssemblyInfo>().EnginePartsList;
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

        indicatorController = GetComponentInParent<IndicatorControllerMRA>(); // 获取本物体下的组件
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

        // 如果此时是第一个点
        if(currentPointNumber == 0)
        {
            // 流程1：尝试获取当前射线路径上的零件。若无零件则startPointPart == null
            GetPartByHandleRay(rightHand, ref startPointPart);

            // 流程2：向服务器提交初次预选点信息
            myController.CmdUpdateSegmentPreSelect(new DPCPreSelect()
            {
                startPoint = newPoint,
                endPoint = new Vector3(),
                state = PreSelectMode.STARTSELECTED
            });

            // 流程3：将当前点（点云碰撞点）加入列表
            currentPointList.Add(newPoint);
        }

        // 如果此时是第二个点,更新终点坐标
        if (currentPointNumber == 1)
        {
            // 流程1：尝试获取当前射线路径上的零件
            GetPartByHandleRay(rightHand, ref endPointPart);

            // 流程2：向服务器提交二次预选点信息（终点）
            myController.CmdUpdateSegmentPreSelect(new DPCPreSelect()
            {
                startPoint = currentPointList[0],
                endPoint = newPoint,
                state = PreSelectMode.ENDSELECTED
            });

            // 流程3：将当前点（点云碰撞点）加入列表
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
                startPartName = startPointPart == null ? null : startPointPart.name.Substring(0, startPointPart.name.Length - 7),
                endPartName = endPointPart == null ? null : endPointPart.name.Substring(0, endPointPart.name.Length - 7),
                curvePointList = new List<Vector3[]>(),
            });

            // 2022.7.21 已选中两个点并确认，在终点位置生成虚影
            // 只有一开始选到了零件才会提交零件虚影信息
            if (startPointPart)
            {
                indicatorController.UpdateIndicator(new DPCIndicator()
                {
                    startPartName = startPointPart == null ? null : startPointPart.name.Substring(0, startPointPart.name.Length - 7),
                    endPartName = endPointPart == null ? null : endPointPart.name.Substring(0, endPointPart.name.Length - 7),
                    endPosition = currentPointList[1],
                    state = IndicatorState.BORN
                });;
            }

            // 2022.7.28 已选中两个点并确认，关闭辅助选点
            myController.CmdUpdateSegmentPreSelect(new DPCPreSelect()
            {
                startPoint = new Vector3(),
                endPoint = new Vector3(),
                state = PreSelectMode.CLOSE
            });

            // 清空临时变量
            currentPointList.Clear();

        }
    }

    private void DeleteLastArrow()
    {

        Debug.Log("VR客户端发起删除线段请求");
        //myController.CmdDeleteSegmentInfo();
    }

    /// <summary>
    /// date:2022.7.21
    /// author:qinwen
    /// introduction:申请情况线段列表
    /// </summary>
    private void DeleteAllArrow()
    {
        myController.CmdDeleteAllArrow();
        Debug.Log("VR客户端发起删除所有线段的请求");
        indicatorController.UpdateIndicator(new DPCIndicator()
        {
            state = IndicatorState.DEAD
        });
        Debug.Log("VR客户端发起删除指示物的请求");
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

    /// <summary>
    /// 同时修改物体及其所有子物体层级
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeLayer(Transform transform, int layer)
    {
        if (transform.childCount > 0)//如果子物体存在
        {
            for (int i = 0; i < transform.childCount; i++)//遍历子物体是否还有子物体
            {
                ChangeLayer(transform.GetChild(i), layer);//这里是只将最后一个无子物体的对象设置层级
            }
            transform.gameObject.layer = layer;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
        }
        else					//无子物体
        {
            transform.gameObject.layer = layer;
        }
    }


    /// <summary>
    /// 同时修改物体及其所有子物体材质
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="layer"></param>
    private void ChangeMaterial(Transform transform, Material material)
    {
        if (transform.childCount > 0)//如果子物体存在
        {
            for (int i = 0; i < transform.childCount; i++)//遍历子物体是否还有子物体
            {
                ChangeMaterial(transform.GetChild(i), material);//这里是只将最后一个无子物体的对象设置层级
            }
            if (transform.GetComponent<MeshRenderer>())
            {
                transform.GetComponent<MeshRenderer>().material = material;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
            }
        }
        else					//无子物体
        {
            transform.GetComponent<MeshRenderer>().material = material;
        }
    }


    private void GetPartByHandleRay(GameObject vrHandle, ref GameObject currentPart)
    {
        // 流程1：再次发射射线检测碰撞物体，碰撞到物体后更新startPointPart
        Ray raycast = new Ray(vrHandle.transform.position, vrHandle.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit,Mathf.Infinity, (1<<15));
        if (!bHit)
        {
            currentPart = null;
            Debug.Log("VR端没有选中零件");
        }
        else
        {
            Debug.Log("选中的零件是: " + hit.collider.gameObject.name.Substring(0, hit.collider.gameObject.name.Length-7));
            currentPart = hit.collider.gameObject; // 本段代码核心

        }
    }




}
