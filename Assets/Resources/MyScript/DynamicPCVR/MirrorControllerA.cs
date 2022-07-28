using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorControllerA : NetworkBehaviour
{
    [Header("Client Mode Controller")]
    [Tooltip("SmartSign的两个子物体VR和AR")]
    public GameObject VRController;
    public GameObject ARController;


    /// <summary>
    /// 需要同步的标识列表
    /// </summary>
    public readonly SyncList<DPCArrow> syncArrowList = new SyncList<DPCArrow>();
    public readonly SyncList<DPCSymbol> syncRotationList = new SyncList<DPCSymbol>();
    public readonly SyncList<DPCSymbol> syncPressList = new SyncList<DPCSymbol>();

    // 需要同步的变量
    [SyncVar]
    public DPCIndicator auxiliaryIndicator = new DPCIndicator() { name = null,position = new Vector3() };

    // 线段指示点信息
    [SyncVar]
    public DPCPreSelect segmentPreSelect = new DPCPreSelect() { startPoint = new Vector3(), endPoint = new Vector3(), state = PreSelectMode.CLOSE };


    #region command

    /// <summary>
    /// 2022.7.28
    /// qinwen
    /// </summary>
    /// <param name="newSegmentPreSelect"></param>
    [Command]
    public void CmdUpdateSegmentPreSelect(DPCPreSelect newSegmentPreSelect)
    {
        segmentPreSelect = newSegmentPreSelect;
        Debug.Log("[server] newSegmentPreSelect updated");
    }

    /// <summary>
    /// 2022.7.21
    /// qinwen
    /// </summary>
    /// <param name="newIndicator"></param>
    [Command]
    public void CmdUpdateDPCIndicator(DPCIndicator newIndicator)
    {
        auxiliaryIndicator = newIndicator;
        Debug.Log("[server] newIndicator updated");
    }

    /// <summary>
    /// VR client add new arrow
    /// </summary>
    /// <param name="newArrow"></param>
    [Command]
    public void CmdAddDPCArrow(DPCArrow newArrow)
    {        
        syncArrowList.Add(newArrow);
        
        Debug.Log("[server] arrow added:" + syncArrowList.Count);
        
    }

    [Command]
    public void CmdUpdateDPCArrow(DPCArrow newArrow)
    {
        syncArrowList[newArrow.index] = newArrow;
        Debug.Log("[server] arrow " + newArrow.index + " updated");
    }

    [Command]
    public void CmdAddDPCRotation(DPCSymbol newRotation)
    {
        syncRotationList.Add(newRotation);

        Debug.Log("[server] arrow added:" + syncRotationList.Count);
    }

    [Command]
    public void CmdUpdateDPCRotation(DPCSymbol newRotation)
    {
        syncRotationList[newRotation.index] = newRotation;
        Debug.Log("[server] rotation "+newRotation.index+" updated");
    }

    [Command]
    public void CmdAddDPCPress(DPCSymbol newPress)
    {
        syncPressList.Add(newPress);
        Debug.Log("server: arrow added:" + syncPressList.Count);

    }

    [Command]
    public void CmdUpdateDPCPress(DPCSymbol newPress)
    {
        syncPressList[newPress.index] = newPress;
        Debug.Log("[server] press " + newPress.index + " updated");
    }

    /// <summary>
    /// date:2022.7.21
    /// author:qinwen
    /// introduction:清理线段列表
    /// </summary>
    [Command]
    public void CmdDeleteAllArrow()
    {
        syncArrowList.Clear();
        Debug.Log("server: arrow cleaned");
    }


    #endregion

    void Awake()
    {

        Debug.Log("Smart Sign Start");

        // control subobject according to Client Mode
        if (GlobleInfo.ClientMode.Equals(CameraMode.VR))
        {
            VRController.SetActive(true);
            ARController.SetActive(false);
        }
        if (GlobleInfo.ClientMode.Equals(CameraMode.AR))
        {
            VRController.SetActive(false);
            ARController.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        syncArrowList.Callback += OnDPCArrowUpdated;
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnDPCArrowUpdated(SyncList<DPCArrow>.Operation op, int index,DPCArrow oldItem ,DPCArrow newItem)
    {
        switch (op)
        {
            case SyncList<DPCArrow>.Operation.OP_ADD:
                // index is where it was added into the list
                // newItem is the new DPCArrow
                break;
            case SyncList<DPCArrow>.Operation.OP_INSERT:
                // index is where it was inserted into the list
                // newItem is the new DPCArrow
                break;
            case SyncList<DPCArrow>.Operation.OP_REMOVEAT:
                // index is where it was removed from the list
                // oldItem is the DPCArrow that was removed
                break;
            case SyncList<DPCArrow>.Operation.OP_SET:
                // index is of the DPCArrow that was changed
                // oldItem is the previous value for the DPCArrow at the index
                // newItem is the new value for the DPCArrow at the index
                break;
            case SyncList<DPCArrow>.Operation.OP_CLEAR:
                // list got cleared
                break;
        }
    }

    void printArrowList()
    {
        int n_arrow = syncArrowList.Count;
        if (n_arrow == 0)
        {
            Debug.Log("No arrow in list now");
        }
        for(int i = 0; i < n_arrow; ++i)
        {
            DPCArrow temp = syncArrowList[i];
            int n_pointOfArrow = temp.curvePointList.Count;
            Debug.Log("id:" + temp.index + ",point list:");
            for(int j = 0; j < n_pointOfArrow; ++j)
            {
                Debug.Log("  " + temp.curvePointList[j]);
            }
        }
    }
}
