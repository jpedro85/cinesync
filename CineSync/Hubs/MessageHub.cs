using CineSync.Data.Models;
using Microsoft.AspNetCore.SignalR;

namespace CineSync.Hubs
{
	public class MessageHub : Hub, IRoom
	{
		public Task JoinRoom( string roomName )
		{
			return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
		}

		public Task LeaveRoom( string roomName )
		{
			return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
		}

		public async Task UpdateYourRequestState(Invite invite) 
		{
			Console.WriteLine("CalledOnServer");
            await Clients.Others.SendAsync("UpdateMyRequestState", invite);
        }

        public async Task UpdateMyRequestState(Invite invite)
        {
            Console.WriteLine("CalledOnServer2");
            await Clients.Others.SendAsync("UpdateYourRequestState", invite);
        }

        public async Task test()
        {
            Console.WriteLine("CalledOnServer3");
        }

    }
}