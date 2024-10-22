﻿using CineSync.Components.DMS;
using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace CineSync.Components.Converssations
{
    public partial class ConverssationUser
    {
		[CascadingParameter(Name = "MessageHubConnection")]
		public HubConnection MessageHubConnection { get; set; } = default!;

        [Parameter,EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

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

        [Inject]
        public MessageManager MessageManager { get; set; } = default!;

		[Inject]
		public UserManager UserManager { get; set; } = default!;

		[Inject]
		public IJSRuntime JS { get; set; } = default!;


		private string _messageHubGroupName = string.Empty;
        private bool _isloading = true;
        private ApplicationUser UserToSend = default!;
        private string _error = string.Empty;
		private DmInput _dminpput = default!;
		private List<ItemMessage> itemMessages = [];

		public ItemMessage ImteMessageRef { get { return null!; } private set { itemMessages.Add(value); } }


		protected override void OnInitialized()
        {
			_isloading = true;
			SubscribeEvents();
        }

        protected override async Task OnParametersSetAsync()
		{
			_isloading = true;
			itemMessages.Clear();
			itemMessages = new List<ItemMessage>();
			GetUserToSend();
			StateHasChanged();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await JS.InvokeVoidAsync("scrollToBottom", "scrollable_messages");
		}

		private void GetUserToSend() 
        {
			if (Conversation.Participants.Count > 1)
			{
				UserToSend = Conversation.Participants
								.Where(u => !u.User.Equals(AuthenticatedUser))
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

		private async Task GetMessages() 
		{
			Conversation.Messages = await ConversationManager.GetMessages(Conversation);
			_isloading = false;
			StateHasChanged();
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
			_messageHubGroupName = ConversationManager.GetGroupName(Conversation);
            MessageHubConnection.InvokeAsync("JoinRoom", _messageHubGroupName );
            MessageHubConnection.On<Invite>("UpdateMyRequestState", UpdateInviteState);
            MessageHubConnection.On<string, uint>("UpdateMessages", OnUpdateMessage);
			MessageHubConnection.On<string, uint, string>("UpdateReaction", OnUpdateReaction);
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

		private async void OnNewMessage( Message message ) 
		{
			Message? _messagedb = await MessageManager.CreateMessage(message, Conversation.Id, AuthenticatedUser.Id);

			if (message != null)
			{
				MessageHubConnection.InvokeAsync("NotifyGroupNewMessage", _messageHubGroupName, _messagedb.Id);
				InvokeAsync(StateHasChanged);
			}
			else
				Console.WriteLine("Merda");

		}

        private async void OnUpdateMessage(string groupName,uint messageid)
        {
			if(groupName == _messageHubGroupName) 
			{
				Message message = (await MessageManager.GetFirstByConditionAsync(m => m.Id == messageid))!;
				await InvokeAsync(StateHasChanged);
			}
        }

		private async void OnUpdateReaction(string groupName, uint messageid,string reaction)
		{
			if (groupName != _messageHubGroupName)
				return;

			ItemMessage? itemMessage = itemMessages.Where(iM => iM.Message.Id == messageid).FirstOrDefault();
			itemMessage?.AddReaction(reaction);
		}

		private void OnMessageReply( Message message ) 
		{
			_dminpput.AddReply( message );
			UnHighlightOthers(message);
        }

		private void UnHighlightOthers( Message message) 
		{
			foreach (var itemMessage in itemMessages)
			{
				if (itemMessage.Message.Id == message.Id)
					continue;

				itemMessage.Highlight(false);
			}
		}
		private void OnRemoveReply( Message message ) 
		{
			ItemMessage? itemMessage = itemMessages.Where( iM => iM.Message.Id == message.Id ).FirstOrDefault();
			itemMessage?.Highlight(false);
        }

		private void OnOpenImojiPiker(Message message) 
		{
			foreach (var itemMessage in itemMessages)
			{
				if (itemMessage.Message.Id == message.Id)
					continue;

				itemMessage.CloseImojiPiker();
			}
		}

		public Task SetLoading() 
		{
			_isloading = true;
			return InvokeAsync(StateHasChanged);
		}
	}
}
