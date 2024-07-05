namespace CineSync.Hubs
{
	public interface IRoom
	{
		public Task JoinRoom( string roomName );

		public Task LeaveRoom( string roomName );

	}
}
