public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } // Nombre de Usuario
    public string Email { get; set; } // Email del Usuario
    public string Password { get; set; } // Contrasena 
    public Role role{ get; set; } // Rol del Usuario
}