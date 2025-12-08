using UnityEditor;
using UnityEngine;
using System.IO;

public class DeletePersistentData : EditorWindow
{
    [MenuItem("Tools/Delete All Data")]
    public static void DeletePersistentFiles()
    {
        string path = Application.persistentDataPath;

        // Clear PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Delete persistent folder
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);

            Debug.Log("All Persistent Data deleted!");
        }
        else
        {
            Debug.LogWarning("Persistent Data Path not found!");
        }

        // 🔥 GỌI LẠI ReloadPref() nếu đang chạy PlayMode
        if (Application.isPlaying)
        {
            if (DataManager.Instance != null)
            {
                Debug.Log("Calling ReloadPref()");
                DataManager.Instance.SendMessage("ReloadPref");
            }
            else
            {
                Debug.LogWarning("DataManager.Instance is null — chưa được tạo trong Scene!");
            }
        }

        EditorUtility.DisplayDialog("Success", "All data has been cleared!", "OK");
    }

    [MenuItem("Tools/Open Persistent Data Folder")]
    public static void OpenPersistentDataFolder()
    {
        string path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
        Debug.Log("Opened: " + path);
    }
}
