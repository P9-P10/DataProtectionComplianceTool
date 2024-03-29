﻿using System.Text.RegularExpressions;

namespace GraphManipulation.Utility;

public static class TypeToString
{
    public static string GetEntityType(Type type)
    {
        return Regex.Replace(type.Name, "([a-z])([A-Z])", "$1 $2").ToLower();
    }
}