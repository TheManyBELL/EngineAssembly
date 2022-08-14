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
    // ����ʵ��ƽ���ƶ�
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
            // �ƶ��Ŀ�ʼʱ��
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
    /// ����ƽ���ƶ�
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

            // �����ƶ�
            cameraObject.transform.position = Vector3.Lerp(start_position, target_position, percentageComplete);
            // �Ƕȱ仯
            // cameraObject.transform.eulerAngles = Vector3.Lerp(cameraObject.transform.eulerAngles, target_eulerAngle, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                break;
            }
        }
        Debug.Log("��ǩ�˴��ƶ�����");
    }
}
