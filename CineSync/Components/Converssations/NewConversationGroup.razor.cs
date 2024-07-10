using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.IdentityModel.Tokens;

namespace CineSync.Components.Converssations
{
	public partial class NewConversationGroup
    {
		[Parameter,EditorRequired]
		public ApplicationUser AuthenticatedUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public  EventCallback<Conversation> OnCreateConversation { get; set; } = default!;

		[Parameter, EditorRequired]
		public EventCallback OnCancel { get; set; } = default!;


		[Inject]
		public ConversationManager ConversationManager { get; set; } = default!;

		[Inject]
		public InvitesManager InvitesManager { get; set; } = default!;



		private string _error = string.Empty;
		private string _invitesError = string.Empty;
		private string _groupName = string.Empty;
		private Dictionary<ApplicationUser,bool> _usersToInvite = new Dictionary<ApplicationUser, bool>();
		private List<string> _usersIds = new List<string>();
		private PopUpSearchUser _popUpSearchUser = default!;

		protected async override Task OnParametersSetAsync()
		{
			//IEnumerable<UserConversations> UserConversationsWithUser =
			//	PageMessageConversations.Conversations
			//		.Where(conversation => conversation.Conversation.Participants
			//		.Any(participant => participant.Equals(SendToUser)));

			//if (UserConversationsWithUser.Count() == 0)
			//{
			//	_isloading = false;
			//	StateHasChanged();
			//	return;
			//}

			//await OnCratedConversation.InvokeAsync(
			//	UserConversationsWithUser.First().Conversation
			//);
		}

		public async void OnClickCreateConversion(MouseEventArgs e)
		{
			if(_groupName.IsNullOrEmpty())
			{
				_error = "Name can not be empty";
				await InvokeAsync(StateHasChanged);
				return;
			}

			Conversation? newConversation = await ConversationManager.CreateConversation(
												_groupName,
												AuthenticatedUser.Id,
												true,
												AuthenticatedUser.Id
											);

			if (newConversation == null)
			{
				_error = $"Could'not create group !";
				return;
			}


			SenInvite(_groupName, newConversation);

			await OnCreateConversation.InvokeAsync(newConversation);
		}

		private async void SenInvite(string groupName, Conversation conversation) 
		{
			var invites = await InvitesManager.CreatGroupEnvitesAsync(groupName, AuthenticatedUser.Id, conversation, _usersIds);

			int errorCount = 0;
			foreach(var invite in invites) 
			{
				if (invite == null)
					errorCount++;
			}

			if(errorCount != 0) 
			{
				_invitesError = "Could no send invite to all users!";
				Console.WriteLine($"Failed to send {errorCount} invites.");
				await InvokeAsync(StateHasChanged);
			}
		}

		private void OnClickUser(ApplicationUser user, bool following) 
		{
			_usersToInvite.Add(user, following);
			_popUpSearchUser.OmitResultUser(user);
            _usersIds.Add(user.Id);

            InvokeAsync(StateHasChanged);
		}

		private void OnRemoveUser(ApplicationUser user)
		{
			_usersToInvite.Remove(user);
			_popUpSearchUser.OmitResultUser(user);
            _usersIds.Remove(user.Id);

            InvokeAsync(StateHasChanged);
		}

        private void OnInputChange(ChangeEventArgs e)
        {
			if ( ! e.Value!.ToString().IsNullOrEmpty() ) 
			{
				_error = string.Empty;
				StateHasChanged();
			}
        }
    }
}
