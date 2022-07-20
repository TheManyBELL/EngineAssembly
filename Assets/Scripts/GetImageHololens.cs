using UnityEngine;
using UnityEngine.Windows.WebCam;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class GetImageHololens : MonoBehaviour
{
    PhotoCapture photoCaptureObject = null;
    CameraParameters c;
    // List<byte> imageBufferList = new List<byte>();
    public Texture2D texture;
   
    void Start()
    {
        Debug.Log("Get Hololens Camera Image Begin!");
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void Update()
    {
    }

    
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        Debug.Log("resolution width: " + cameraResolution.width + ", resolution height: " + cameraResolution.height);
        texture = new Texture2D(cameraResolution.width, cameraResolution.height);
        photoCaptureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("photo end");
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }


    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCaptturePhotoToMemory);
            Debug.Log("photo start success");
        }
        else
        {
            Debug.Log("photo start fail");
        } 
    }

    private Texture2D CreateTexture(List<byte> rawData, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.BGRA32, false);
        tex.LoadRawTextureData(rawData.ToArray());
        tex.Apply();
        return tex;
    }

    void OnCaptturePhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            photoCaptureFrame.UploadImageDataToTexture(texture);
            // photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);
            // imageBufferList = FlipVertical(imageBufferList, cameraParameters.cameraResolutionWidth, cameraParameters.cameraResolutionHeight, 4);
            // texture = CreateTexture(imageBufferList, c.cameraResolutionWidth, c.cameraResolutionHeight);
        }
        // photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Saved Photo to disk!");
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk");
        }
    }

}