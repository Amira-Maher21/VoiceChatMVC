using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VoiceChatMVC.Models;
using VoiceChatMVC.Services;

namespace VoiceChatMVC.Hubs
{
    public class ChatHub : Hub
    {
        private readonly RoomManager _roomManager;
        private readonly CallLogService _callLogService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(RoomManager roomManager, CallLogService callLogService, ILogger<ChatHub> logger)
        {
            _roomManager = roomManager;
            _callLogService = callLogService;
            _logger = logger;
        }

        private Task SendUserListToRoom(string roomName)
        {
            var room = _roomManager.GetRoom(roomName);
            if (room == null) return Task.CompletedTask;

            var users = room.Users
                .Select(u => new { ConnectionId = u.Key, Username = u.Value })
                .ToList();

            return Clients.Group(room.RoomName).SendAsync("UserList", users);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var room = _roomManager.GetUserRoom(Context.ConnectionId);
            _roomManager.RemoveUserFromAllRooms(Context.ConnectionId);

            if (room != null)
            {
                await Clients.Group(room.RoomName).SendAsync("UserLeft", Context.ConnectionId);
                await SendUserListToRoom(room.RoomName);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendSignal(string targetConnectionId, string type, object data)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveSignal", Context.ConnectionId, type, data);
        }

        public async Task JoinRoom(string username, string roomName)
        {
            var room = _roomManager.CreateOrGetRoom(roomName);
            room.AddUser(Context.ConnectionId, username);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserJoined", Context.ConnectionId, username);
            await SendUserListToRoom(roomName);
        }

        public async Task<int> StartCallLog(string targetConnectionId)
        {
            var room = _roomManager.GetUserRoom(Context.ConnectionId);
            if (room == null)
            {
                _logger.LogWarning("StartCallLog failed: room is null.");
                return -1;
            }

            if (!room.Users.TryGetValue(Context.ConnectionId, out var caller) ||
                !room.Users.TryGetValue(targetConnectionId, out var receiver))
            {
                _logger.LogWarning("StartCallLog failed: one or both users not found in the room.");
                return -1;
            }

            int callId = await _callLogService.StartCallAsync(caller, receiver, room.RoomName);
            _logger.LogInformation("Call started between {Caller} and {Receiver} in room {RoomName} with CallId {CallId}", caller, receiver, room.RoomName, callId);

            return callId;
        }

        public async Task EndCallLog(int callId)
        {
            await _callLogService.EndCallAsync(callId);
            _logger.LogInformation("Call ended with CallId {CallId}", callId);
        }
    }
}
