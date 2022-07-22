using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

public class socketClientVRHMD : MonoBehaviour
{
    public string serverIP;
    public int serverPort;
    Socket clientSocket;
    Thread receiveThread;
    private ClickEvent next_control_script;
    private static socketClientVRHMD instance;


    void Init()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("SUCCESS 客户端已启动!");
        try
        {
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(serverIP), serverPort)); //配置服务器IP与端口  
            Debug.Log("连接服务器成功");

            receiveThread = new Thread(ReceivePosAndEuler);
            receiveThread.Start();
        } 
        catch
        {
            Debug.Log("连接服务器失败");
            return;
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        next_control_script = GameObject.Find("ClickEventObject").GetComponent<ClickEvent>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 接收从服务器来的消息
    void ReceivePosAndEuler()
    {
        while (true)
        {
            try
            {             
                byte[] buffer = new byte[1024];
                int byteNum = clientSocket.Receive(buffer);//将接受到的数据，存入buffer。返回值为接受到的字节数
                if (byteNum == 0)
                {
                    Debug.Log("ERROR 没有收到服务端信息");
                    return;
                }
                string message = Encoding.UTF8.GetString(buffer, 0, byteNum);//将接受到的数据，转化为字符串
                // next_control_script.EnableNext();
                next_control_script.receive = true;
                Debug.Log(message);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                if (clientSocket != null)
                {
                    // 关闭Socket并释放资源
                    clientSocket.Close();
                }
                break;
            }
        }
    }

    //这个是干啥的
    public static socketClientVRHMD getInstance()
    {
        if (instance == null)
        {
            instance = new socketClientVRHMD();
            instance.Init();
        }
        return instance;
    }

    //退出整个socket
    public static void SocketQuit()
    {
        Debug.Log("SocketQuit");
        //先关闭客户端
        if (instance != null)
        {
            if (instance.clientSocket != null)
            {
                Debug.Log("clientSocketQuit");
                instance.clientSocket.Close();
            }
            //再关闭线程
            if (instance.receiveThread != null)
            {
                Debug.Log("receiveThread");
                instance.receiveThread.Interrupt();
                instance.receiveThread.Abort();
            }
        }
    }
}
