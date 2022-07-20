using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class XianshuAnimationGenerator : AnimationGenerator
{
    public GameObject ropePrefab;
    public bool isMain = false;
    public float lengthOffset = 0;
    public Transform startPos;
    public List<Transform> LPathList;
    public List<Transform> RPathList;
    public float durationTime = 30;

    private GameObject ropeObj;

    //public List<Transform> generateAnimationList;
    // Start is called before the first frame update
    private void Awake()
    {
        RopeInit();
        GenerateAnimation();
    }

    private void RopeInit()
    {
        Transform t = GameObject.Find("HoloLensStyleBounds").transform;
        ropeObj = GameObject.Instantiate(ropePrefab, startPos.position, t.rotation, gameObject.transform);
        Debug.Log(ropeObj.transform.rotation.eulerAngles);
        ropeObj.name = ropePrefab.name;
        //ropeObj.transform.rotation = startPos.rotation;
    }

    private void RopeCopy()
    {
        Destroy(ropeObj);
    }

    private void SetRopeLength()
    {
        float leftLength = 0, rightLength = 0;
        for (int i = 1; i < LPathList.Count; i++)
        {
            leftLength += (LPathList[i].transform.position - LPathList[i - 1].transform.position).magnitude;
        }
        for (int i = 1; i < RPathList.Count; i++)
        {
            rightLength += (RPathList[i].transform.position - RPathList[i - 1].transform.position).magnitude;
        }
        ObiRope rope = ropeObj.GetComponent<ObiRope>();
        ObiRopeCursor cursor = ropeObj.GetComponent<ObiRopeCursor>();
        cursor.ChangeLength(rope.restLength + leftLength + rightLength + lengthOffset);
        rope.RecalculateRestLength();
    }
    public void GenerateAnimation()
    {
        //RopeInit();
        GameObject leftControlPoint = ropeObj.transform.Find("LeftControlPoint").gameObject;
        GameObject rightControlPoint = ropeObj.transform.Find("RightControlPoint").gameObject;

        if (LPathList != null)
            GenerateControlPointAnimation(leftControlPoint, LPathList);
        if (RPathList != null)
            GenerateControlPointAnimation(rightControlPoint, RPathList);
        
        //ropeObj.SetActive(false);
        SetSelfVisibility(ropeObj, false);
    }

    private void GenerateControlPointAnimation(GameObject controlPoint, List<Transform> pathList)
    {

        List<Transform> pathListCal = new List<Transform>();
        foreach (Transform t in pathList)
        {
            GameObject go = Instantiate<GameObject>(t.gameObject, t.position, t.rotation, controlPoint.transform.parent);
            //go.transform.parent = controlPoint.transform.parent;
            pathListCal.Add(go.transform);
        }

        Animation animation = controlPoint.AddComponent<Animation>();

        AnimationClip myClip = new AnimationClip();
        myClip.name = "AnimationClip: " + controlPoint.name;
        myClip.legacy = true;
        myClip.wrapMode = WrapMode.Once;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.x));

        int len = pathList.Count;
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localPosition.x));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.y));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localPosition.y));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.y", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.z));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localPosition.z));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.z", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.x));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localRotation.x));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.x", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.y));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localRotation.y));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.y", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.z));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localRotation.z));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.z", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.w));
        for (int i = 0; i < len; i++)
        {
            curve.AddKey(new Keyframe(durationTime / pathList.Count * (i + 1), pathListCal[i].localRotation.w));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.w", curve);

        animation.AddClip(myClip, myClip.name);
        animation.clip = myClip;

        generatedAnimList.Add(animation);
    }

    private void SetSelfVisibility(GameObject obj, bool val)
    {
        foreach (var renderer in obj.GetComponents<Renderer>())
        {
            renderer.enabled = val;
        }
    }
    private void SetBranchVisibility(bool val)
    {
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            if (transform.parent.GetChild(i).gameObject.name.Equals(gameObject.name)) continue;
            foreach (var renderer in transform.parent.GetChild(i).gameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer.gameObject.name.Contains("SimuRope")) continue;
                renderer.enabled = val;
            }
            //transform.parent.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    public override void PlayAnimation()
    {
        if (!ropeObj)
        {
            Debug.Log("init in play: " + gameObject.name);
            RopeInit();
            GenerateAnimation();
        }
        else
        {
            Debug.Log("have ropeObj: " + gameObject.name);
        }
        //Debug.Log("Play");
        if (isMain)
        {
            //SetBranchVisibility(false);
        }
        else
        {
            gameObject.GetComponent<Renderer>().enabled = false;
        }
        Debug.Log(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name.Equals(ropeObj.name)) continue;
            transform.GetChild(i).gameObject.SetActive(false);
        }
        //ropeObj.SetActive(true);
        SetSelfVisibility(ropeObj, true);
        //ropeObj.GetComponent<ObiRopeExtrudedRenderer>().enabled = true;
        SetRopeLength();
        //Debug.Log(gameObject.name + "Xianshu Animation Play");
        foreach (var animation in generatedAnimList)
        {
            animation.Play();
        }
    }

    public override void StopAnimation()
    {
        if (isMain)
        {
            //SetBranchVisibility(true);
        }
        else
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        foreach (var animation in generatedAnimList)
        {
            animation.Stop();
        }
        generatedAnimList.Clear();

        SetSelfVisibility(ropeObj, false);
        Destroy(ropeObj);
        //RopeInit();
        //GenerateAnimation();
    }

    public override Vector3 GetArrowPos()
    {
        return ropeObj.GetComponent<MeshRenderer>().bounds.center;
    }
}
