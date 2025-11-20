using UnityEngine;

public class SOSingleton<T> : ScriptableObject where T : Object
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<T>("SO Singleton/" + typeof(T).Name);
            }
            return _instance;
        }
    }
}