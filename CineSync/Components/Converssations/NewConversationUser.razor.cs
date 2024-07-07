using CineSync.Data.Models;
using CineSync.Data;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Converssations
{
    public partial class NewConversationUser
    {
		[Parameter, EditorRequired]
		public ApplicationUser AuthenticatedUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public ApplicationUser SendToUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public PageMessageConversations PageMessageConversations { get; set; } = default!;

		[Parameter, EditorRequired]
		public EventCallback<Conversation> OnCratedConversation { get; set; } = default!;


		[Inject]
		public InvitesManager EnvitesManager { get; set; } = default!;

		[Inject]
		public ConversationManager ConversationManager { get; set; } = default!;


		private bool _isloading = true;
		private string _error = string.Empty;

		protected async override Task OnParametersSetAsync()
		{
			IEnumerable<UserConversations> UserConversationsWithUser = 
				PageMessageConversations.Conversations
					.Where(conversation => conversation.Conversation.Participants
					.Any(participant => participant.Equals(SendToUser)));

			if ( UserConversationsWithUser.Count() == 0 ) 
			{
				_isloading = false;
				StateHasChanged();
				return;
			}

			await OnCratedConversation.InvokeAsync(
				UserConversationsWithUser.First().Conversation
			);
		}

		public async void OnClickSendRequest(MouseEventArgs e)
		{
			Conversation? newConversation = await ConversationManager.CreateConversation(
												$"{AuthenticatedUser.Id}_{SendToUser.Id}",
												false,
												AuthenticatedUser.Id
											);
			if (newConversation == null)
			{
				_error = $"Could'not send invite to {SendToUser.UserName} !";
				await InvokeAsync(StateHasChanged);
				return;
			}

			Invite? newInvite = await EnvitesManager.CreatMessgeEnviteAsync(
						AuthenticatedUser.Id,
						SendToUser.Id,
						newConversation
					);

			if (newInvite == null)
			{
				_error = $"Could'not send invite to {SendToUser.UserName} !";
				await InvokeAsync(StateHasChanged);
			}
            else
            {
				PageMessageConversations.UpdateMyRequests(newInvite);
				await OnCratedConversation.InvokeAsync(newConversation);
            }

        }
	}
}
