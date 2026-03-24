using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebQuanLyNhanSu;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
        var display = member?.GetCustomAttribute<DisplayAttribute>();
        return display?.Name ?? enumValue.ToString();
    }
}
