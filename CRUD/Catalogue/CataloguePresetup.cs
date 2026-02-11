namespace to_integrations.CRUD.Catalogue
{
    public class CataloguePresetup
    {
        public static string ValidToken { get; set; } = "";
        public static string ExpiredToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkyMjJ9.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        public static void InitializeTokens(string validToken)
        {
            ValidToken = validToken;
        }
    }
}
