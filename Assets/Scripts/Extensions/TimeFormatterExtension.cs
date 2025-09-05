using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class TimeFormatterExtension
{
    public static string FormatSaveTime(string timeString)
    {
        if (string.IsNullOrEmpty(timeString))
            return string.Empty;

        if (DateTime.TryParse(timeString, null, DateTimeStyles.RoundtripKind, out var dt))
        {
            // return dt.ToString("dd/MM/yyyy HH:mm:ss");
            return dt.ToString("HH:mm:ss - dd/MM/yyyy ");
            // đổi format theo ý: "yyyy-MM-dd HH:mm:ss" hoặc "dd MMM yyyy HH:mm"
        }

        return timeString; // fallback nếu parse lỗi
    }
}
