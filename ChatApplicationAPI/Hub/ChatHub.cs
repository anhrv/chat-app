using ChatApplicationAPI.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationAPI.Hub
{
	public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
	{
		private readonly IDictionary<string, UserRoomConnection> _connection;

        public ChatHub(IDictionary<string, UserRoomConnection> connection)
        {
            _connection = connection;
        }

        public async Task JoinRoom(UserRoomConnection userRoomConnection)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, userRoomConnection.Room!);
			_connection[Context.ConnectionId] = userRoomConnection;
			await Clients.Group(userRoomConnection.Room!).SendAsync("RecieveMessage", "admin", $"{userRoomConnection.User} has joined the chat", DateTime.Now);
			await SendConnectedUsers(userRoomConnection.Room!);
		}

		public async Task SendMessage(string message)
		{
			if(_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection userRoomConnection))
			{
				await Clients.Group(userRoomConnection.Room!).SendAsync("RecieveMessage", userRoomConnection.User, message, DateTime.Now);
			}
		}

		public override Task OnDisconnectedAsync(Exception? exception)
		{
			if (!_connection.TryGetValue(Context.ConnectionId, out UserRoomConnection userRoomConnection))
			{
				return base.OnDisconnectedAsync(exception);
			}
			_connection.Remove(Context.ConnectionId);
			Clients.Group(userRoomConnection.Room!).SendAsync("RecieveMessage", "admin", $"{userRoomConnection.User} has left the chat", DateTime.Now);
			SendConnectedUsers(userRoomConnection.Room!);
			return base.OnDisconnectedAsync(exception);
		}

		public Task SendConnectedUsers(string room)
		{
			var users = _connection.Values.Where(u => u.Room == room).Select(s => s.User);
			return Clients.Group(room).SendAsync("ConnectedUsers", users);
		}
	}
}
