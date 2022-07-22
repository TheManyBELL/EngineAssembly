using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAnimationController : MonoBehaviour
{
    private static TempAnimationController instance;
    public List<GameObject> animObjectList;

    private static int activeIndex = -1;
    private static int previousIndex = -1;
    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        PlayAnimation();
    }
    public static void PlayAnimation()
    {
        previousIndex = activeIndex;
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (activeIndex == -1) return;
            instance.animObjectList[activeIndex].GetComponent<AnimationGenerator>().ReplayAnimation();
            return;
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            activeIndex += 1;
            activeIndex %= instance.animObjectList.Count;
        }
        if (Input.GetKeyDown(KeyCode.PageDown) && activeIndex != -1)
        {
            activeIndex += instance.animObjectList.Count - 1;
            activeIndex %= instance.animObjectList.Count;
        }
        if (activeIndex == -1) return;
        if (previousIndex != -1 && previousIndex != activeIndex)
        {
            //Debug.Log("Stop Anim");
            instance.animObjectList[previousIndex].GetComponent<AnimationGenerator>().StopAnimation();
        }
        if (activeIndex != -1 && previousIndex != activeIndex)
        {
            //Debug.Log("Play Anim");
            instance.animObjectList[activeIndex].GetComponent<AnimationGenerator>().PlayAnimation();
        }
        UpdateArrowPosV2();
    }
    private static void UpdateArrowPos()
    {
        if (activeIndex == -1) return;
        ArrowController.SetArrowPos(instance.animObjectList[activeIndex].GetComponent<AnimationGenerator>().GetArrowPos());
    }

    private static void UpdateArrowPosV2()
    {
        if (activeIndex == -1) return;
        ArrowControllerV2.SetArrowPos(instance.animObjectList[activeIndex].GetComponent<AnimationGenerator>().GetArrowPos());
    }
}
