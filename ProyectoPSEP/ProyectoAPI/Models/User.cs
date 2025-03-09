using Google.Cloud.Firestore;

[FirestoreData]
public class User
{
    [FirestoreProperty]
    public int Id { get; set; }

    [FirestoreProperty]
    public string UserName { get; set; } // Nombre de Usuario

    [FirestoreProperty]
    public string Email { get; set; } // Email del Usuario

    [FirestoreProperty]
    public string Password { get; set; } // Contraseña  

    [FirestoreProperty]
    public string Role { get; set; } // Rol del Usuario  

    // Ensure a parameterless constructor
    public User() { }
}

