using System.Reflection;
using System.Runtime.Serialization;

namespace ArtStation.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumMemberValue(Enum enumValue)
        {
            if (enumValue == null)
                return null;

            var memberInfo = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault();

            var attribute = memberInfo?
                .GetCustomAttribute<EnumMemberAttribute>();

            return attribute?.Value ?? enumValue.ToString(); // fallback to enum name if no attribute
        }
    }
}
