using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;


public class CoreController : MonoBehaviour
{
    //public Obi.ObiSolver solver;

    private AnimationControllerNew animaController;
    private DeviceController deviceController;

    private HoloLabelController holoLabelController;
    private AssemblyInfo assemblyInfo;

    /// <summary>
    /// 在inspector面板拖入，跟随用户移动的控制面板
    /// </summary>
    public GameObject FollowSolverPanel;

    private AssemblyProgressPanelCotrol assemblyProgress;

    /// <summary>
    /// author:qinwen
    /// note:引擎项目新增代码
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("CoreController");

        //deviceController = GetComponent<DeviceController>();
        //animaController = GetComponent<AnimationControllerNew>();
        //holoLabelController = GetComponent<HoloLabelController>();
        assemblyInfo = GetComponent<AssemblyInfo>();

        //assemblyProgress = GetComponent<AssemblyProgressPanelCotrol>();


    }

    // Update is called once per frame
    void Update()
    {
        // author: haixiang
        // 通过-和=两个按键控制装配动画播放
        //if (Input.GetKeyDown(KeyCode.Minus))
        //{
        //    OnPressBackButton();
        //}
        //if (Input.GetKeyDown(KeyCode.Equals))
        //{
        //    OnPressNextButton();
        //}
        //UpdateArrowPosV2();
    }

    private void SetVisibility(GameObject go, bool val)
    {
        foreach (var renderer in go.GetComponents<Renderer>())
        {
            renderer.enabled = val;
        }
        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = val;
        }
    }

    /// <summary>
    /// 进入下一步装配
    /// </summary>
    public void OnPressNextButton()
    {
        Debug.Log("按下了 下一步 按键");
        // 更新curSN，检查能否下一步
        // deviceController根据curSN关闭当前设备，打开下一个设备
        // labelController根据curSN关闭当前标签，打开下一个标签
        // animaController根据curSN关闭当前动画，打开下一个动画
        int curSN = assemblyInfo.curSN;
        if (curSN == assemblyInfo.componentAssemblySequence.Count)
        {
            Debug.LogWarning("已经完成装配!");
            return;
        }
        
        int nextSN = curSN + 1;

        
        // 关闭当前动画
        if (curSN != -1)
        {
            GameObject curComponent = assemblyInfo.componentAssemblySequence[curSN];
            if (curComponent.GetComponent<AnimationGenerator>())
            {
                curComponent.GetComponent<AnimationGenerator>().StopAnimation();
            }
        }
        // 打开下一个动画
        if(nextSN != assemblyInfo.componentAssemblySequence.Count)
        {
            GameObject nextComponent = assemblyInfo.componentAssemblySequence[nextSN];
            nextComponent.SetActive(true);
            //SetVisibility(nextComponent, true);
            if (nextComponent.GetComponent<AnimationGenerator>())
            {
                nextComponent.GetComponent<AnimationGenerator>().PlayAnimation();
            }
            // 更新面板
            assemblyProgress.AddNewProgressInfo(nextSN + 1, nextComponent.name);
        }

        assemblyInfo.curSN = nextSN;
    }

    public void OnPressBackButton()
    {
        Debug.Log("按下了 上一步 按键");
        int curSN = assemblyInfo.curSN;
        if (curSN == -1)
        {
            Debug.LogWarning("已经回到了装配初始状态!");
            return;
        }

        int backSN = curSN - 1;
        // 关闭当前物体
        if(curSN != assemblyInfo.componentAssemblySequence.Count)
        {
            GameObject curComponent = assemblyInfo.componentAssemblySequence[curSN];
            //SetVisibility(curComponent, false);
            if (curComponent.GetComponent<AnimationGenerator>())
            {
                curComponent.GetComponent<AnimationGenerator>().StopAnimation();
            }
            //SetVisibility(curComponent, false);
            curComponent.SetActive(false);

        }
        // 打开上一个物体
        if (backSN != -1)
        {
            GameObject backComponent = assemblyInfo.componentAssemblySequence[backSN];
            backComponent.SetActive(true);
            //SetVisibility(backComponent, true);
            if (backComponent.GetComponent<AnimationGenerator>())
            {
                backComponent.GetComponent<AnimationGenerator>().PlayAnimation();
            }
            assemblyProgress.AddNewProgressInfo(backSN+1, backComponent.name);
        }
       
        assemblyInfo.curSN = backSN;
    }

    public void OnPressNextButtonNew()
    {
        Animator ator = null;
        if (!GameObject.Find("风机外壳(clone)")) return;

        ator = GameObject.Find("风机外壳(clone)").GetComponent<Animator>();
        int current = ator.GetInteger("AnimationIndex");
        current++;
        ator.SetInteger("AnimationIndex", current);
        
        ator = GameObject.Find("风机外壳支架(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("高压涡轮主体(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("风扇(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("轴承(大)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("轴承(小)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
    }

    public void OnPressBackButtonNew()
    {
        Animator ator = null;
        if (!GameObject.Find("风机外壳(clone)")) return;

        ator = GameObject.Find("风机外壳(clone)").GetComponent<Animator>();
        int current = ator.GetInteger("AnimationIndex");
        current--;
        ator.SetInteger("AnimationIndex", current);

        ator = GameObject.Find("风机外壳支架(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("高压涡轮主体(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("风扇(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("轴承(大)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("轴承(小)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
    }

    public void OnPressPinButton()
    {
        Follow follow = FollowSolverPanel.GetComponent<Follow>();
        follow.enabled = !follow.enabled;
    }

    public void OnDistributedLabelButton()
    {
        holoLabelController.LabelLayoutAlgorithm();
    }

    private void UpdateArrowPosV2()
    {
        int curSN = assemblyInfo.curSN;
        if (curSN == -1) return;
        if (curSN == assemblyInfo.componentAssemblySequence.Count)
        {
            ArrowControllerV2.DisableArrow();
            return;
        }
        GameObject curComponent = assemblyInfo.componentAssemblySequence[curSN];
        ArrowControllerV2.SetArrowPos(curComponent.GetComponent<AnimationGenerator>().GetArrowPos());
    }

    /// <summary>
    /// author: qinwen
    /// 引擎装配新增代码
    /// </summary>
    


}
