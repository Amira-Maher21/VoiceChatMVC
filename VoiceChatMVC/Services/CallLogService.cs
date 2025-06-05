using VoiceChatMVC.Models;


using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VoiceChatMVC.Data;

namespace VoiceChatMVC.Services
{





    // Services/CallLogService.cs
    public class CallLogService
    {
        private readonly CallLogDbContext _context;

        public CallLogService(CallLogDbContext context)
        {
            _context = context;
        }

        public async Task<int> StartCallAsync(string caller, string receiver, string roomName)
        {
            var callLog = new CallLog
            {
                Caller = caller,
                Receiver = receiver,
                RoomName = roomName,
                StartTime = DateTime.UtcNow
            };
            _context.CallLogs.Add(callLog);
            await _context.SaveChangesAsync();
            return callLog.Id;
        }

        public async Task EndCallAsync(int callId)
        {
            var call = await _context.CallLogs.FindAsync(callId);
            if (call != null && call.EndTime == null)
            {
                call.EndTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
