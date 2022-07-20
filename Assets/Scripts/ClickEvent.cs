using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/ClickEvent")]
public class ClickEvent : MonoBehaviour
{
    public int enable_index;
    public int enable_group;

    public int group_count;
    public int[] object_count;
    private const int MAX_GROUP = 100;
    private const int MAX_OBJECT = 100;

    public GameObject Scene;
    public bool StartAssembly;

    public bool receive;

    void Awake() {
        // GameObject.Find("Diagnostics").SetActive(false);
        group_count = 3;
        enable_index = 1;
        enable_group = 1;

        // objectCount = new int[MAX_GROUP];
        object_count = new int[4];
        object_count[1] = 6;
        object_count[2] = 13;
        object_count[3] = 1;

        StartAssembly = false;
        receive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update() {
        if (receive) {
            EnableNext();
            receive = false;
        }

    }

    public void EnableNext()
    {   
        // enable select box
        string group_name = "Scene/Group" + enable_group.ToString();
        GameObject group_obj = GameObject.Find(group_name);
        Debug.Log("group name " + group_name);

        string select_box_name = "SelectBox" + enable_index.ToString();
        GameObject now_box_obj = null;
        if (group_obj.transform.Find(select_box_name))
        {
            now_box_obj = group_obj.transform.Find(select_box_name).gameObject;
            now_box_obj.SetActive(false);
        }

        // change animation param
        int animation_index = 1;
        while (true)
        {
            string animation_play_name = "AnimationPlayObj" + animation_index.ToString();
            if (!group_obj.transform.Find(animation_play_name)) break;

            GameObject animation_play_obj = group_obj.transform.Find(animation_play_name).gameObject;
            Animator ator = animation_play_obj.GetComponent<Animator>();
            ator.SetInteger("AnimationIndex", enable_index + 1);
            ++animation_index;
        }
        

        enable_index += 1;
        bool change_group = false;
        if (enable_index > object_count[enable_group])
        {
            if (enable_group == group_count)
                enable_index = object_count[enable_group];
            else
            {
                ++enable_group;
                enable_index = 1;
                change_group = true;
            }
        }

        group_name = "Scene/Group" + enable_group.ToString();
        GameObject new_group_obj = GameObject.Find(group_name);

        select_box_name = "SelectBox" + enable_index.ToString();
        GameObject new_box_obj = null;
        if (new_group_obj.transform.Find(select_box_name))
        {
            new_box_obj = new_group_obj.transform.Find(select_box_name).gameObject;
            new_box_obj.SetActive(true);
        }

        if (change_group)
        {
            animation_index = 1;
            while (true)
            {
                string animation_play_name = "AnimationPlayObj" + animation_index.ToString();
                if (!new_group_obj.transform.Find(animation_play_name)) break;

                GameObject animation_play_obj = group_obj.transform.Find(animation_play_name).gameObject;
                Animator ator = animation_play_obj.GetComponent<Animator>();
                ator.SetInteger("AnimationIndex", enable_index);
                ++animation_index;
            }
        }
    }

    public void EnableStart()
    {
        GameObject initScene = GameObject.Find("Scene/InitScene");
        initScene.SetActive(false);
        GameObject Plane = GameObject.Find("Scene/Plane");
        Plane.SetActive(false);
        GameObject firstGroup = GameObject.Find("Scene/Group1");
        GameObject firstSelectBox = firstGroup.transform.Find("SelectBox1").gameObject;
        firstSelectBox.SetActive(true);

        // GameObject.Find("CameraCapture").GetComponent<ComputeSquareCount>().enabled = true;
        StartAssembly = true;
    }

    public void MoveLeft()
    {
        Scene.transform.position += new Vector3(-0.01f, 0.0f, 0.0f);
    }

    public void MoveRight() 
    {
        Scene.transform.position += new Vector3(0.01f, 0.0f, 0.0f);
    }

    public void MoveBack()
    {
        Scene.transform.position += new Vector3(0.0f, 0.0f, 0.01f);
    }

    public void MoveFront() {
        Scene.transform.position += new Vector3(0.0f, 0.0f, -0.01f);
    }

    public void MoveUp() {
        Scene.transform.position += new Vector3(0.0f, 0.01f, 0.0f);
    }

    public void MoveDown() {
        Scene.transform.position += new Vector3(0.0f, -0.01f, 0.0f);
    }

    public void RotateClockwise() {
        Scene.transform.eulerAngles += new Vector3(0.0f, 0.5f, 0.0f);
    }

    public void RotateNotclockwise() {
        Scene.transform.eulerAngles += new Vector3(0.0f, -0.5f, 0.0f);
    }

    public void Larger() {
        Scene.transform.localScale += new Vector3(0.01f, 0.0f, 0.01f);
    }

    public void Smaller() {
        Scene.transform.localScale += new Vector3(-0.01f, 0.0f, -0.01f);
    }

    public void Clean() {
        GameObject.Find("PressableButton/Left").SetActive(false);
        GameObject.Find("PressableButton/Right").SetActive(false);
        GameObject.Find("PressableButton/Back").SetActive(false);
        GameObject.Find("PressableButton/Front").SetActive(false);
        GameObject.Find("PressableButton/Up").SetActive(false);
        GameObject.Find("PressableButton/Down").SetActive(false);

        GameObject.Find("PressableButton/RotateClockwise").SetActive(false);
        GameObject.Find("PressableButton/NotRotateClockwise").SetActive(false);
        GameObject.Find("PressableButton/Larger").SetActive(false);
        GameObject.Find("PressableButton/Smaller").SetActive(false);

        GameObject.Find("PressableButton/Clean").SetActive(false);
    }
}
