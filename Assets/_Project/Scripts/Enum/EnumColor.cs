using System.Collections.Generic;
using UnityEngine;

public enum EnumColor
{
    None = 0,
    red = 1,
    bule = 2,
    yellow = 3,
    green = 4,
    purple = 5,
    pink = 6,
    brown = 7,
    skyblue = 8,
    orange = 9,
    lightgreen = 10,
    lightpurple = 11,
    darkgreen = 12,
}

//// Database shape offsets (tọa độ relative từ ô gốc)
//private static readonly Dictionary<HolderShape, Vector2Int[]> shapeOffsets =
//    new Dictionary<HolderShape, Vector2Int[]>
//    {
//            // 1 ô
//            { HolderShape.One, new[]
//                {
//                    new Vector2Int(0, 0)
//                }
//            },

//            // 2 ô: mặc định dọc (gốc + trên)
//            { HolderShape.Two, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(0, 1)
//                }
//            },

//            // 3 ô: mặc định dọc (gốc, trên, dưới)
//            { HolderShape.Three, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(0, 1),
//                    new Vector2Int(0, -1)
//                }
//            },

//            // L ngắn: 2 ô hình góc
//            { HolderShape.ShortL, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(-1, 0),
//                    new Vector2Int(0, -1),
//                }
//            },

//            // T ngắn: gốc ở giữa
//            { HolderShape.ShortT, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(-1, 0),
//                    new Vector2Int(1, 0),
//                    new Vector2Int(0, -1),
//                }
//            },

//            // L 3 ô
//            { HolderShape.L, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(-1,0),
//                    new Vector2Int(0, -1),
//                    new Vector2Int(0, -2),
//                }
//            },

//            // Reverse L 3 ô
//            { HolderShape.ReverseL, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(1,0),
//                    new Vector2Int(0, -1),
//                    new Vector2Int(0, -2),
//                }
//            },

//            // 3x3
//            { HolderShape.ThreeSquare, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(1, 0),
//                    new Vector2Int(-1, 0),
//                    new Vector2Int(0, 1),
//                    new Vector2Int(-1, 1),
//                    new Vector2Int(1, 1),
//                    new Vector2Int(-1, -1),
//                    new Vector2Int(0, -1),
//                    new Vector2Int(1, -1),
//                }
//            },

//            // 1x2
//            { HolderShape.TwoSquare, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(1, 0),
//                    new Vector2Int(0, 1),
//                    new Vector2Int(1, 1),
//                }
//            },

//            // Hình dấu +
//            { HolderShape.Plus, new[]
//                {
//                    new Vector2Int(0, 0),
//                    new Vector2Int(0, 1),
//                    new Vector2Int(0, -1),
//                    new Vector2Int(1, 0),
//                    new Vector2Int(-1, 0)
//                }
//            },
//    };
