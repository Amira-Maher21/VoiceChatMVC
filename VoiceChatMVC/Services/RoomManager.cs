using VoiceChatMVC.Models;
using System.Collections.Concurrent;

namespace VoiceChatMVC.Services;
 
public class RoomManager
{
    private readonly ConcurrentDictionary<string, Room> _rooms = new();

     
    public Room CreateOrGetRoom(string roomName)
    {
        return _rooms.GetOrAdd(roomName, rn => new Room(rn));
    }

     
    public bool RoomExists(string roomName)
    {
        return _rooms.ContainsKey(roomName);
    }

     
    public Room? GetRoom(string roomName)
    {
        _rooms.TryGetValue(roomName, out var room);
        return room;
    }

     
    public void RemoveEmptyRoom(string roomName)
    {
        if (_rooms.TryGetValue(roomName, out var room) && room.Users.IsEmpty)
        {
            _rooms.TryRemove(roomName, out _);
        }
    }

     
    public void RemoveUserFromAllRooms(string connectionId)
    {
        foreach (var room in _rooms.Values)
        {
            if (room.RemoveUser(connectionId))
            {
                RemoveEmptyRoom(room.RoomName);
            }
        }
    }

     
    public Room? GetUserRoom(string connectionId)
    {
        return _rooms.Values.FirstOrDefault(r => r.Users.ContainsKey(connectionId));
    }
}
