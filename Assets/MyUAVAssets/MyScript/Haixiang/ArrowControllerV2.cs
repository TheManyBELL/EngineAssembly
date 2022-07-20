using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
public class ArrowControllerV2 : MonoBehaviour
{
    private static ArrowControllerV2 instance;

    public GameObject directionalArrowPrefab;
    public GameObject activeArrowPrefab;

    private static GameObject emptyTarget;

    //private static Vector3 targetPos = Vector3.zero;

    private static DirectionalIndicator directionalIndicator;
    private static ActiveIndicator activeIndicator;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        emptyTarget = new GameObject("Empty Target");

        directionalArrowPrefab = Instantiate(directionalArrowPrefab);
        activeArrowPrefab = Instantiate(activeArrowPrefab);

        directionalIndicator = instance.directionalArrowPrefab.GetComponent<DirectionalIndicator>();
        activeIndicator = instance.activeArrowPrefab.GetComponent<ActiveIndicator>();
    }

    public static void SetArrowPos(Vector3 pos)
    {
        emptyTarget.transform.position = pos;
        directionalIndicator.DirectionalTarget = emptyTarget.transform;
        activeIndicator.ActiveTarget = emptyTarget.transform;
    }

    public static void DisableArrow()
    {
        directionalIndicator.DirectionalTarget = null;
        activeIndicator.ActiveTarget = null;
    }
}
