namespace Application.Constants
{
    public static class RolesConstants
    {
        public static string Admin => "Admin";
        public static string Usuario => "User";

        public static List<string> ValidRoles => new List<string>
        {
            Admin,
            Usuario
        };
    }
}
