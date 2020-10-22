namespace RneSniffer.Helpers
{
    using System.Configuration;
    using System.Drawing;

    public static class ConfigHelper
    {
        public static string GetStringValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return value;
        }

        public static bool GetBoolValue(string key)
        {
            bool value = bool.Parse(GetStringValue(key));
            return value;
        }
        public static int GetIntValue(string key)
        {
            int value = int.Parse(GetStringValue(key));
            return value;
        }


        public static Size? GetSizeValue(string key)
        {
            var values = GetStringValue(key).Split('*');
            if (values != null && values.Length == 2)
            {
                Size value = new Size(int.Parse(values[0]), int.Parse(values[1]));
                return value;
            }
            return null;
        }

        public static string GetCodeLocation()
        {
            var codeLocation = System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            return codeLocation;
        }
    }
}
