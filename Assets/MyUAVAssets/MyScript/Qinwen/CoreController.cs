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
    /// ��inspector������룬�����û��ƶ��Ŀ������
    /// </summary>
    public GameObject FollowSolverPanel;

    private AssemblyProgressPanelCotrol assemblyProgress;

    /// <summary>
    /// author:qinwen
    /// note:������Ŀ��������
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
        // ͨ��-��=������������װ�䶯������
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
    /// ������һ��װ��
    /// </summary>
    public void OnPressNextButton()
    {
        Debug.Log("������ ��һ�� ����");
        // ����curSN������ܷ���һ��
        // deviceController����curSN�رյ�ǰ�豸������һ���豸
        // labelController����curSN�رյ�ǰ��ǩ������һ����ǩ
        // animaController����curSN�رյ�ǰ����������һ������
        int curSN = assemblyInfo.curSN;
        if (curSN == assemblyInfo.componentAssemblySequence.Count)
        {
            Debug.LogWarning("�Ѿ����װ��!");
            return;
        }
        
        int nextSN = curSN + 1;

        
        // �رյ�ǰ����
        if (curSN != -1)
        {
            GameObject curComponent = assemblyInfo.componentAssemblySequence[curSN];
            if (curComponent.GetComponent<AnimationGenerator>())
            {
                curComponent.GetComponent<AnimationGenerator>().StopAnimation();
            }
        }
        // ����һ������
        if(nextSN != assemblyInfo.componentAssemblySequence.Count)
        {
            GameObject nextComponent = assemblyInfo.componentAssemblySequence[nextSN];
            nextComponent.SetActive(true);
            //SetVisibility(nextComponent, true);
            if (nextComponent.GetComponent<AnimationGenerator>())
            {
                nextComponent.GetComponent<AnimationGenerator>().PlayAnimation();
            }
            // �������
            assemblyProgress.AddNewProgressInfo(nextSN + 1, nextComponent.name);
        }

        assemblyInfo.curSN = nextSN;
    }

    public void OnPressBackButton()
    {
        Debug.Log("������ ��һ�� ����");
        int curSN = assemblyInfo.curSN;
        if (curSN == -1)
        {
            Debug.LogWarning("�Ѿ��ص���װ���ʼ״̬!");
            return;
        }

        int backSN = curSN - 1;
        // �رյ�ǰ����
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
        // ����һ������
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
        if (!GameObject.Find("������(clone)")) return;

        ator = GameObject.Find("������(clone)").GetComponent<Animator>();
        int current = ator.GetInteger("AnimationIndex");
        current++;
        ator.SetInteger("AnimationIndex", current);
        
        ator = GameObject.Find("������֧��(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("��ѹ��������(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("����(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("���(��)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("���(С)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
    }

    public void OnPressBackButtonNew()
    {
        Animator ator = null;
        if (!GameObject.Find("������(clone)")) return;

        ator = GameObject.Find("������(clone)").GetComponent<Animator>();
        int current = ator.GetInteger("AnimationIndex");
        current--;
        ator.SetInteger("AnimationIndex", current);

        ator = GameObject.Find("������֧��(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("��ѹ��������(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("����(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("���(��)(clone)").GetComponent<Animator>();
        ator.SetInteger("AnimationIndex", current);
        ator = GameObject.Find("���(С)(clone)").GetComponent<Animator>();
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
    /// ����װ����������
    /// </summary>
    


}
