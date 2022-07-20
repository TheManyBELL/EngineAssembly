using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 direction = new Vector3(0, 1, 0);
    public float ShowPercentage = 0.0f;
    private Material m;

    private Vector3 position;
    private float max;
    void Start()
    {
        m = GetComponent<SkinnedMeshRenderer>().material;
        Debug.Log("Use SkinnedMeshRenderer");
        position = GetComponent<SkinnedMeshRenderer>().bounds.center;
        max = Vector3.Dot(GetComponent<SkinnedMeshRenderer>().bounds.extents, direction);
        //if (GetComponent<SkinnedMeshRenderer>() != null)
        //{
            
        //else
        //{
        //    m = GetComponent<MeshRenderer>().material;
        //    position = transform.position;
        //}
        //Debug.Log(m.name);
        direction = direction.normalized;
    }
    void Update()
    {
        m.SetVector("_Position", position);
        m.SetFloat("_Max", max);
        m.SetFloat("_ShowPercent", ShowPercentage);
        Debug.Log(max);
        m.SetVector("_Direction", direction);
    }
}
