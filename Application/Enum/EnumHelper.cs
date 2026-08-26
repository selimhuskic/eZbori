namespace Application.Enum
{
    public static class EnumHelper
    {
        public static T GetAdequateEnum<T>(string valueToParse) where T : struct
        {
            System.Enum.TryParse(valueToParse, out T value);

            return value;
        }
    }
}
