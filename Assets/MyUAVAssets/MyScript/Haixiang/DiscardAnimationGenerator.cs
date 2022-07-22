using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardAnimationGenerator : AnimationGenerator
{
    public Vector3 direction = new Vector3(0, 1, 0);
    
    public float durationTime = 5.0f;
    private Material m;

    public float ShowPercentage = 0.0f;
    private Vector3 position;
    private float max;
    public override Vector3 GetArrowPos()
    {
        return gameObject.GetComponent<MeshRenderer>().bounds.center;
    }
    public void GenerateAnimation()
    {
        m = gameObject.GetComponent<MeshRenderer>().material;
        position = GetComponent<MeshRenderer>().bounds.center;
        direction = GameObject.Find("HoloLensStyleBounds").transform.rotation * direction;
        direction = direction.normalized;
        max = Vector3.Dot(GetComponent<MeshRenderer>().bounds.extents, direction);
        max = Mathf.Abs(max);
        
        //if (Mathf.Abs(direction.x) < 1e-5)
        //    direction.x = 0;
        //if (Mathf.Abs(direction.y) < 1e-5)
        //    direction.y = 0;
        //if (Mathf.Abs(direction.z) < 1e-5)
        //    direction.z = 0;

        AddDiscardAnim();
    }

    private void AddDiscardAnim()
    {
        Animation anim = gameObject.AddComponent<Animation>();

        AnimationClip myClip = new AnimationClip();
        myClip.name = "AnimationClip: " + gameObject.name;
        myClip.legacy = true;
        myClip.wrapMode = WrapMode.Once;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, 0.0f));
        curve.AddKey(new Keyframe(durationTime, 1.0f));
        myClip.SetCurve("", typeof(DiscardAnimationGenerator), "ShowPercentage", curve);

        anim.AddClip(myClip, myClip.name);
        anim.clip = myClip;

        generatedAnimList.Add(anim);
    }
    public override void PlayAnimation()
    {
        ShowPercentage = 0;
        foreach (var anim in generatedAnimList)
        {
            anim.Play();
        }
    }

    public override void StopAnimation()
    {
        foreach (var anim in generatedAnimList)
        {
            anim.Stop();
        }
        ShowPercentage = 1.0f;
    }

    void Awake()
    {
        GenerateAnimation();
        m.SetVector("_Position", position);
        m.SetFloat("_Max", max);
        m.SetFloat("_ShowPercent", ShowPercentage);
        //Debug.Log(max);
        m.SetVector("_Direction", direction);
    }

    // Update is called once per frame
    void Update()
    {
        m.SetVector("_Position", position);
        m.SetFloat("_Max", max);
        m.SetFloat("_ShowPercent", ShowPercentage);
        //Debug.Log(max);
        m.SetVector("_Direction", direction);
    }
}
