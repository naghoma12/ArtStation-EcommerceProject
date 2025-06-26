using ArtStation.Core.Entities.Identity;
using System.Reflection;
using System.Runtime.Serialization;

namespace ArtStation.Helper
{
    public static class EnumHelper
    {
        //convert from enum to string
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

        //convert from string to enum
        public static Gender? ParseGender(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return Enum.TryParse(typeof(Gender), input, true, out var result)
                ? (Gender?)result
                : null;
        }
    }
}
