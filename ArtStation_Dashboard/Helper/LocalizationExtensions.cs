namespace ArtStation_Dashboard.Helper
{
    public static class LocalizationExtensions
    {
        public static string Localize(this (string Ar, string En) pair, string language)
        {
            return language == "en" ? pair.En : pair.Ar;
        }
    }
}
