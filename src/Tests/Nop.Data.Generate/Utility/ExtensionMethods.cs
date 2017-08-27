using System;

public static class ExtensionMethods
{
    public static string UppercaseFirstLetter(this string value)
    {
        //
        // Uppercase the first letter in the string.
        //
        if (value.Length > 0)
        {
            char[] array = value.ToCharArray();
            array[0] = char.ToUpper(array[0]);
            return new string(array);
        }
        return value;
    }

    public static string GetSubStr(this string value, String StartStr, String EndStr, Boolean StartEndStrInclude = false)
    {
        int startInt = value.IndexOf(StartStr);
        value = value.Substring(startInt);
        if (!StartEndStrInclude)
            value = value.Substring(StartStr.Length);
        if (StartEndStrInclude)
            value = value.Substring(0, value.IndexOf(EndStr) + 1);
        else
            value = value.Substring(0, value.IndexOf(EndStr));
        return value;
    }

    public static Int16 ToShort(this Int32 value)
    {
        return Convert.ToInt16(value);
    }

    public static Int16 ToShort(this object value)
    {
        return Convert.ToInt16(value);
    }

    public static Int32 ToInt(this object value)
    {
        return Convert.ToInt32(value);
    }

    public static Int64 ToLong(this object value)
    {
        return Convert.ToInt64(value);
    }

    public static Int16 ToShort(this String value)
    {
        return Convert.ToInt16(value);
    }

    public static Int32 ToInt(this String value)
    {
        return Convert.ToInt32(value);
    }

    public static Int64 ToLong(this String value)
    {
        return Convert.ToInt64(value);
    }
}