using System;
using System.Linq;

/// <summary>
/// Magic bullshit for java-style enums
/// </summary>
public static class EnumExtensions {

    public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        return type.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
    }
}
