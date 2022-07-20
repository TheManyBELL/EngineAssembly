using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationGenerator : MonoBehaviour
{
    public List<Animation> generatedAnimList = new List<Animation>();

    //public abstract void GenerateAnimation();

    public abstract void PlayAnimation();

    public abstract void StopAnimation();

    public void ReplayAnimation()
    {
        StopAnimation();
        PlayAnimation();
    }

    public abstract Vector3 GetArrowPos();
}
