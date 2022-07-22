using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchAnimationGenerator : AnimationGenerator
{
    public List<GameObject> branchList;

    public override Vector3 GetArrowPos()
    {
        Vector3 vec = Vector3.zero;
        foreach (var branch in branchList)
        {
            vec += branch.GetComponent<AnimationGenerator>().GetArrowPos();
        }
        return vec / branchList.Count;
    }

    public override void PlayAnimation()
    {
        foreach (var branch in branchList)
        {
            //branch.GetComponent<MeshRenderer>().enabled = false;
            branch.GetComponent<AnimationGenerator>().PlayAnimation();
            generatedAnimList.AddRange(branch.GetComponent<AnimationGenerator>().generatedAnimList);
        }
    }

    public override void StopAnimation()
    {
        foreach (var branch in branchList)
        {
            //branch.GetComponent<MeshRenderer>().enabled = true;
            branch.GetComponent<AnimationGenerator>().StopAnimation();
        }
        generatedAnimList.Clear();
        //transform.parent = originParent;
    }
}
