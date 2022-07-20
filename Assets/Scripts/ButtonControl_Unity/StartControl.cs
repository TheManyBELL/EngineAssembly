using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartControl : MonoBehaviour
{
    private Button btn;
    public UnityEvent event2;

    void Awake()
    {
        // GameObject.Find("Diagnostics").SetActive(false);
        btn = this.gameObject.GetComponent<Button>();
        event2.AddListener(PlaceEnd);

    }

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(() => {
            event2.Invoke();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlaceEnd()
    {
        GameObject initScene = GameObject.Find("Scene/InitScene");
        initScene.SetActive(false);
        GameObject Plane = GameObject.Find("Scene/Plane");
        Plane.SetActive(false);
        GameObject firstGroup = GameObject.Find("Scene/Group1");
        GameObject firstSelectBox = firstGroup.transform.Find("SelectBox1").gameObject;
        firstSelectBox.SetActive(true);

        // GameObject.Find("CameraCapture").GetComponent<ComputeVisionTarget>().enabled = true;
        // GameObject.Find("CameraCapture").GetComponent<ComputeSquareCount>().enabled = true;
        GameObject.Find("ClickEventObject").GetComponent<ClickEvent>().StartAssembly = true;
    }
}
