using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GridEditorWindow : EditorWindow
{
    private MapData currentMap;

    private Vector2 scrollPos;

    private const float cellSize = 40f;

    [MenuItem("Tools/Grid Map Editor")]
    public static void Open()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        currentMap = (MapData)EditorGUILayout.ObjectField("Map Data", currentMap, typeof(MapData), false);

        if (currentMap == null)
        {
            DrawMapCreationUI();
            return;
        }

        EditorGUILayout.Space();

        DrawMapSettingsRealtime(); // 👈 Auto resize real-time

        EditorGUILayout.Space();
        DrawGridUI();

        DrawButtons();
    }


    // ================================================================
    // Create New Map
    // ================================================================
    private int newWidth = 5;
    private int newHeight = 5;

    private void DrawMapCreationUI()
    {
        EditorGUILayout.LabelField("Create New Map", EditorStyles.boldLabel);

        newWidth = EditorGUILayout.IntField("Width", newWidth);
        newHeight = EditorGUILayout.IntField("Height", newHeight);

        if (GUILayout.Button("Create New MapData"))
        {
            CreateMapData(newWidth, newHeight);
        }
    }

    private void CreateMapData(int width, int height)
    {
        currentMap = ScriptableObject.CreateInstance<MapData>();
        currentMap.width = width;
        currentMap.height = height;

        currentMap.nodes = new List<NodeData>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                currentMap.nodes.Add(new NodeData(x, y, true));

        string path = EditorUtility.SaveFilePanelInProject(
            "Save MapData",
            "NewMapData",
            "asset",
            "Choose where to save");

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(currentMap, path);
            AssetDatabase.SaveAssets();
        }
    }


    // ================================================================
    // REAL-TIME RESIZE GRID
    // ================================================================
    private void DrawMapSettingsRealtime()
    {
        EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);

        int width = EditorGUILayout.IntField("Width", currentMap.width);
        int height = EditorGUILayout.IntField("Height", currentMap.height);

        // ONLY resize when value changed
        if (width != currentMap.width || height != currentMap.height)
        {
            ResizeGrid(width, height);
        }
    }

    private void ResizeGrid(int newWidth, int newHeight)
    {
        List<NodeData> newNodes = new List<NodeData>();

        for (int x = 0; x < newWidth; x++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                NodeData oldNode = currentMap.nodes.Find(n => n.x == x && n.y == y);

                if (oldNode != null)
                {
                    newNodes.Add(oldNode);
                }
                else
                {
                    // New nodes default isSpawn = true
                    newNodes.Add(new NodeData(x, y, true));
                }
            }
        }

        currentMap.nodes = newNodes;
        currentMap.width = newWidth;
        currentMap.height = newHeight;

        EditorUtility.SetDirty(currentMap);
        Repaint();
    }


    // ================================================================
    // PAINT GRID GUI
    // ================================================================
    private void DrawGridUI()
    {
        EditorGUILayout.LabelField("Grid Visual", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        int w = currentMap.width;
        int h = currentMap.height;

        for (int y = h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < w; x++)
            {
                NodeData node = currentMap.nodes.Find(n => n.x == x && n.y == y);

                GUI.backgroundColor = node.isSpawn ? Color.gray : Color.black;

                if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    node.isSpawn = !node.isSpawn;
                    EditorUtility.SetDirty(currentMap);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndScrollView();
    }


    // ================================================================
    // SAVE + DELETE
    // ================================================================
    private void DrawButtons()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Save Changes"))
        {
            EditorUtility.SetDirty(currentMap);
            AssetDatabase.SaveAssets();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete Map File"))
        {
            string path = AssetDatabase.GetAssetPath(currentMap);

            if (EditorUtility.DisplayDialog("Delete Map?", "Xóa file này?", "Yes", "No"))
            {
                AssetDatabase.DeleteAsset(path);
                currentMap = null;
            }
        }
        GUI.backgroundColor = Color.white;
    }
}
