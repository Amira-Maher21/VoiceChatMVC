using System.Collections.Concurrent;

 
    public class Room(string roomName)
    {
        public string RoomName { get; } = roomName;

         public ConcurrentDictionary<string, string> Users { get; } = new();

        
        /// <param name="connectionId">The connection ID.</param>
         public bool RemoveUser(string connectionId)
        {
            return Users.TryRemove(connectionId, out _);
        }

         
        public void AddUser(string connectionId, string username)
        {
            Users[connectionId] = username;
        }
    }
