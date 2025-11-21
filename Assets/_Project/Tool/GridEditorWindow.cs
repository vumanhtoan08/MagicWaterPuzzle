using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum EditorMode
{
    Grid,
    Pipe,
    Holder
}

// 10 loại shape của Holder
public enum HolderShape
{
    One,
    L,
    ReverseL,
    ShortL,
    ShortT,
    Three,
    ThreeSquare,
    Plus,
    Two,
    TwoSquare
}

// Góc quay chỉ 4 trạng thái
public enum HolderRotation
{
    Deg0 = 0,
    Deg90 = 1,
    Deg180 = 2,
    Deg270 = 3
}

public class GridEditorWindow : EditorWindow
{
    // =====================================================================
    // FIELDS
    // =====================================================================

    private MapData currentMap;

    private const float cellSize = 40f;

    private EditorMode currentMode = EditorMode.Grid;

    // Pipe
    private PipeData selectedPipeData = null;

    // Holder
    private HolderShape? selectedShape = null;
    private HolderRotation selectedRotation = HolderRotation.Deg0;
    private Vector2Int? holderOrigin = null;
    private EnumColor selectedEnumColor = EnumColor.red;

    // Scroll tổng cho 3 grid (dọc)
    private Vector2 mainScroll;

    // Database shape offsets (tọa độ relative từ ô gốc)
    private static readonly Dictionary<HolderShape, Vector2Int[]> shapeOffsets =
        new Dictionary<HolderShape, Vector2Int[]>
        {
            // 1 ô
            { HolderShape.One, new[]
                {
                    new Vector2Int(0, 0)
                }
            },

            // 2 ô: mặc định dọc (gốc + trên)
            { HolderShape.Two, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1)
                }
            },

            // 3 ô: mặc định dọc (gốc, trên, dưới)
            { HolderShape.Three, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1)
                }
            },

            // L ngắn: 2 ô hình góc
            { HolderShape.ShortL, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, -1),
                }
            },

            // T ngắn: gốc ở giữa
            { HolderShape.ShortT, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, -1),
                }
            },

            // L 3 ô
            { HolderShape.L, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1,0),
                    new Vector2Int(0, -1),
                    new Vector2Int(0, -2),
                }
            },

            // Reverse L 3 ô
            { HolderShape.ReverseL, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1,0),
                    new Vector2Int(0, -1),
                    new Vector2Int(0, -2),
                }
            },

            // 3x3
            { HolderShape.ThreeSquare, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(-1, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, -1),
                    new Vector2Int(0, -1),
                    new Vector2Int(1, -1),
                }
            },

            // 1x2
            { HolderShape.TwoSquare, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                }
            },

            // Hình dấu +
            { HolderShape.Plus, new[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, -1),
                    new Vector2Int(1, 0),
                    new Vector2Int(-1, 0)
                }
            },
        };

    // =====================================================================
    // MENU
    // =====================================================================

    [MenuItem("Tools/Level Editor")]
    public static void Open()
    {
        GetWindow<GridEditorWindow>("Grid Editor");
    }

    // =====================================================================
    // ONGUI
    // =====================================================================

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
                DrawClearAllHoldersButton();
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawThreeGrids();      // 3 Grid Map/Pipe/Holder (dọc + scroll)

        DrawSaveDeleteButtons();
    }

    // =====================================================================
    // MODE TOOLBAR
    // =====================================================================

    private void DrawModeToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Toggle(currentMode == EditorMode.Grid, "Grid", "Button"))
            currentMode = EditorMode.Grid;

        if (GUILayout.Toggle(currentMode == EditorMode.Pipe, "Pipe", "Button"))
            currentMode = EditorMode.Pipe;

        if (GUILayout.Toggle(currentMode == EditorMode.Holder, "Holder", "Button"))
            currentMode = EditorMode.Holder;

        EditorGUILayout.EndHorizontal();
    }

    // =====================================================================
    // CREATE NEW MAP
    // =====================================================================

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
        currentMap.holders = new List<HolderData>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                currentMap.nodes.Add(new NodeData(x, y, true));
            }
        }

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

    // =====================================================================
    // MAP SETTINGS + RESIZE
    // =====================================================================

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

    // =====================================================================
    // 3 GRID VIEW (DỌC + SCROLL DỌC)
    // =====================================================================

    private void DrawThreeGrids()
    {
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll, GUILayout.ExpandHeight(true));

        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);

        DrawCenteredGridPanel("Grid Map", DrawCellMap);
        GUILayout.Space(30);

        DrawCenteredGridPanel("Grid Pipe", DrawCellPipe);
        GUILayout.Space(30);

        DrawCenteredGridPanel("Grid Holder", DrawCellHolder);

        GUILayout.Space(20);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }

    private void DrawCenteredGridPanel(string title, Action<int, int> drawCellFunc)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical(GUILayout.Width(320));

        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

        int w = currentMap.width;
        int h = currentMap.height;

        for (int y = h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            for (int x = 0; x < w; x++)
            {
                drawCellFunc(x, y);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    // =====================================================================
    // CELL RENDERERS
    // =====================================================================

    private void DrawCellMap(int x, int y)
    {
        NodeData node = currentMap.nodes.Find(n => n.x == x && n.y == y);

        GUI.backgroundColor = node.isSpawn ? Color.gray : Color.black;

        if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
        {
            if (currentMode == EditorMode.Grid)
            {
                node.isSpawn = !node.isSpawn;
                EditorUtility.SetDirty(currentMap);
            }
        }

        GUI.backgroundColor = Color.white;
    }

    private void DrawCellPipe(int x, int y)
    {
        PipeData pipe = currentMap.pipes.Find(p => p.x == x && p.y == y);

        GUI.backgroundColor = pipe != null ? Color.cyan : Color.gray;

        if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
        {
            if (currentMode == EditorMode.Pipe)
            {
                OnClickPipeCell(x, y);
            }
        }

        GUI.backgroundColor = Color.white;
    }

    private void DrawCellHolder(int x, int y)
    {
        HolderData holder = currentMap.holders.Find(h => h.x == x && h.y == y);

        if (holder != null)
        {
            GUI.backgroundColor = ConvertEnumColor(holder.color); // màu theo data
        }
        else
        {
            GUI.backgroundColor = Color.gray;
        }

        if (GUILayout.Button($"{x},{y}", GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
        {
            if (currentMode == EditorMode.Holder)
            {
                // chọn ô gốc
                holderOrigin = new Vector2Int(x, y);
            }
        }

        GUI.backgroundColor = Color.white;
    }

    // =====================================================================
    // PIPE EDITOR
    // =====================================================================

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
                "Click một ô trong Grid Pipe để tạo/chọn Pipe.",
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

    // =====================================================================
    // HOLDER EDITOR (SHAPE + ROTATION + COLOR + PREVIEW)
    // =====================================================================

    private void DrawHolderEditorPanel()
    {
        EditorGUILayout.LabelField("Holder Editor", EditorStyles.boldLabel);

        // ----------------------------
        // 10 SHAPE BUTTONS
        // ----------------------------
        EditorGUILayout.LabelField("Holder Shapes:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        foreach (HolderShape shape in Enum.GetValues(typeof(HolderShape)))
        {
            GUI.backgroundColor = (selectedShape == shape) ? new Color(0.5f, 0.7f, 1f) : Color.white;

            if (GUILayout.Button(shape.ToString(), GUILayout.Width(80), GUILayout.Height(30)))
            {
                selectedShape = shape;
            }
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (selectedShape == null)
        {
            EditorGUILayout.HelpBox("Chọn 1 Holder Shape.", MessageType.Info);
            return;
        }

        if (holderOrigin == null)
        {
            EditorGUILayout.HelpBox("Click một ô trên Grid Holder để đặt Origin.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Origin: ({holderOrigin.Value.x}, {holderOrigin.Value.y})");

        // Rotation dropdown
        selectedRotation = (HolderRotation)EditorGUILayout.EnumPopup("Rotation", selectedRotation);

        // Color picker
        selectedEnumColor = (EnumColor)EditorGUILayout.EnumPopup("Holder Color", selectedEnumColor);

        // Preview
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);
        DrawShapePreview((HolderShape)selectedShape, selectedRotation, ConvertEnumColor(selectedEnumColor));

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply Shape", GUILayout.Height(30)))
        {
            ApplyHolderShape(
                holderOrigin.Value.x,
                holderOrigin.Value.y,
                (HolderShape)selectedShape,
                (int)selectedRotation
            );
        }
    }

    private void DrawClearAllHoldersButton()
    {
        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);

        if (GUILayout.Button("Clear All Holders"))
        {
            if (EditorUtility.DisplayDialog("Clear All Holders", "Xóa TẤT CẢ Holder trong map?", "Yes", "No"))
            {
                currentMap.holders.Clear();
                holderOrigin = null;
                selectedShape = null;

                EditorUtility.SetDirty(currentMap);
                Repaint();
            }
        }

        GUI.backgroundColor = Color.white;
    }

    private void ApplyHolderShape(int originX, int originY, HolderShape shape, int rotateStep)
    {
        List<Vector2Int> cells = GetShapeCells(shape, rotateStep);

        // Xóa holder cũ ở các cell sẽ chiếm rồi add mới
        foreach (var c in cells)
        {
            int tx = originX + c.x;
            int ty = originY + c.y;

            if (tx < 0 || ty < 0 || tx >= currentMap.width || ty >= currentMap.height)
                continue;

            currentMap.holders.RemoveAll(h => h.x == tx && h.y == ty);

            currentMap.holders.Add(new HolderData()
            {
                x = tx,
                y = ty,
                shapeType = shape,
                rotation = rotateStep * 90,
                color = selectedEnumColor,
                type = HolderType.Basic
            });
        }

        EditorUtility.SetDirty(currentMap);
        Repaint();
    }

    private List<Vector2Int> GetShapeCells(HolderShape shape, int rotateStep)
    {
        var baseOffsets = shapeOffsets[shape];
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (var o in baseOffsets)
        {
            Vector2 v = new Vector2(o.x, o.y);

            for (int i = 0; i < rotateStep; i++)
            {
                // xoay 90 độ (x,y) -> (-y, x)
                v = new Vector2(-v.y, v.x);
            }

            result.Add(new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)));
        }

        return result;
    }

    private void DrawShapePreview(HolderShape shape, HolderRotation rotation, Color color)
    {
        // ----------------------------
        // Căn giữa Preview
        // ----------------------------
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        const int previewSize = 150;
        Rect rect = GUILayoutUtility.GetRect(previewSize, previewSize, GUILayout.Width(previewSize), GUILayout.Height(previewSize));

        // ----------------------------
        // Nền preview
        // ----------------------------
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        // 5x5 grid
        float cell = rect.width / 5f;

        // Grid lines
        Handles.color = new Color(1, 1, 1, 0.08f);
        for (int i = 0; i <= 5; i++)
        {
            // vertical lines
            Handles.DrawLine(
                new Vector2(rect.x + i * cell, rect.y),
                new Vector2(rect.x + i * cell, rect.y + rect.height)
            );

            // horizontal lines
            Handles.DrawLine(
                new Vector2(rect.x, rect.y + i * cell),
                new Vector2(rect.x + rect.width, rect.y + i * cell)
            );
        }

        // ----------------------------
        // Tính cell bị fill
        // ----------------------------
        List<Vector2Int> cells = GetShapeCells(shape, (int)rotation);

        // center offset (grid 5x5)
        Vector2 center = new Vector2(2, 2);

        foreach (var c in cells)
        {
            Vector2Int pos = c + Vector2Int.FloorToInt(center);

            if (pos.x < 0 || pos.x > 4 || pos.y < 0 || pos.y > 4)
                continue;

            Rect r = new Rect(
                rect.x + pos.x * cell,
                rect.y + (4 - pos.y) * cell,   // invert Y
                cell,
                cell
            );

            EditorGUI.DrawRect(r, color);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private Color ConvertEnumColor(EnumColor ec)
    {
        return ec switch
        {
            EnumColor.red => Color.red,
            EnumColor.bule => Color.blue,
            EnumColor.yellow => Color.yellow,
            EnumColor.green => Color.green,
            EnumColor.purple => new Color(0.5f, 0f, 0.5f),
            EnumColor.pink => new Color(1f, 0.4f, 0.7f),
            EnumColor.brown => new Color(0.4f, 0.2f, 0f),
            EnumColor.skyblue => new Color(0.3f, 0.7f, 1f),
            EnumColor.orange => new Color(1f, 0.5f, 0f),
            EnumColor.lightgreen => new Color(0.6f, 1f, 0.6f),
            EnumColor.lightpurple => new Color(0.8f, 0.6f, 1f),
            EnumColor.darkgreen => new Color(0f, 0.3f, 0f),
            _ => Color.white,
        };
    }

    // =====================================================================
    // SAVE + DELETE
    // =====================================================================

    private void DrawSaveDeleteButtons()
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
