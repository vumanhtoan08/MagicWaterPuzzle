using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Information Grid")]
    private MapData mapData;
    public float spacing = 2f;

    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Camera camObj;

    public void OnStart()
    {
        mapData = LevelManager.Instance.CurrentMap;

        GenGrid();
        GenBorder();
        GenPipe();
        GenBox();
    }

    #region Gen Node and Boder

    [Header("Visual Boder")]
    [SerializeField] private GameObject straightPrefab;
    [SerializeField] private GameObject outerPrefab;
    [SerializeField] private GameObject interPrefab;
    [SerializeField] private Transform nodeHolder;
    [SerializeField] private Transform boderHolder;

    [Header("Outer Border Settings")]
    public OuterBorderConfig topRightOuterConfig;
    public OuterBorderConfig botRightOuterConfig;
    public OuterBorderConfig botLeftOuterConfig;
    public OuterBorderConfig topLeftOuterConfig;

    [Header("Inter Border Settings")]
    public InterBorderConfig topRightConfig;
    public InterBorderConfig botRightConfig;
    public InterBorderConfig botLeftConfig;
    public InterBorderConfig topLeftConfig;

    private void GenGrid()
    {
        for (int i = 0; i < mapData.width; i++)
        {
            for (int j = 0; j < mapData.height; j++)
            {
                NodeData node = mapData.nodes.Find(n => n.x == i && n.y == j);

                if (node == null)
                    continue;

                if (node.isSpawn)
                {
                    Instantiate(nodePrefab, new Vector3(i * spacing, j * spacing), Quaternion.identity, nodeHolder);
                }
            }
        }

        camObj.transform.position = new Vector3(mapData.width - 1, mapData.height - 1, camObj.transform.position.z);
        camObj.orthographicSize = mapData.width * 3;
    }

    private void GenBorder()
    {
        for (int x = 0; x < mapData.width; x++)
        {
            for (int y = 0; y < mapData.height; y++)
            {
                NodeData current = GetNode(x, y);
                if (current == null) continue;

                NodeData top = GetNode(x, y + 1);
                NodeData right = GetNode(x + 1, y);
                NodeData bot = GetNode(x, y - 1);
                NodeData left = GetNode(x - 1, y);

                NodeData topRight = GetNode(x + 1, y + 1);
                NodeData botRight = GetNode(x + 1, y - 1);
                NodeData botLeft = GetNode(x - 1, y - 1);
                NodeData topLeft = GetNode(x - 1, y + 1);

                if (current.isSpawn)
                {
                    // TOP-RIGHT
                    if ((top == null || !top.isSpawn) &&
                        (right == null || !right.isSpawn))
                    {
                        Vector3 pos = new Vector3(
                            x * spacing + topRightOuterConfig.offset.x,
                            y * spacing + topRightOuterConfig.offset.y,
                            0
                        );
                        Instantiate(outerPrefab, pos, Quaternion.Euler(0, 0, topRightOuterConfig.rotationZ), boderHolder);

                        if (botRight == null || !botRight.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing), Quaternion.Euler(0, 0, 90), boderHolder);
                        if (topLeft == null || !topLeft.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing + 1), Quaternion.Euler(0, 0, 180), boderHolder);
                    }

                    // BOT-RIGHT
                    if ((bot == null || !bot.isSpawn) &&
                        (right == null || !right.isSpawn))
                    {
                        Vector3 pos = new Vector3(
                            x * spacing + botRightOuterConfig.offset.x,
                            y * spacing + botRightOuterConfig.offset.y,
                            0
                        );
                        Instantiate(outerPrefab, pos, Quaternion.Euler(0, 0, botRightOuterConfig.rotationZ), boderHolder);

                        if (botLeft == null || !botLeft.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing - 1), Quaternion.Euler(0, 0, 0), boderHolder);
                        if (topRight == null || !topRight.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing + 1), Quaternion.Euler(0, 0, 90), boderHolder);
                    }

                    // BOT-LEFT
                    if ((bot == null || !bot.isSpawn) &&
                        (left == null || !left.isSpawn))
                    {
                        Vector3 pos = new Vector3(
                            x * spacing + botLeftOuterConfig.offset.x,
                            y * spacing + botLeftOuterConfig.offset.y,
                            0
                        );
                        Instantiate(outerPrefab, pos, Quaternion.Euler(0, 0, botLeftOuterConfig.rotationZ), boderHolder);

                        if (topLeft == null || !topLeft.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing), Quaternion.Euler(0, 0, 270), boderHolder);
                        if (botRight == null || !botRight.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing - 1), Quaternion.Euler(0, 0, 0), boderHolder);
                    }

                    // TOP-LEFT
                    if ((top == null || !top.isSpawn) &&
                        (left == null || !left.isSpawn))
                    {
                        Vector3 pos = new Vector3(
                            x * spacing + topLeftOuterConfig.offset.x,
                            y * spacing + topLeftOuterConfig.offset.y,
                            0
                        );
                        Instantiate(outerPrefab, pos, Quaternion.Euler(0, 0, topLeftOuterConfig.rotationZ), boderHolder);

                        if (botLeft == null || !botLeft.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing - 1), Quaternion.Euler(0, 0, 270), boderHolder);
                        if (topRight == null || !topRight.isSpawn)
                            Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing + 1), Quaternion.Euler(0, 0, 180), boderHolder);
                    }

                    if ((top == null || !top.isSpawn) && (topLeft == null || !topLeft.isSpawn) && (topRight == null || !topRight.isSpawn)
                        && (left != null && left.isSpawn) && (right != null && right.isSpawn))
                    {
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing + 1), Quaternion.Euler(0, 0, 180), boderHolder);
                        Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing + 1), Quaternion.Euler(0, 0, 180), boderHolder);
                    }

                    if ((right == null || !right.isSpawn) && (botRight == null || !botRight.isSpawn) && (topRight == null || !topRight.isSpawn)
                        && (top != null && top.isSpawn) && (bot != null && bot.isSpawn))
                    {
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing + 1), Quaternion.Euler(0, 0, 90), boderHolder);
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing), Quaternion.Euler(0, 0, 90), boderHolder);
                    }

                    if ((bot == null || !bot.isSpawn) && (botLeft == null || !botLeft.isSpawn) && (botRight == null || !botRight.isSpawn)
                        && (left != null && left.isSpawn) && (right != null && right.isSpawn))
                    {
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing - 1), Quaternion.Euler(0, 0, 0), boderHolder);
                        Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing - 1), Quaternion.Euler(0, 0, 0), boderHolder);
                    }

                    if ((left == null || !left.isSpawn) && (topLeft == null || !topLeft.isSpawn) && (botLeft == null || !botLeft.isSpawn)
                        && (top != null && top.isSpawn) && (bot != null && bot.isSpawn))
                    {
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing), Quaternion.Euler(0, 0, 270), boderHolder);
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing - 1), Quaternion.Euler(0, 0, 270), boderHolder);
                    }

                    continue;
                }

                // TOP-RIGHT
                if ((top != null && top.isSpawn) &&
                    (right != null && right.isSpawn))
                {
                    Vector3 pos = new Vector3(
                        x * spacing + topRightConfig.offset.x,
                        y * spacing + topRightConfig.offset.y,
                        0
                    );
                    Instantiate(interPrefab, pos, Quaternion.Euler(0, 0, topRightConfig.rotationZ), boderHolder);

                    if (left == null || !left.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing + 1), Quaternion.Euler(0, 0, 0), boderHolder);
                    if (bot == null || !bot.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing - 1), Quaternion.Euler(0, 0, 270), boderHolder);
                }

                // BOT-RIGHT
                if ((bot != null && bot.isSpawn) &&
                    (right != null && right.isSpawn))
                {
                    Vector3 pos = new Vector3(
                        x * spacing + botRightConfig.offset.x,
                        y * spacing + botRightConfig.offset.y,
                        0
                    );
                    Instantiate(interPrefab, pos, Quaternion.Euler(0, 0, botRightConfig.rotationZ), boderHolder);

                    if (top == null || !top.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing), Quaternion.Euler(0, 0, 270), boderHolder);
                    if (left == null || !left.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing - 1), Quaternion.Euler(0, 0, 180), boderHolder);
                }

                // BOT-LEFT
                if ((bot != null && bot.isSpawn) &&
                    (left != null && left.isSpawn))
                {
                    Vector3 pos = new Vector3(
                        x * spacing + botLeftConfig.offset.x,
                        y * spacing + botLeftConfig.offset.y,
                        0
                    );
                    Instantiate(interPrefab, pos, Quaternion.Euler(0, 0, botLeftConfig.rotationZ), boderHolder);

                    if (top == null || !top.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing + 1), Quaternion.Euler(0, 0, 90), boderHolder);
                    if (right == null || !right.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing, y * spacing - 1), Quaternion.Euler(0, 0, 180), boderHolder);
                }

                // TOP-LEFT
                if ((top != null && top.isSpawn) &&
                    (left != null && left.isSpawn))
                {
                    Vector3 pos = new Vector3(
                        x * spacing + topLeftConfig.offset.x,
                        y * spacing + topLeftConfig.offset.y,
                        0
                    );
                    Instantiate(interPrefab, pos, Quaternion.Euler(0, 0, topLeftConfig.rotationZ), boderHolder);

                    if (bot == null || !bot.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing - 1, y * spacing), Quaternion.Euler(0, 0, 90), boderHolder);
                    if (right == null || !right.isSpawn)
                        Instantiate(straightPrefab, new Vector2(x * spacing + 1, y * spacing + 1), Quaternion.Euler(0, 0, 0), boderHolder);
                }
            }
        }
    }

    #endregion

    #region Gen Pipe

    [Header("Pipe REF")]
    [SerializeField] private GameObject pipePrefabs;
    [SerializeField] private Transform pipeHolder;

    private void GenPipe()
    {
        foreach (var pipe in mapData.pipes)
        {
            NodeData node = GetNode(pipe.x, pipe.y);
            GameObject pipeObj;

            if (pipe.y == 0)
            {
                pipeObj = Instantiate(pipePrefabs, new Vector2(pipe.x * spacing, pipe.y * spacing - 2), Quaternion.identity, pipeHolder);
            }
            else if (pipe.x == 0)
            {
                pipeObj = Instantiate(pipePrefabs, new Vector2(pipe.x * spacing - 2, pipe.y * spacing), Quaternion.Euler(0, 0, 270), pipeHolder);
            }
            else if (pipe.y == mapData.height - 1)
            {
                pipeObj = Instantiate(pipePrefabs, new Vector2(pipe.x * spacing, pipe.y * spacing + 2), Quaternion.Euler(0, 0, 180), pipeHolder);
            }
            else if (pipe.x == mapData.width - 1)
            {
                pipeObj = Instantiate(pipePrefabs, new Vector2(pipe.x * spacing + 2, pipe.y * spacing), Quaternion.Euler(0, 0, 90), pipeHolder);
            }
            else
            {
                // fallback
                pipeObj = Instantiate(pipePrefabs, new Vector2(pipe.x * spacing, pipe.y * spacing), Quaternion.identity, pipeHolder);
            }

            PipeBase pipeBase = pipeObj.GetComponent<PipeBase>();
            pipeBase.GetPipeData(pipe);
            pipeBase.GenWater();
        }
    }

    #endregion

    #region Gen Box

    [Header("Holder REF")]
    [SerializeField] private Transform boxHolder; 

    private void GenBox()
    {
        foreach (var holder in mapData.holders)
        {
            NodeData node = GetNode(holder.x, holder.y);
            GameObject holderObj;

            holderObj = Instantiate(SOBoxPrefabs.GetBoxPrefabs(holder.shapeType),
                new Vector3(holder.x * spacing, holder.y * spacing), Quaternion.Euler(0,0,holder.rotation) ,boxHolder);

            BoxVisual boxVisual = holderObj.GetComponent<BoxVisual>();
            boxVisual.OnUpdateVisualOfHolderType(holder);

            BoxHandleCollider boxHandleCollider = holderObj.GetComponent<BoxHandleCollider>();
            boxHandleCollider.GetHolderData(holder);
            LevelManager.Instance.AddBoxWater(boxHandleCollider);
        } 
    }

    #endregion

    private NodeData GetNode(int x, int y)
    {
        return mapData.nodes.Find(n => n.x == x && n.y == y);
    }

}

[System.Serializable]
public class OuterBorderConfig
{
    public Vector2 offset;
    public float rotationZ;
}


[System.Serializable]
public class InterBorderConfig
{
    public Vector2 offset;   // offset theo spacing
    public float rotationZ;  // góc xoay
}
