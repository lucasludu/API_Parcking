namespace Models.Request._user
{
    public class RegisterUserRequest
    {
        // Datos del Usuario
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        // Datos de la Cochera (Opcionales: si se envían, es un registro de Dueño)
        public string? NombreCochera { get; set; }
        public string? DireccionCochera { get; set; }
        public int? CapacidadCochera { get; set; }

        // Opcional: Para el caso de empleados que se unen a una cochera existente
        public Guid? CocheraIdExistente { get; set; }
    }
}
