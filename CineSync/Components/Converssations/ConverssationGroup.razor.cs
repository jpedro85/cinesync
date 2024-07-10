using CineSync.Components.DMS;
using CineSync.Data.Models;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace CineSync.Components.Converssations
{
    public partial class ConverssationGroup
    {
		[CascadingParameter(Name = "MessageHubConnection")]
		public HubConnection MessageHubConnection { get; set; } = default!;

		[Parameter, EditorRequired]
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
		private string _error = string.Empty;
		private DmInput _dminpput = default!;
		private List<ItemMessage> itemMessages = [];
		private PopUpParticipants _popUpParticipants = default!;
		private PopUpSearchUser _popupSearchUser = default!;

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

			StateHasChanged();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			OmitResultsInSearchUser();
			await JS.InvokeVoidAsync("scrollToBottom", "scrollable_messages");
		}

		private void OmitResultsInSearchUser() 
		{
			foreach (var uc in Conversation.Participants)
			{
				_popupSearchUser.OmitResultUser(uc.User);
			}
			foreach (var uc in Conversation.Invites)
			{
				_popupSearchUser.OmitResultUser(uc.Target);
			}
		}

		private async Task GetMessages()
		{
			Conversation.Messages = await ConversationManager.GetMessages(Conversation);
			_isloading = false;
			StateHasChanged();
		}

		private async void ResendInvite(Invite invite)
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
			MessageHubConnection.InvokeAsync("JoinRoom", _messageHubGroupName);
			MessageHubConnection.On<Invite>("UpdateMyRequestState", UpdateInviteState);
			MessageHubConnection.On<string,uint>("UpdateMessages", OnUpdateMessage);
			MessageHubConnection.On<string, uint, string>("UpdateReaction", OnUpdateReaction);
		}

		public async void UpdateInviteState(Invite invite)
		{
			if (Conversation != null && Conversation.Id == invite.ConversationId)
			{
				Conversation.Invites.Where(i => i.Equals(invite))
					.First().State = invite.State;

				Conversation.Participants = await ConversationManager.GetParticipants(Conversation);

				await InvokeAsync(StateHasChanged);
			}
		}

		private async void OnNewMessage(Message message)
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

		private async void OnUpdateMessage(string groupName, uint messageid)
		{
			if(groupName == _messageHubGroupName) 
			{
				Message message = (await MessageManager.GetFirstByConditionAsync(m => m.Id == messageid))!;
				await InvokeAsync(StateHasChanged);
			}
		}

		private async void OnUpdateReaction( string groupname, uint messageid, string reaction)
		{
			if (groupname != _messageHubGroupName)
				return;

			ItemMessage? itemMessage = itemMessages.Where(iM => iM.Message.Id == messageid).FirstOrDefault();
			itemMessage?.AddReaction(reaction);
		}

		private void OnMessageReply(Message message)
		{
			_dminpput.AddReply(message);
			UnHighlightOthers(message);
		}

		private void UnHighlightOthers(Message message)
		{
			foreach (var itemMessage in itemMessages)
			{
				if (itemMessage.Message.Id == message.Id)
					continue;

				itemMessage.Highlight(false);
			}
		}
		private void OnRemoveReply(Message message)
		{
			ItemMessage? itemMessage = itemMessages.Where(iM => iM.Message.Id == message.Id).FirstOrDefault();
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

		public void OnOpenParticipants() 
		{
			_popUpParticipants.Open();
		}

		public async void CreateInvite(ApplicationUser user) 
		{
			if( !Conversation.Invites.Any(i=>i.Target.Equals(user)) && 
				!Conversation.Participants.Any(uc => uc.User.Equals(user))
			) 
			{
				await InvitesManager.CreatGroupEnvitesAsync(Conversation.Name, AuthenticatedUser.Id, Conversation, user.Id);
				_popupSearchUser.OmitResultUser(user);
			}
		}
	}


}
