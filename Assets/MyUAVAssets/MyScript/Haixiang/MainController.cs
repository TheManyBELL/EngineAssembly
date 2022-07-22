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

    /// <summary>
    /// 线束列表，手动在inspector面板赋值
    /// 需要添加标签并对标签进行管理
    /// </summary>
    public List<GameObject> waikeList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
