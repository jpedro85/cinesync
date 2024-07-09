using CineSync.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Utilities;
using System.Collections.Concurrent;
using System.Reflection;

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

		public Task NotifyGroupNewMessage( string roomName, uint messageId )
        {
			return Clients.OthersInGroup(roomName).SendAsync( "UpdateMessages" , messageId);
		}

		public Task NotifyGroupNewReaction(string roomName, uint messageId, string reaction)
		{
			return Clients.OthersInGroup(roomName).SendAsync("UpdateReaction", messageId, reaction);
		}

		public async Task UpdateYourRequestState(Invite invite) 
		{
            await Clients.Others.SendAsync("UpdateMyRequestState", invite);
        }

        public async Task UpdateMyRequestState(Invite invite)
        {
            await Clients.Others.SendAsync("UpdateYourRequestState", invite);
        }

        public async Task test()
        {
            Console.WriteLine("CalledOnServer3");
        }

    }
}