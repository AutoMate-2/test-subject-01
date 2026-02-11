namespace to_integrations.HelperMethods
{
    public static class TokenCache
    {
        private static string _cachedToken = "";

        public static string CachedToken
        {
            get => _cachedToken;
            set => _cachedToken = value;
        }

        public static void Clear()
        {
            _cachedToken = "";
        }

        public static bool HasToken => !string.IsNullOrEmpty(_cachedToken);
    }
}
