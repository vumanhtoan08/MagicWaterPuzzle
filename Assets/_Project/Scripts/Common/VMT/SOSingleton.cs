using UnityEngine;

public class SOSingleton<T> : ScriptableObject where T : SOSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {

                _instance = Resources.Load<T>("SO Singleton/" + typeof(T).Name);
                _instance.Init();
            }
            return _instance;
        }
    }

    public virtual void Init() { }
}

// tạo SO đúng với tên Script