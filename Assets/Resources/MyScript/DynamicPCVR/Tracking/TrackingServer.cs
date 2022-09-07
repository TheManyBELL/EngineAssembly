using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System;

public class TrackingServer : MonoBehaviour
{

    public GameObject kinect_front;    // kinect正前方
    public GameObject kinect_up;       // kinect下方
    public GameObject kinect_right;    // kinect右方

    public string IP;
    public int port;
    private Socket serverSocket;
    private List<Socket> clientList;

    private Queue<string> msg_queue;

    Dictionary<string, string> partsDictionary = new Dictionary<string, string>();

    void Awake()
    {
        if (GlobleInfo.ClientMode.Equals(CameraMode.VR))
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new List<Socket>();

            msg_queue = new Queue<string>();
            initialPartsDictionary();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GlobleInfo.ClientMode.Equals(CameraMode.VR))
        {
            Debug.Log("服务器端已启动!");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), port));
            serverSocket.Listen(10); // 设定最多10个排队连接请求   
            Thread myThread = new Thread(ListenClientConnect); // 通过多线程监听客户端连接  
            myThread.Start();
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobleInfo.ClientMode.Equals(CameraMode.VR))
        {
            while (msg_queue.Count > 0)
            {
                string msg = msg_queue.Dequeue();
                ParseMsg(msg);
            }
        }
            
    }

    void initialPartsDictionary()
    {
        partsDictionary = new Dictionary<string, string>();

        partsDictionary.Add("Bearing", "轴承(大)");
        partsDictionary.Add("BearingSmall", "轴承(小)");
        partsDictionary.Add("Fan", "风扇");
        partsDictionary.Add("FanCasting", "风机外壳");
        partsDictionary.Add("FanSupport", "风机外壳支架");
        partsDictionary.Add("HPC", "高压涡轮主体");

        partsDictionary.Add("EnginePart", "风机外壳支架");
        partsDictionary.Add("LPTCasing", "低压涡轮外壳");
        partsDictionary.Add("NozzleBody", "管嘴顶部");
        partsDictionary.Add("NozzleVoids", "管嘴底部");
        partsDictionary.Add("Spool", "高压涡轮主体");
        partsDictionary.Add("Stand", "发动机底座");


    }

    void ListenClientConnect()
    {
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clientSocket.Send(Encoding.UTF8.GetBytes("服务器连接成功"));
            clientList.Add(clientSocket);
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(clientSocket);
            Debug.Log("接收到客户端连接");
        }
    }

    void ReceiveMessage(object clientSocket)
    {
        byte[] buffer = new byte[1024];
        Socket myClientSocket = (Socket)clientSocket;
        while (true)
        {
            try
            {
                // 通过clientSocket接收数据  
                int receiveBytes = myClientSocket.Receive(buffer);
                if (receiveBytes == 0)
                {
                    Debug.Log("No Message!");
                    return;
                }
                string recvStr = Encoding.UTF8.GetString(buffer, 0, receiveBytes);
                // Debug.LogFormat ("接收客户端 {0} 的消息：{1}", myClientSocket.RemoteEndPoint.ToString(), recvStr);
                string[] singleStr = recvStr.Split(';');
                foreach (string s in singleStr)
                {
                    if (s.Length == 0) continue;
                    msg_queue.Enqueue(s);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                myClientSocket.Shutdown(SocketShutdown.Both); // 禁止发送和上传
                myClientSocket.Close(); // 关闭Socket并释放资源
                break;
            }
        }
    }

    void ParseMsg(string msg)
    {
        string[] name_and_pos = msg.Split(':');
        if (name_and_pos.Length < 2) return;
        Debug.Assert(name_and_pos.Length == 2);

        // string[] m_val = name_and_pos[1].Split(" ");
        // Debug.Assert(m_val.Length == 16);

        List<float> m_val = stringToFloatList(name_and_pos[1]);
        if (m_val.Count < 12) return;
        Debug.Assert(m_val.Count == 12);
        //Debug.Log("零件名:" + name_and_pos[0]);
        string partsNameInHierachy = partsDictionary[name_and_pos[0]] + "(Clone)";
        //Debug.Log("零件名H:"+partsNameInHierachy);
        GameObject t = GameObject.Find(partsNameInHierachy);
        if (!t) return;

        // mat
        Vector4 homogeneous = new Vector4(0, 0, 0, 1);
        Vector3 right = (kinect_right.transform.position - transform.position).normalized,
            up = (kinect_up.transform.position - transform.position).normalized,
            front = (kinect_front.transform.position - transform.position).normalized;

        Matrix4x4 kinect_coordinate = new Matrix4x4(right, up, front, homogeneous);
        Matrix4x4 transform_mat = new Matrix4x4(
            new Vector4(m_val[0 * 4 + 0], m_val[1 * 4 + 0], m_val[2 * 4 + 0], 0),
            new Vector4(m_val[0 * 4 + 1], m_val[1 * 4 + 1], m_val[2 * 4 + 1], 0),
            new Vector4(m_val[0 * 4 + 2], m_val[1 * 4 + 2], m_val[2 * 4 + 2], 0),
            new Vector4(m_val[0 * 4 + 3], m_val[1 * 4 + 3], m_val[2 * 4 + 3], 1));

        // translate
        t.transform.position = new Vector3(0.0f, 0.0f, 0.0f);   // back
        t.transform.position = this.transform.position +
            vec4to3(kinect_coordinate * (transform_mat * vec3to4(t.transform.position, 1.0f)));

        // rotate 
        t.transform.eulerAngles = new Vector3(0, 0, 0);
        Vector4 n_forward = kinect_coordinate * (transform_mat * vec3to4(t.transform.forward, 0.0f));
        Vector4 n_up = kinect_coordinate * (transform_mat * vec3to4(t.transform.up, 0.0f));
        t.transform.rotation = Quaternion.LookRotation(n_forward, n_up);
    }

    List<float> stringToFloatList(string str)
    {
        string oneDimensional = @"-?\d+(\.\d+)?";
        Regex posPattern = new Regex(oneDimensional);

        List<float> info = new List<float>();
        foreach (Match match in posPattern.Matches(str))
        {
            info.Add(Convert.ToSingle(match.Value));
        }
        return info;
    }

    public bool connected()
    {
        return clientList.Count > 0;
    }

    Vector4 vec3to4(Vector3 t, float w)
    {
        return new Vector4(t.x, t.y, t.z, w);
    }

    Vector3 vec4to3(Vector4 t)
    {
        return new Vector3(t.x, t.y, t.z);
    }
}
