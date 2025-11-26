//using UnityEngine;

//public static class FPSLimiter
//{
//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
//    public static void InitGameSetup()
//    {
//        QualitySettings.vSyncCount = 0;

//        var maxFrameRate = Mathf.Max(60, (int)Screen.currentResolution.refreshRateRatio.value);
//        Debug.Log((int)Screen.currentResolution.refreshRateRatio.value);

//        Application.targetFrameRate = maxFrameRate;

//        Screen.sleepTimeout = SleepTimeout.NeverSleep;
//    }
//}