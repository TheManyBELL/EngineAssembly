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

    public GameObject kinect_front;    // kinect��ǰ��
    public GameObject kinect_up;       // kinect�·�
    public GameObject kinect_right;    // kinect�ҷ�

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
            Debug.Log("��������������!");
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(IP), port));
            serverSocket.Listen(10); // �趨���10���Ŷ���������   
            Thread myThread = new Thread(ListenClientConnect); // ͨ�����̼߳����ͻ�������  
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

        partsDictionary.Add("Bearing", "���(��)");
        partsDictionary.Add("BearingSmall", "���(С)");
        partsDictionary.Add("Fan", "����");
        partsDictionary.Add("FanCasting", "������");
        partsDictionary.Add("FanSupport", "������֧��");
        partsDictionary.Add("HPC", "��ѹ��������");

        partsDictionary.Add("EnginePart", "������֧��");
        partsDictionary.Add("LPTCasing", "��ѹ�������");
        partsDictionary.Add("NozzleBody", "���춥��");
        partsDictionary.Add("NozzleVoids", "����ײ�");
        partsDictionary.Add("Spool", "��ѹ��������");
        partsDictionary.Add("Stand", "����������");


    }

    void ListenClientConnect()
    {
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clientSocket.Send(Encoding.UTF8.GetBytes("���������ӳɹ�"));
            clientList.Add(clientSocket);
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(clientSocket);
            Debug.Log("���յ��ͻ�������");
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
                // ͨ��clientSocket��������  
                int receiveBytes = myClientSocket.Receive(buffer);
                if (receiveBytes == 0)
                {
                    Debug.Log("No Message!");
                    return;
                }
                string recvStr = Encoding.UTF8.GetString(buffer, 0, receiveBytes);
                // Debug.LogFormat ("���տͻ��� {0} ����Ϣ��{1}", myClientSocket.RemoteEndPoint.ToString(), recvStr);
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
                myClientSocket.Shutdown(SocketShutdown.Both); // ��ֹ���ͺ��ϴ�
                myClientSocket.Close(); // �ر�Socket���ͷ���Դ
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
        //Debug.Log("�����:" + name_and_pos[0]);
        string partsNameInHierachy = partsDictionary[name_and_pos[0]] + "(Clone)";
        //Debug.Log("�����H:"+partsNameInHierachy);
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
