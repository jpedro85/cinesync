using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
	public partial class PopUpParticipants
	{
		static int counter = 0;
		private string _Id = string.Empty;

		[Parameter,EditorRequired]
		public ApplicationUser ApplicationUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public Conversation Conversation { get; set; } = default!;

		[Parameter, EditorRequired]
		public EventCallback OnClickInvite { get; set; } = default!;


		[Inject]
		private ConversationManager ConversationManager { get; set; } = default!;

		[Inject]
		private InvitesManager InvitesManager { get; set; } = default!;

		private PopUpLayout _popUpLayout = default!;

		protected override void OnInitialized()
		{
			counter++;
			_Id = counter.ToString();
		}

		public void Open() 
		{
			_popUpLayout.Open();
		}

		public void Close() 
		{
			_popUpLayout.Close();
		}

		private async void OnClickRemoveInvite(Invite invite) 
		{
			Conversation.Invites.Remove(invite);
			await ConversationManager.UpdateEntity(Conversation, "Invites");
		}

		private async void OnClickLeave(ApplicationUser user) 
		{
			Invite invite = Conversation.Invites.Where(i=>i.Target.Equals(user)).First();

			if(user.Id == Conversation.OwnerId && Conversation.Participants.Count > 1) 
			{
				foreach(var uc in Conversation.Participants) 
				{
					if (uc.User.Equals(user)) 
					{
						Conversation.Participants.Remove(uc);
						break;
					}
				}

				Conversation.OwnerId = Conversation.Participants.First().UserId;

				Conversation.Invites.Remove(invite);
				await ConversationManager.UpdateEntity(Conversation, "OwnerId", "Participants", "Invites");
			}
			else if (user.Id == Conversation.OwnerId) 
			{
				await ConversationManager.RemoveAsync(Conversation);
			}
			else 
			{
				foreach (var uc in Conversation.Participants)
				{
					if (uc.User.Equals(user))
					{
						Conversation.Participants.Remove(uc);
						break;
					}
				}

				Conversation.Invites.Remove(invite);
				await ConversationManager.UpdateEntity(Conversation, "Participants", "Invites");
			}

		}

		private async void OnClickAdd() 
		{
			_popUpLayout.Close();
			await OnClickInvite.InvokeAsync();
		}
	}
}
