using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using System;

public class XianshuOperation : MonoBehaviour
{
    public List<GameObject> xianshuList;
    public List<GameObject> groupList;
    private List<GameObject> xList;
    
    public float speed;
    public float stretchFactor;
    public GameObject simulationRope;
    public GameObject mainCamera;

    public float animaDuration = 30.0f;

    public int activeIndex = 0;
    private bool leftSelection = true; //true when operate leftAttachment, false when operating rightAttachment
    public bool enableAttachment = true;
    public bool enableShowAnimation = false;
    public bool enableRopeSimulation = true;
    public bool combineXianshu = false;

    private GameObject leftAttachment;
    private GameObject rightAttachment;
    private GameObject currentAttachment;

    private ObiRope rope;
    private ObiRopeCursor cursor;

    private GameObject leftControlPoint;
    private GameObject rightControlPoint;
    private GameObject currentControlPoint;

    private List<Animation> showAnimationList = new List<Animation>();
    public List<Animation> leftAnimationList = new List<Animation>();
    private List<Animation> rightAnimationList = new List<Animation>();
    private List<float> maxLengthList = new List<float>();



    // Start is called before the first frame update
    void Start()
    {
        xList = combineXianshu ? groupList : xianshuList;
        foreach (GameObject xianshu in xList)
        {
            if (!enableAttachment)
            {
                //xianshu.GetComponent<ObiSoftbody>().enabled = false;
                if (combineXianshu)
                {
                    for (int i = 0; i < xianshu.transform.childCount; i++)
                    {
                        xianshu.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else
                {
                    xianshu.GetComponent<MeshRenderer>().enabled = false;
                }
            }
            if (enableRopeSimulation)
            {
                RopeInit(xianshu);
                if (xianshu.GetComponent<PathList>() != null)
                {
                    Debug.Log("Add Xianshu Animation");
                    AddXianshuAnim(xianshu);
                }
                else
                {
                    showAnimationList.Add(null);
                    leftAnimationList.Add(null);
                    rightAnimationList.Add(null);
                    maxLengthList.Add(0);
                }
            }
            else
            {
                //showAnimationList.Add(null);
                //xianshuList[activeIndex].GetComponent<ObiSoftbody>().enabled = true;
                if (enableShowAnimation)
                {
                    AddShowAnim(xianshu);
                }
                leftAnimationList.Add(null);
                rightAnimationList.Add(null);
                maxLengthList.Add(0);
            }
        }
    }

    private void AddShowAnim(GameObject xianshu)
    {
        Animation animation = xianshu.AddComponent<Animation>();

        AnimationClip myClip = new AnimationClip();
        myClip.name = "AnimationClip: " + xianshu.name;
        myClip.legacy = true;
        myClip.wrapMode = WrapMode.ClampForever;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, 0.0f));
        curve.AddKey(new Keyframe(animaDuration, 1.0f));
        myClip.SetCurve("", typeof(DiscardTest), "ShowPercentage", curve);

        //curve = new AnimationCurve();
        //curve.AddKey(new Keyframe(0, 1.0f));
        //curve.AddKey(new Keyframe(animaDuration, 0.0f));
        //myClip.SetCurve("", typeof(ObiSoftbody), "enabled", curve);

        animation.AddClip(myClip, myClip.name);
        animation.clip = myClip;

        showAnimationList.Add(animation);
    }

    private void AddXianshuAnim(GameObject xianshu)
    {
        leftControlPoint = xianshu.transform.Find(simulationRope.name).Find("LeftControlPoint").gameObject;
        rightControlPoint = xianshu.transform.Find(simulationRope.name).Find("RightControlPoint").gameObject;

        List<Transform> leftList = xianshu.GetComponent<PathList>().leftList;
        List<Transform> rightList = xianshu.GetComponent<PathList>().rightList;

        //xianshu.AddComponent<Animation>();

        //AnimationClip myclip = new AnimationClip();
        //myclip.name = "AnimationClip: " + xianshu.name;
        //myclip.legacy = true;
        //myclip.wrapMode = WrapMode.Loop;

        //AnimationCurve curve = new AnimationCurve();
        Transform center = xianshu.transform.Find(simulationRope.name).transform;

        AddControlPointAnim(leftControlPoint, leftList);
        AddControlPointAnim(rightControlPoint, rightList);

        float leftLength = 0, rightLength = 0;
        for (int i = 1; i < leftList.Count; i++)
        {
            leftLength += (leftList[i].transform.position - leftList[i - 1].transform.position).magnitude;
        }
        for (int i = 1; i < rightList.Count; i++)
        {
            rightLength += (rightList[i].transform.position - rightList[i - 1].transform.position).magnitude;
        }
        maxLengthList.Add(leftLength + rightLength);
        rope = xianshu.transform.Find(simulationRope.name).gameObject.GetComponent<ObiRope>();
        cursor = xianshu.transform.Find(simulationRope.name).gameObject.GetComponent<ObiRopeCursor>();
        cursor.ChangeLength(rope.restLength + (leftLength + rightLength));
    }

    private void AddControlPointAnim(GameObject controlPoint, List<Transform> semiList)
    {
        foreach (Transform t in semiList)
        {
            t.transform.parent = controlPoint.transform.parent;
        }
        
        Animation animation = controlPoint.AddComponent<Animation>();

        AnimationClip myClip = new AnimationClip();
        myClip.name = "AnimationClip: " + controlPoint.name;
        myClip.legacy = true;
        myClip.wrapMode = WrapMode.ClampForever;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.x));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localPosition.x));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.y));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localPosition.y));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.y", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localPosition.z));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localPosition.z));
        }
        myClip.SetCurve("", typeof(Transform), "localPosition.z", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.x));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localRotation.x));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.x", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.y));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localRotation.y));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.y", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.z));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localRotation.z));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.z", curve);

        curve = new AnimationCurve();
        curve.AddKey(new Keyframe(0, controlPoint.transform.localRotation.w));
        for (int i = 0; i < semiList.Count; i++)
        {
            curve.AddKey(new Keyframe(animaDuration / semiList.Count * (i + 1), semiList[i].localRotation.w));
        }
        myClip.SetCurve("", typeof(Transform), "localRotation.w", curve);

        animation.AddClip(myClip, myClip.name);
        animation.clip = myClip;

        //animationList.Add(animation);
        if (controlPoint.name.Equals("LeftControlPoint"))
        {
            leftAnimationList.Add(animation);
        }
        else
        {
            rightAnimationList.Add(animation);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            activeIndex += 1;
            activeIndex %= xList.Count;
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            activeIndex += xList.Count - 1;
            activeIndex %= xList.Count;
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            leftSelection = true;
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            leftSelection = false;
        }

        if (leftAnimationList[activeIndex] != null)
        {
            leftAnimationList[activeIndex].Play();
            rightAnimationList[activeIndex].Play();

            //rope = xianshuList[activeIndex].transform.Find(simulationRope.name).gameObject.GetComponent<ObiRope>();
            //cursor = xianshuList[activeIndex].transform.Find(simulationRope.name).gameObject.GetComponent<ObiRopeCursor>();
            //cursor.ChangeLength((leftControlPoint.transform.position - rightControlPoint.transform.position).magnitude);
            //cursor.ChangeLength((leftControlPoint.transform.position - rightControlPoint.transform.position).magnitude * 2.5f);
        }
        else
        {
            if (enableAttachment)
            {
                if (enableShowAnimation)
                {
                    showAnimationList[activeIndex].Play();
                }
                else
                {
                    AttachmentOperation();
                }
            }
            if (enableRopeSimulation)
            {
                RopeOperation();
            }
        }
    }
    private void RopeInit(GameObject xianshu)
    {
        Vector3 center = Vector3.one;
        if (!combineXianshu)
        {
            center = xianshu.GetComponent<MeshRenderer>().bounds.center;
        }
        if (xianshu.transform.Find("RopeStart") != null)
        {
            center = xianshu.transform.Find("RopeStart").transform.position;
        }
        GameObject ropeObj = GameObject.Instantiate(simulationRope, center, Quaternion.identity, xianshu.transform);
        //GameObject ropeObj = GameObject.Instantiate(gameObject, center, Quaternion.identity, xianshu.transform);
        ropeObj.name = simulationRope.name;

        //rope = ropeObj.GetComponent<ObiRope>();
        //cursor = ropeObj.GetComponent<ObiRopeCursor>();

        //leftControlPoint = ropeObj.transform.Find("LeftControlPoint").gameObject;
        //rightControlPoint = ropeObj.transform.Find("RightControlPoint").gameObject;
        //cursor.ChangeLength((leftControlPoint.transform.position - rightControlPoint.transform.position).magnitude);
    }

    private void AttachmentOperation()
    {
        leftAttachment = xList[activeIndex].transform.Find("LeftAttachment").gameObject;
        rightAttachment = xList[activeIndex].transform.Find("RightAttachment").gameObject;
        currentAttachment = leftSelection ? leftAttachment : rightAttachment;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentAttachment.transform.Translate(mainCamera.transform.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentAttachment.transform.Translate(-mainCamera.transform.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentAttachment.transform.Translate(mainCamera.transform.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            currentAttachment.transform.Translate(-mainCamera.transform.up * speed * Time.deltaTime);
        }
    }

    private void RopeOperation()
    {
        leftControlPoint = xList[activeIndex].transform.Find(simulationRope.name).Find("LeftControlPoint").gameObject;
        rightControlPoint = xList[activeIndex].transform.Find(simulationRope.name).Find("RightControlPoint").gameObject;
        currentControlPoint = leftSelection ? leftControlPoint : rightControlPoint;

        rope = xList[activeIndex].transform.Find(simulationRope.name).gameObject.GetComponent<ObiRope>();
        cursor = xList[activeIndex].transform.Find(simulationRope.name).gameObject.GetComponent<ObiRopeCursor>();

        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentControlPoint.transform.Translate(mainCamera.transform.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentControlPoint.transform.Translate(-mainCamera.transform.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            currentControlPoint.transform.Translate(mainCamera.transform.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            currentControlPoint.transform.Translate(-mainCamera.transform.up * speed * Time.deltaTime);
        }
        cursor.ChangeLength((leftControlPoint.transform.position - rightControlPoint.transform.position).magnitude);
    }
}
