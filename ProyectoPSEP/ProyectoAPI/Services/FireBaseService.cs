using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoAPI.Services
{
    public class FirebaseService
    {
        private FirestoreDb _firestoreDb;

        public FirebaseService()
        {
            _firestoreDb = FirestoreDb.Create("proyectopsep"); // Replace with your Firebase project ID
        }

        public async Task<List<User>> GetUsers()
        {
            CollectionReference usersRef = _firestoreDb.Collection("Users");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            List<User> users = new List<User>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    users.Add(doc.ConvertTo<User>());
                }
            }
            return users;
        }

        public async Task<User> GetUser(string username)
        {
            DocumentReference usersRef = _firestoreDb.Collection("Users").Document(username);
            DocumentSnapshot snapshot = await usersRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<User>();
            }
            return null;
        }

        public async Task AddUser(User user)
        {
            DocumentReference userRef = _firestoreDb.Collection("Users").Document(user.Id.ToString());
            await userRef.SetAsync(user);
        }

        public async Task UpdateUser(string username, Dictionary<string, object> updatedFields)
        {
            DocumentReference userRef = _firestoreDb.Collection("Users").Document(username);
            await userRef.UpdateAsync(updatedFields);
        }

        public async Task DeleteUser(string username)
        {
            DocumentReference userRef = _firestoreDb.Collection("Users").Document(username);
            await userRef.DeleteAsync();
        }

        public async Task<List<Games>> GetGames()
        {
            CollectionReference gamesRef = _firestoreDb.Collection("Games");
            QuerySnapshot snapshot = await gamesRef.GetSnapshotAsync();
            List<Game> games = new List<Game>();

            foreach (DocumentSnapshot doc in snapshot.Documents)
            {
                if (doc.Exists)
                {
                    games.Add(doc.ConvertTo<Game>());
                }
            }
            return games;
        }

        public async Task<Game> GetGame(string gameId)
        {
            DocumentReference gamesRef = _firestoreDb.Collection("Games").Document(gameId);
            DocumentSnapshot snapshot = await gamesRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Game>();
            }
            return null;
        }

        public async Task AddGame(Game game)
        {
            DocumentReference gameRef = _firestoreDb.Collection("Games").Document(game.Id.ToString());
            await gameRef.SetAsync(game);
        }

        public async Task AddMultipleGames(List<Game> games)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var batch = db.StartBatch();

            foreach (var game in games)
            {
                var gameRef = db.Collection("Games").Document(game.Id.ToString());
                batch.Set(gameRef, game);
            }

            await batch.CommitAsync();
        }

        public async Task DeleteGame(string gameId)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var gameRef = db.Collection("Games").Document(gameId);
            await gameRef.DeleteAsync();
        }

        public async Task DeleteMultipleGames(List<string> gameIds)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var batch = db.StartBatch();

            foreach (var gameId in gameIds)
            {
                var gameRef = db.Collection("Games").Document(gameId);
                batch.Delete(gameRef);
            }

            await batch.CommitAsync();
        }

        public async Task UpdateGame(string gameId, Dictionary<string, object> updates)
        {
            var db = FirebaseFirestore.DefaultInstance;
            var gameRef = db.Collection("Games").Document(gameId);
            await gameRef.UpdateAsync(updates);
        }

    }
    

}
