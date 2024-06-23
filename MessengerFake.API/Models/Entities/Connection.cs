using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MessengerFake.API.Models.Entities
{
    public class Connection
    {
        public Connection()
        {
            
        }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}
