using UnityEngine;
using UnityEditor;

public class LevelLoaderWindow : EditorWindow
{
    private int levelToLoad = 1;

    [MenuItem("Tools/Level Loader")]
    public static void Open()
    {
        GetWindow<LevelLoaderWindow>("Level Loader");
    }

    private void OnGUI()
    {
        GUILayout.Label("Load Level Runtime", EditorStyles.boldLabel);

        levelToLoad = EditorGUILayout.IntField("Level:", levelToLoad);

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Load Level"))
        {
            LoadLevel();
        }

        GUI.enabled = true;

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("➡️ Nhấn Play để Load Level", MessageType.Info);
    }

    private void LoadLevel()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("❗ Nhấn Play trước khi Load Level");
            return;
        }

        var manager = LevelManager.Instance;

        if (manager == null)
        {
            Debug.LogError("❌ LevelManager Singleton chưa tồn tại trong scene!");
            return;
        }

        manager.LoadLevel(levelToLoad);

        Debug.Log($"✅ Loaded Level {levelToLoad}");
    }

}
