using Microsoft.EntityFrameworkCore;
using VoiceChatMVC.Models;

namespace VoiceChatMVC.Data
{

    public class CallLogDbContext : DbContext
    {
        public CallLogDbContext(DbContextOptions<CallLogDbContext> options) : base(options) { }

        public DbSet<CallLog> CallLogs { get; set; }

    }
}
