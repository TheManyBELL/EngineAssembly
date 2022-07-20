using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowController : MonoBehaviour
{
    private static ArrowController instance;
    
    public GameObject screenArrowPrefab;
    public Canvas canvas;
    public int screenArrowOffset = 90;
    
    public GameObject worldArrowPrefab;
    public float worldArrowOffsetY = 1.0f;

    private GameObject currentWorldArrow;
    private GameObject currentScreenArrow;

    public GameObject testXianshu;
    private Vector3 arrowPos;

    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        arrowPos = testXianshu.GetComponent<MeshRenderer>().bounds.center;
        InitArrow();
    }

    private void Update()
    {
        UpdateArrow(arrowPos);
    }

    private static bool isInView(Vector3 worldPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1) return true; //只做x方向的判断
        else return false;
    }

    private static bool isInFrontOfView(Vector3 worldPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector2 viewPos = Camera.main.WorldToViewportPoint(worldPos);
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面

        return dot > 0;
    }

    public static void InitArrow()
    {
        // 生成两个箭头，控制其中一个处于enable状态
        instance.currentWorldArrow = GameObject.Instantiate(instance.worldArrowPrefab);
        //currentWorldArrow.transform.position = t.transform.position + new Vector3(0, worldArrowOffsetY, 0);
        instance.currentWorldArrow.SetActive(false);

        instance.currentScreenArrow = GameObject.Instantiate(instance.screenArrowPrefab, instance.canvas.transform);
        //Debug.Log(instance.canvas.transform.position);
        instance.currentScreenArrow.transform.parent = instance.canvas.transform;
        instance.currentScreenArrow.SetActive(false);
    }

    private static Vector2 ViewPointToCanvasPoint(Vector2 vec)
    {
        Vector2 ret = new Vector2();
        var rect = instance.canvas.GetComponent<RectTransform>().rect;
        ret.x = rect.width * vec.x - rect.width * 0.5f;
        ret.y = rect.height * vec.y - rect.height * 0.5f;
        return ret;
        //Vector2 basePos = new Vector2(-instance.canvas.GetComponent<RectTransform>().rect.width, -instance.canvas.GetComponent<RectTransform>().rect.height);
        //return basePos + new Vector2(vec.x * instance.canvas.GetComponent<RectTransform>().rect.width, vec.y * instance.canvas.GetComponent<RectTransform>().rect.height);
        //return new Vector2(vec.x * instance.canvas.GetComponent<RectTransform>().rect.width, vec.y * instance.canvas.GetComponent<RectTransform>().rect.height);
    }
    public static void UpdateArrow(Vector3 pos)
    {
        if (isInView(pos))
        {
            instance.currentWorldArrow.SetActive(true);
            instance.currentScreenArrow.SetActive(false);
            instance.currentWorldArrow.transform.position = pos + new Vector3(0, instance.worldArrowOffsetY, 0);
            var rotation = Quaternion.LookRotation(Camera.main.transform.TransformVector(Vector3.forward), Camera.main.transform.TransformVector(Vector3.up));
            rotation = new Quaternion(0, rotation.y, 0, rotation.w);
            instance.currentWorldArrow.transform.rotation = rotation;
            //instance.currentWorldArrow.transform.Rotate(new Vector3(0, 0, 90), Space.Self);
        }
        else
        {
            instance.currentWorldArrow.SetActive(false);
            instance.currentScreenArrow.SetActive(true);
            Transform camTransform = Camera.main.transform;
            Vector2 viewPos = Camera.main.WorldToViewportPoint(pos);
            Vector3 dir = (pos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);//判断物体是否在相机前面
            if (dot < 0)
            {
                viewPos.x = 1.0f-viewPos.x;
                viewPos.y = 1.0f-viewPos.y;
            }
            
            //Vector2 baseMax = new Vector2(instance.canvas.pixelRect.width / 2, instance.canvas.pixelRect.height / 2);
             Vector2 vec = ViewPointToCanvasPoint(new Vector2(viewPos.x >= 0.5f ? 1 : 0, (viewPos.y - 0.5f) * 0.5f / Mathf.Abs(viewPos.x - 0.5f) + 0.5f));
            //Vector2 vec = ViewPointToCanvasPoint(new Vector2(viewPos.x >= 0.5f ? 1 : 0, (viewPos.y - 0.5f) * 0.5f / Mathf.Abs(viewPos.x - 0.5f)));

            if (vec.y > instance.canvas.GetComponent<RectTransform>().rect.height / 2 - instance.screenArrowOffset)
            {
                vec.y = instance.canvas.GetComponent<RectTransform>().rect.height / 2 - instance.screenArrowOffset;
            }
            else if (vec.y < - instance.canvas.GetComponent<RectTransform>().rect.height / 2 + instance.screenArrowOffset)
            {
                vec.y = -instance.canvas.GetComponent<RectTransform>().rect.height / 2 + instance.screenArrowOffset;
            }

            //Debug.Log(vec);

            if (viewPos.x >= 0.5f)
            {
                vec.x -= instance.screenArrowOffset;
                instance.currentScreenArrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 180);
                //instance.currentScreenArrow.GetComponent<Text>().text = "▶";
            }
            else
            {
                vec.x += instance.screenArrowOffset;
                instance.currentScreenArrow.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                //instance.currentScreenArrow.GetComponent<Text>().text = "◀";
            }

            instance.currentScreenArrow.transform.localPosition = vec;
        }
    }

    public static void SetArrowPos(Vector3 pos)
    {
        instance.arrowPos = pos;
    }
}
