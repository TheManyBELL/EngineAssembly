using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Left : MonoBehaviour
{
    public Button left, right, back, front, up, down;
    public Button clockwise, notclockwise;
    public Button larger, smaller;
    public UnityEvent left_event, right_event, back_event, front_event, up_event, down_event;
    public UnityEvent clockwise_event, notclockwise_event;
    public UnityEvent larger_event, smaller_event;

    public GameObject Scene;

    void Awake()
    {
        left_event.AddListener(MoveLeft);
        right_event.AddListener(MoveRight);
        back_event.AddListener(MoveBack);
        front_event.AddListener(MoveFront);
        up_event.AddListener(MoveUp);
        down_event.AddListener(MoveDown);

        clockwise_event.AddListener(RotateClockwise);
        notclockwise_event.AddListener(RotateNotclockwise);

        larger_event.AddListener(Larger);
        smaller_event.AddListener(Smaller);
    }

    // Start is called before the first frame update
    void Start()
    {
        left.onClick.AddListener(() => {
            left_event.Invoke();
        });
        right.onClick.AddListener(() => {
            right_event.Invoke();
        });
        back.onClick.AddListener(() => {
            back_event.Invoke();
        });
        front.onClick.AddListener(() => {
            front_event.Invoke();
        });
        up.onClick.AddListener(() => {
            up_event.Invoke();
        });
        down.onClick.AddListener(() => {
            down_event.Invoke();
        });


        clockwise.onClick.AddListener(() => {
            clockwise_event.Invoke();
        });
        notclockwise.onClick.AddListener(() => {
            notclockwise_event.Invoke();
        });


        larger.onClick.AddListener(() => {
            larger_event.Invoke();
        });
        smaller.onClick.AddListener(() => {
            smaller_event.Invoke();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveLeft()
    {
        Scene.transform.position += new Vector3(-0.01f, 0.0f, 0.0f);
    }

    void MoveRight() 
    {
        Scene.transform.position += new Vector3(0.01f, 0.0f, 0.0f);
    }

    void MoveBack()
    {
        Scene.transform.position += new Vector3(0.0f, 0.0f, 0.01f);
    }

    void MoveFront() {
        Scene.transform.position += new Vector3(0.0f, 0.0f, -0.01f);
    }

    void MoveUp() {
        Scene.transform.position += new Vector3(0.0f, 0.01f, 0.0f);
    }

    void MoveDown() {
        Scene.transform.position += new Vector3(0.0f, -0.01f, 0.0f);
    }

    void RotateClockwise() {
        Scene.transform.eulerAngles += new Vector3(0.0f, 0.5f, 0.0f);
    }

    void RotateNotclockwise() {
        Scene.transform.eulerAngles += new Vector3(0.0f, -0.5f, 0.0f);
    }

    void Larger() {
        Scene.transform.localScale += new Vector3(0.01f, 0.0f, 0.01f);
    }

    void Smaller() {
        Scene.transform.localScale += new Vector3(-0.01f, 0.0f, -0.01f);
    }
}
