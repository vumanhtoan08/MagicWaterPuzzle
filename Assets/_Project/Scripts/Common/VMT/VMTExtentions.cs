using System;
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

    #region Compare
    /// <summary>
    /// So sánh giá trị hiện tại với một giá trị khác.
    /// Trả về:
    ///  -1 nếu nhỏ hơn
    ///   0 nếu bằng nhau
    ///   1 nếu lớn hơn
    /// </summary>
    public static int CompareToEx<T>(this T value, T other) where T : IComparable
    {
        return value.CompareTo(other);
    }

    /// <summary>
    /// Kiểm tra giá trị nhỏ hơn giá trị khác
    /// </summary>
    public static bool LessThan<T>(this T value, T other) where T : IComparable
    {
        return value.CompareTo(other) < 0;
    }

    /// <summary>
    /// Kiểm tra giá trị lớn hơn giá trị khác
    /// </summary>
    public static bool GreaterThan<T>(this T value, T other) where T : IComparable
    {
        return value.CompareTo(other) > 0;
    }

    /// <summary>
    /// Kiểm tra giá trị nhỏ hơn hoặc bằng
    /// </summary>
    public static bool LessOrEqual<T>(this T value, T other) where T : IComparable
    {
        return value.CompareTo(other) <= 0;
    }

    /// <summary>
    /// Kiểm tra giá trị lớn hơn hoặc bằng
    /// </summary>
    public static bool GreaterOrEqual<T>(this T value, T other) where T : IComparable
    {
        return value.CompareTo(other) >= 0;
    }
    #endregion
}