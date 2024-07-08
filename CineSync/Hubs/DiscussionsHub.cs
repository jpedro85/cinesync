
using CineSync.Data.Models;
using Microsoft.AspNetCore.SignalR;

namespace CineSync.Hubs
{
    public class DiscussionsHub : Hub , IRoom
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public Task NotifyGroupUpdateDiscussion(string roomName, int nLikes, int nDislikes)
        {
            return Clients.OthersInGroup(roomName).SendAsync("Update", roomName, nLikes, nDislikes);
        }

        public Task NotifyGroupNewComment(string roomName, uint commentId )
        {
            return Clients.OthersInGroup(roomName).SendAsync("NewComment", roomName, commentId);
        }

        public Task NotifyGroupRemovedComment(string roomName, uint commentId )
        {
            return Clients.OthersInGroup(roomName).SendAsync("RemoveComment", roomName, commentId );
        }

        //public Task NotifyGroupUpdateComment(string roomName, uint commentId, int nLikes , int nDislikes)
        //{
        //    return Clients.OthersInGroup(roomName).SendAsync("UpdateComment", roomName, commentId, nLikes, nDislikes);
        //}

        public async Task test()
        {
            Console.WriteLine("CalledOnServer2");
        }

    }
}
