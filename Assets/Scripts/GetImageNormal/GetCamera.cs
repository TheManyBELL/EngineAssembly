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
        /// 摄像头图像类,继承自texture
        /// </summary>
        static WebCamTexture camTexture;
        /// <summary>
        /// 图像保存地址
        /// </summary>
        public static string Save_Path = Application.streamingAssetsPath + "/FaceDetect/FaceDetect.jpg";
        /// <summary>
        /// 获取图像
        /// </summary>
        /// <param name="image"></param>
        public static void GetWebCam(Image image)
        {
            //如果用户允许访问，开始获取图像
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                //先获取设备
                WebCamDevice[] device = WebCamTexture.devices;

                string deviceName = device[0].name;
                Debug.Log(deviceName);
                //然后获取图像
                camTexture = new WebCamTexture(deviceName);
                //将获取的图像赋值
                image.material = new Material(Shader.Find("Unlit/Texture"));
                image.material.mainTexture = camTexture;
                //开始实时获取
                camTexture.Play();

            } 
            else
            {
                Debug.Log("aaa");
            }
        }
        /// 图片保存有两种方法：
        ///（1）截屏
        ///（2）直接保存摄像头图像数据
        ///  本文采用第二种方法，使用webcamTexture这个类保存摄像机的图像。
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <returns></returns>
        public static Texture2D Save()
        {
            Texture2D t2d = new Texture2D(camTexture.width, camTexture.height, TextureFormat.ARGB32, true);
            //将WebCamTexture 的像素保存到texture2D中
            t2d.SetPixels(camTexture.GetPixels());
            t2d.Apply();

            return t2d;
        }

    }

}
