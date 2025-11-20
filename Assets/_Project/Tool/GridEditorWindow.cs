using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public enum EditorMode
{
    Grid,
    Pipe,
    Holder
}

public class GridEditorWindow : EditorWindow
{
    private MapData currentMap;

    private Vector2 scrollPos;
    private const float cellSize = 40f;

    // Mode đang chọn
    private EditorMode currentMode = EditorMode.Grid;

    // Pipe đang được chọn để edit
    private PipeData selectedPipeData = null;

    [MenuItem("Tools/Level Editor")]
    public static void Open()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        // Chọn MapData
        currentMap = (MapData)EditorGUILayout.ObjectField("Map Data", currentMap, typeof(MapData), false);

        if (currentMap == null)
        {
            DrawMapCreationUI();
            return;
        }

        EditorGUILayout.Space();

        // =========================
        // 3 MODE: GRID / PIPE / HOLDER
        // =========================
        DrawModeToolbar();

        EditorGUILayout.Space();

        // PANEL THEO MODE
        switch (currentMode)
        {
            case EditorMode.Grid:
                DrawMapSettingsRealtime();
                DrawResetMapButton();
                break;

            case EditorMode.Pipe:
                DrawPipeEditorPanel();
                DrawClearAllPipesButton();
                break;

            case EditorMode.Holder:
                DrawHolderEditorPanel();
                break;
        }

        EditorGUILayout.Space();

        // =========================
        // GRID VISUAL
        // =========================
        DrawGridUI();

        // =========================
        // SAVE + DELETE
        // =========================
        DrawButtons();
    }

    // ================================================================
    // MODE TOOLBAR
    // ================================================================
    private void DrawModeToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Toggle(currentMode == EditorMode.Grid, "Design Grid", "Button"))
            currentMode = EditorMode.Grid;

        if (GUILayout.Toggle(currentMode == EditorMode.Pipe, "Design Pipe", "Button"))
            currentMode = EditorMode.Pipe;

        if (GUILayout.Toggle(currentMode == EditorMode.Holder, "Design Holder", "Button"))
            currentMode = EditorMode.Holder;

        EditorGUILayout.EndHorizontal();
    }

    // ================================================================
    // CREATE NEW MAP
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
        currentMap.pipes = new List<PipeData>();

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
    // REAL-TIME RESIZE GRID (MODE: GRID)
    // ================================================================
    private void DrawMapSettingsRealtime()
    {
        EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);

        int width = EditorGUILayout.IntField("Width", currentMap.width);
        int height = EditorGUILayout.IntField("Height", currentMap.height);
        float time = EditorGUILayout.FloatField("Time", currentMap.time);

        if (!Mathf.Approximately(time, currentMap.time))
        {
            currentMap.time = time;
            EditorUtility.SetDirty(currentMap);
        }

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
                    newNodes.Add(oldNode);
                else
                    newNodes.Add(new NodeData(x, y, true));
            }
        }

        currentMap.nodes = newNodes;
        currentMap.width = newWidth;
        currentMap.height = newHeight;

        EditorUtility.SetDirty(currentMap);
        Repaint();
    }

    // ================================================================
    // RESET MAP BUTTON (Grid Mode)
    // ================================================================
    private void DrawResetMapButton()
    {
        GUI.backgroundColor = Color.yellow;

        if (GUILayout.Button("Reset Map (All isSpawn = TRUE)"))
        {
            foreach (var node in currentMap.nodes)
                node.isSpawn = true;

            EditorUtility.SetDirty(currentMap);
            Repaint();
        }

        GUI.backgroundColor = Color.white;
    }

    // ================================================================
    // GRID VISUAL & CLICK LOGIC
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
                PipeData pipe = currentMap.pipes.Find(p => p.x == x && p.y == y);

                if (pipe != null)
                    GUI.backgroundColor = Color.cyan;
                else
                    GUI.backgroundColor = node.isSpawn ? Color.gray : Color.black;

                if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                {
                    HandleCellClick(x, y, node);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndScrollView();
    }

    private void HandleCellClick(int x, int y, NodeData node)
    {
        switch (currentMode)
        {
            case EditorMode.Grid:
                node.isSpawn = !node.isSpawn;
                EditorUtility.SetDirty(currentMap);
                break;

            case EditorMode.Pipe:
                OnClickPipeCell(x, y);
                break;
        }
    }

    // ================================================================
    // PIPE PANEL
    // ================================================================
    private void OnClickPipeCell(int x, int y)
    {
        PipeData pipe = currentMap.pipes.Find(p => p.x == x && p.y == y);

        if (pipe == null)
        {
            pipe = new PipeData()
            {
                x = x,
                y = y,
                waterColors = new List<WaterColor>()
            };
            currentMap.pipes.Add(pipe);
        }

        selectedPipeData = pipe;
        EditorUtility.SetDirty(currentMap);
        Repaint();
    }

    private void DrawPipeEditorPanel()
    {
        EditorGUILayout.LabelField("Pipe Editor", EditorStyles.boldLabel);

        if (selectedPipeData == null)
        {
            EditorGUILayout.HelpBox(
                "Click một ô trong Grid (khi đang ở Design Pipe) để tạo/chọn Pipe.",
                MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Pipe at ({selectedPipeData.x},{selectedPipeData.y})", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Water List", EditorStyles.boldLabel);

        for (int i = 0; i < selectedPipeData.waterColors.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            selectedPipeData.waterColors[i].color =
                (EnumColor)EditorGUILayout.EnumPopup(selectedPipeData.waterColors[i].color, GUILayout.Width(100));

            selectedPipeData.waterColors[i].Value =
                EditorGUILayout.FloatField(selectedPipeData.waterColors[i].Value, GUILayout.Width(60));

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                selectedPipeData.waterColors.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Water"))
        {
            selectedPipeData.waterColors.Add(new WaterColor()
            {
                color = EnumColor.None,
                Value = 1
            });
        }

        EditorGUILayout.Space();

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete This Pipe"))
        {
            currentMap.pipes.Remove(selectedPipeData);
            selectedPipeData = null;

            GUI.backgroundColor = Color.white;
            EditorUtility.SetDirty(currentMap);
            Repaint();
            return;
        }
        GUI.backgroundColor = Color.white;

        EditorUtility.SetDirty(currentMap);
    }

    // ================================================================
    // CLEAR ALL PIPES BUTTON (Pipe Mode)
    // ================================================================
    private void DrawClearAllPipesButton()
    {
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);

        if (GUILayout.Button("Clear All Pipes"))
        {
            if (EditorUtility.DisplayDialog("Clear All Pipes", "Xóa TẤT CẢ Pipe trong map?", "Yes", "No"))
            {
                currentMap.pipes.Clear();
                selectedPipeData = null;

                EditorUtility.SetDirty(currentMap);
                Repaint();
            }
        }

        GUI.backgroundColor = Color.white;
    }

    // ================================================================
    // HOLDER PANEL
    // ================================================================
    private void DrawHolderEditorPanel()
    {
        EditorGUILayout.LabelField("Holder Editor", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Chưa implement. Bạn có thể dùng pattern giống Pipe Editor.", MessageType.Info);
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
                selectedPipeData = null;
            }
        }
        GUI.backgroundColor = Color.white;
    }
}
