using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWalking : MonoBehaviour
{
    GameObject cameraObject;

    public Vector3 startPosition;
    public Vector3 startEulerAngle;
    public Vector3 endPosition;
    public Vector3 endEulerAngle;

    public float timeTakenDuringLerp = 10.0f;
    // 用于实现平滑移动
    private float _timeStartedLerping;


    // Start is called before the first frame update
    void Start()
    {
        cameraObject = this.transform.gameObject;
        SetCameraPosition(cameraObject, startPosition, startEulerAngle);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetCameraPosition(cameraObject, startPosition, startEulerAngle);
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            // 移动的开始时间
            _timeStartedLerping = Time.time;
            StartCoroutine(LerpMoveCamera(cameraObject,startPosition, endPosition, endEulerAngle));
        }

    }


    private void SetCameraPosition(GameObject camera,Vector3 pos,Vector3 ang)
    {
        camera.transform.position = pos;
        camera.transform.eulerAngles = ang;
    }


    /// <summary>
    /// 物体平滑移动
    /// </summary>
    /// <param name="cameraObject"></param>
    /// <param name="target_position"></param>
    /// <returns></returns>
    IEnumerator LerpMoveCamera(GameObject cameraObject, Vector3 start_position, Vector3 target_position, Vector3 target_eulerAngle)
    {
        while (cameraObject.transform.position != target_position)
        {
            yield return new WaitForFixedUpdate();

            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            Debug.Log(timeSinceStarted.ToString());

            // 坐标移动
            cameraObject.transform.position = Vector3.Lerp(start_position, target_position, percentageComplete);
            // 角度变化
            // cameraObject.transform.eulerAngles = Vector3.Lerp(cameraObject.transform.eulerAngles, target_eulerAngle, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                break;
            }
        }
        Debug.Log("标签此次移动结束");
    }
}
