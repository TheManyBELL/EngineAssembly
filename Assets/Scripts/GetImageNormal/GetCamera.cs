using UnityEngine;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Tool.GetCamera
{
    public class GetCamera
    {
        /// <summary>
        /// ����ͷͼ����,�̳���texture
        /// </summary>
        static WebCamTexture camTexture;
        /// <summary>
        /// ͼ�񱣴��ַ
        /// </summary>
        public static string Save_Path = Application.streamingAssetsPath + "/FaceDetect/FaceDetect.jpg";
        /// <summary>
        /// ��ȡͼ��
        /// </summary>
        /// <param name="image"></param>
        public static void GetWebCam(Image image)
        {
            //����û�������ʣ���ʼ��ȡͼ��
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                //�Ȼ�ȡ�豸
                WebCamDevice[] device = WebCamTexture.devices;

                string deviceName = device[0].name;
                Debug.Log(deviceName);
                //Ȼ���ȡͼ��
                camTexture = new WebCamTexture(deviceName);
                //����ȡ��ͼ��ֵ
                image.material = new Material(Shader.Find("Unlit/Texture"));
                image.material.mainTexture = camTexture;
                //��ʼʵʱ��ȡ
                camTexture.Play();

            } 
            else
            {
                Debug.Log("aaa");
            }
        }
        /// ͼƬ���������ַ�����
        ///��1������
        ///��2��ֱ�ӱ�������ͷͼ������
        ///  ���Ĳ��õڶ��ַ�����ʹ��webcamTexture����ౣ���������ͼ��
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        /// <returns></returns>
        public static Texture2D Save()
        {
            Texture2D t2d = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, true);
            //��WebCamTexture �����ر��浽texture2D��
            t2d.SetPixels(camTexture.GetPixels());
            t2d.Apply();

            return t2d;
        }

    }

}
