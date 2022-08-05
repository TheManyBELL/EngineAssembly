using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    /// <summary>
    /// 按照装配顺序在inspector面板赋值的设备列表
    /// </summary>
    public List<GameObject> componentAssemblySequence;

    /// <summary>
    /// 设备列表，手动在inspector面板赋值
    /// 需要生成动画、添加标签、添加材质
    /// </summary>
    public List<GameObject> deviceList;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
