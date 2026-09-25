namespace Pes
{
    /// <summary>
    /// Codes d'action enregistrés dans le journal d'audit (table AuditLogs).
    /// </summary>
    public static class AuditActions
    {
        // Captures automatiques (AuditInterceptor).
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";

        // Événements d'authentification (AccountController).
        public const string Login = "Login";
        public const string LoginFailed = "LoginFailed";
        public const string Logout = "Logout";
        public const string Register = "Register";
        public const string PasswordChange = "PasswordChange";

        // Gestion des utilisateurs et des rôles (SecurityService).
        public const string UserCreate = "UserCreate";
        public const string UserUpdate = "UserUpdate";
        public const string UserDelete = "UserDelete";
        public const string RoleCreate = "RoleCreate";
        public const string RoleDelete = "RoleDelete";
    }
}
