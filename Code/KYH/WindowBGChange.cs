using System;
using UnityEngine;
using UnityEngine.UI;

public class WindowBGChange : MonoBehaviour
{
    [SerializeField]
    private RawImage rawImage;

    private void Start()
    {
        try
        {
            rawImage.texture = BGManager.Instance.GetPCWallpaper();
        }
        catch(Exception e)
        {
            print("못 불러오는 배경화면임;;");
            print($"Error: {e.Message}");
        }
    }
}
