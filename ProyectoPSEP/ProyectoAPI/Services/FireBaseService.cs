using Google.Cloud.Firestore;
using FirebaseAdmin;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;


    public class FireBaseService
    {
        private FirestoreDb _firestoreDb;

        public FireBaseService()
        {
            string credentialsPath = Path.Combine(Directory.GetCurrentDirectory(), "FireBase", "proyectopsep-firebase-adminsdk-fbsvc-ed38ba0352.json");

            // Check if file exists before setting the variable
            if (!File.Exists(credentialsPath))
            {
                throw new FileNotFoundException("Firebase credentials file not found.", credentialsPath);
            }

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            // Ensure FirebaseApp is created only once
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }

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
            var userRef = _firestoreDb.Collection("User");
            var query = userRef.WhereEqualTo("UserName", username);

            // Execute the query and get the first matching document (if it exists)
            var snapshot = await query.Limit(1).GetSnapshotAsync();

            // Check if any document was found
            if (snapshot.Documents.Count > 0)
            {
                var docRef = snapshot.Documents.First().Reference;
                await docRef.UpdateAsync(updatedFields); // Return the first document found
            }
            
            
        }

        public async Task DeleteUser(string username)
        {
            var userRef = _firestoreDb.Collection("User");
            var query = userRef.WhereEqualTo("UserName", username);

            // Execute the query and get the first matching document (if it exists)
            var snapshot = await query.Limit(1).GetSnapshotAsync();

            // Check if any document was found
            if (snapshot.Documents.Count > 0)
            {
                var docRef = snapshot.Documents.First().Reference;
                await docRef.DeleteAsync(); // Return the first document found
            }
            
        }

        public async Task<List<Game>> GetGames()
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
            
            var batch = _firestoreDb.StartBatch();

            foreach (var game in games)
            {
                var gameRef = _firestoreDb.Collection("Games").Document(game.Id.ToString());
                batch.Set(gameRef, game);
            }

            await batch.CommitAsync();
        }

        public async Task DeleteGame(string gameId)
        {
            
            var gameRef = _firestoreDb.Collection("Games").Document(gameId);
            await gameRef.DeleteAsync();
        }

        public async Task DeleteMultipleGames(List<int> gameIds)
        {
           
            var batch = _firestoreDb.StartBatch();

            foreach (var gameId in gameIds)
            {
                var gameRef = _firestoreDb.Collection("Games").Document(gameId.ToString());
                batch.Delete(gameRef);
            }

            await batch.CommitAsync();
        }

        public async Task UpdateGame(string gameId, Dictionary<string, object> updates)
        {
            
            var gameRef = _firestoreDb.Collection("Games").Document(gameId);
            await gameRef.UpdateAsync(updates);
        }

    }
    
