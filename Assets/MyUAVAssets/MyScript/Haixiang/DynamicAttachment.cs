using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class DynamicAttachment : MonoBehaviour
{
    // Start is called before the first frame update
    public List<ObiParticleGroup> particleGroups;
    void Start()
    {
        //InitRope();
        InitPA();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitPA()
    {
        GameObject leftAttachment = new GameObject("LeftAttachment");
        GameObject rightAttachment = new GameObject("RightAttachment");

        Vector3 min = gameObject.GetComponent<MeshRenderer>().bounds.min;
        Vector3 max = gameObject.GetComponent<MeshRenderer>().bounds.max;
        Vector3 center = gameObject.GetComponent<MeshRenderer>().bounds.center;
        Vector3 extents = gameObject.GetComponent<MeshRenderer>().bounds.extents;

        Transform tf = gameObject.GetComponent<MeshCollider>().transform;
        //Transform tf2 = gameObject.GetComponent<ObiSoftbody>().transform;
        leftAttachment.transform.parent = transform;
        rightAttachment.transform.parent = transform;
        leftAttachment.transform.position = max;
        rightAttachment.transform.position = min;
        //ObiActorBlueprint blueprint = gameObject.GetComponent<ObiSoftbody>().blueprint;
        //particleGroups = blueprint.groups;

        ObiParticleGroup start = particleGroups[0];
        ObiParticleGroup end = particleGroups[1];
        ObiParticleAttachment leftPA = gameObject.AddComponent<ObiParticleAttachment>();
        ObiParticleAttachment rightPA = gameObject.AddComponent<ObiParticleAttachment>();
        leftPA.target = leftAttachment.transform;
        rightPA.target = rightAttachment.transform;
        leftPA.particleGroup = start;
        rightPA.particleGroup = end;
    }
}
