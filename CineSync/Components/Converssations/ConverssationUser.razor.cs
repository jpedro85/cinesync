using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace CineSync.Components.Converssations
{
    public partial class ConverssationUser
    {
		[CascadingParameter(Name = "MessageHubConnection")]
		public HubConnection MessageHubConnection { get; set; } = default!;

        [Parameter,EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public PageMessageConversations PageMessageConversations { get; set; } = default!;

        [Parameter, EditorRequired]
        public Conversation Conversation { get; set; } = default!;

		[Parameter, EditorRequired]
		public EventCallback<Conversation> OnRemoveMessage { get; set; } = default!;

		[Parameter]
        public Invite? Invite { get; set; } = default;


        [Inject]
        public InvitesManager InvitesManager { get; set; } = default!;

		[Inject]
		public ConversationManager ConversationManager { get; set; } = default!;


		private bool _isloading = true;
        private ApplicationUser UserToSend = default!;
		private string _error = string.Empty;


        protected override void OnInitialized()
        {
			SubscribeEvents();
        }

        protected override async Task OnParametersSetAsync()
		{
			GetUserToSend();
			GetMessages();
			_isloading = false;
			StateHasChanged();
		}

		//protected override void OnAfterRender(bool firstRender)
		//{
		//	//if (firstRender)
		//	//{
		//	//	GetUserToSend();
		//	//	GetMessages();
		//	//	_isloading = false;
		//	//	StateHasChanged();
		//	//}
		//}
	
        private void GetUserToSend() 
        {
			if (Conversation.Participants.Count > 1)
			{
				UserToSend = Conversation.Participants
								.Where(u => !u.Equals(AuthenticatedUser))
								.First()
								.User;
			}
			else
			{
				UserToSend = Conversation.Invites
								.Where(u => !u.Equals(AuthenticatedUser))
								.First()
								.Target;
			}
		}

		private async void GetMessages() 
		{
			Conversation.Messages = await ConversationManager.GetMessages(Conversation);
		}

		private async void ResendInvite( Invite invite ) 
		{
			if (!await InvitesManager.ResendInviteAsync(AuthenticatedUser.Id, invite))
			{
				_error = "Could not resend invite.";
			}
			await InvokeAsync(StateHasChanged);
		}

		private async void RemoveMessage() 
		{
			await ConversationManager.RemoveAsync(Conversation);
			await OnRemoveMessage.InvokeAsync(Conversation);
		}

        private void SubscribeEvents()
        {
            MessageHubConnection.On<Invite>("UpdateMyRequestState", UpdateInviteState);
        }

        public async void UpdateInviteState( Invite invite ) 
		{
            if ( Conversation != null && Conversation.Id == invite.ConversationId)
            {
                Conversation.Invites.Where(i => i.Equals(invite))
                    .First().State = invite.State;

				Conversation.Participants = await ConversationManager.GetParticipants(Conversation);

                await InvokeAsync(StateHasChanged);
            }
        }
	}
}
