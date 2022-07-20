using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NextControl : MonoBehaviour
{
    // Start is called before the first frame update

    public int enable_index;
    public int enable_group;
    public int step;

    public UnityEvent event1;
    public int group_count;
    public int[] object_count;

    private const int MAX_GROUP = 100;
    private const int MAX_OBJECT = 100;

    private Button btn;

    void Awake()
    {
        group_count = 3;
        enable_index = 1;
        enable_group = 1;

        // objectCount = new int[MAX_GROUP];
        object_count = new int[4];
        object_count[1] = 6;
        object_count[2] = 13;
        object_count[3] = 1;

        btn = this.gameObject.GetComponent<Button>();
        event1.AddListener(EnableNext);
    }

    void Start()
    {
        btn.onClick.AddListener(() => {
            event1.Invoke();
        });

        // GameObject.Find("Diagnostics").SetActive(false);

        /*int group_i = 1, obj_i = 1;
        while (true)
        {
            obj_i = 1;
            string groupName = "Group" + group_i.ToString();
            GameObject tempGroup = GameObject.Find(groupName);
            if (!tempGroup)
                break;

            while (true)
            {
                string objName = "ObjectTarget" + obj_i.ToString();
                if (!tempGroup.transform.Find(objName))
                    break;
                obj_i++;
            }
            objectCount[group_i] = obj_i - 1;
            group_i++;
        }
        GroupCount = group_i - 1;*/
    }

    void Update()
    {

    }

    public void EnableNext()
    {
        /*int temp_enable_index = GameObject.Find("Canvas/Next").GetComponent<ButtonControl>().enable_index;
        int temp_enable_group = GameObject.Find("Canvas/Next").GetComponent<ButtonControl>().enable_group;*/
        
        // enable select box
        string group_name = "Scene/Group" + enable_group.ToString();
        GameObject group_obj = GameObject.Find(group_name);

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
        

        // 更新 index
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

        // enable 新的
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

}
