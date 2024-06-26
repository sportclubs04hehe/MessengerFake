namespace MessengerFake.API.SignalR
{
    public class PresenceTracker
    {
        // Khai báo một Dictionary static và readonly để lưu trữ người dùng trực tuyến và danh sách ID kết nối của họ.
        private static readonly Dictionary<string, List<string>> OnlineUsers = [];

        // Phương thức này được gọi khi một người dùng kết nối.
        public Task<bool> UserConnected(string username, string connectionId)
        {
            // Biến này để theo dõi xem người dùng có phải là người dùng mới trực tuyến không.
            var isOnline = false;

            // Khóa đối tượng từ điển để đảm bảo chỉ có một luồng có thể truy cập vào khối mã này cùng lúc.
            lock (OnlineUsers)
            {
                // Kiểm tra xem Dictionary có chứa khóa là tên người dùng này không.
                if (OnlineUsers.ContainsKey(username))
                {
                    // Nếu có, thêm ID kết nối vào danh sách ID kết nối của người dùng.
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    // Nếu không, thêm một mục mới vào Dictionary với key là tên người dùng và value là danh sách chứa ID kết nối.
                    OnlineUsers.Add(username, [connectionId]);
                    isOnline = true;             // Đặt biến isOnline thành true để chỉ ra rằng người dùng này vừa trực tuyến.
                }
            }

            // Trả về một nhiệm vụ chứa kết quả là giá trị của isOnline.
            return Task.FromResult(isOnline);
        }

        // Method này được gọi khi người dùng ngắt kết nối
        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            // variable này theo dõi xem người dùng có offline hay không?
            var isOffline = false;

            // Lock the dictionary đảm bảo mỗi lần chỉ có một luồng truy cập khối này
            lock (OnlineUsers)
            {
                // Nếu dictionary không chứa usename này thì trả về offline = false (nghĩa là online)
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                // Xóa connectionId khỏi danh sách người dùng
                OnlineUsers[username].Remove(connectionId);

                // Nếu danh sách IDs kết nối trống, xóa nguời dùng khỏi dictionary
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true; // đặt trạng thái offline thành true
                }
            }

            return Task.FromResult(isOffline);  
        }

        // Method này trả về một mảng tên người dùng trực tuyến.
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        // Method này trả về id kết nối với một người dùng được chỉ định
        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            // variable để giữ id kết nối
            List<string> connectionIds;

            // lấy d/s kết nối với người dùng chỉ định
            if (OnlineUsers.TryGetValue(username, out var connections))
            {
                // Khóa danh sách kết nối để đảm bảo an toàn cho luồng khi truy cập.
                lock (connections)
                {
                    // Tạo một bản sao của danh sách kết nối để tránh các vấn đề khi sửa đổi đồng thời.
                    connectionIds = connections.ToList();
                }
            }
            else
            {
                // If the user is not found in the dictionary, return an empty list.
                connectionIds = [];
            }

            return Task.FromResult(connectionIds);
        }


    }
}
