using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VMTExtentions
{
    /// <summary>
    /// Hàm Check Null với Component <br/>
    /// Cách dùng: Viết Component + gọi hàm <br/>
    /// VD: Rigidbody.IsNull("Chưa gán Rigidbody!")
    /// </summary>
    public static bool IsNull(this Component comp, string message)
    {
        if (comp == null)
        {
            Debug.LogWarning(message);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Hàm check đủ giá trị hay không <br/>
    /// Cách dùng: Viết biến + gọi hàm <br/>
    /// VD: var a = 10; a.IsGreaterThanZero(); 
    /// </summary>
    public static bool IsGreaterThanZero<T>(this T value) where T : struct, System.IComparable
    {
        return value.CompareTo(0) > 0;
    }
}