using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class MobileVibration
{
    public static void Vibrate(long ms = 100)
    {
        if(AudioManager.VibrateSetting == 1)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer =
               new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject activity =
                unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject vibrator =
                activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator != null)
                vibrator.Call("vibrate", ms);
        }
#endif
        }
    }
}