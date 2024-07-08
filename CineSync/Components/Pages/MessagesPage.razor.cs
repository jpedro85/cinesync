using CineSync.Data.Models;
using CineSync.Components.Layout;
using Microsoft.AspNetCore.Components;
using CineSync.Data;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.SignalR;
using CineSync.Components.Converssations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.SignalR.Protocol;
using CineSync.Hubs;
using System.Data;
using Microsoft.AspNetCore.SignalR.Client;

namespace CineSync.Components.Pages
{
    public partial class MessagesPage
    {
        [Parameter]
        public uint ConversationId { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        private ApplicationUser AuthenticatedUser { get; set; } = default!;

        private PageLayout _pageLayout = default!;
        private HubConnection MessageHubConnection { get; set; } = default!;

        private PopUpSearchUser _popupSearchUser = default!;
        private PageMessageConversations _pageMessageConversations = default!;
        private PageGroupConversation _pageGroupConversations = default!;

        private ApplicationUser _userToSend = default!;
        private Conversation _conversation = default!;
        private ConverssationUser? _conversationUser = null;

        private bool _initialized = false;
        private bool _Foolowing = false;
        private bool _isLoading = false;

        private State? _actualState = null;
        private enum State
        {
            NEW_MESSAGE,
            NEW_GROUP,
            GROUP,
            MESSAGE
        }

        private async void Initialize() 
        {
            if(_initialized) return;

            AuthenticatedUser = _pageLayout.AuthenticatedUser!;

            await ConnectToMessageHub();

			_initialized = true;
        }

      
		private async Task ConnectToMessageHub() 
        {
			MessageHubConnection = new HubConnectionBuilder()
			  .WithUrl(NavigationManager.ToAbsoluteUri("/MessageHub"))
			  .Build();

            await MessageHubConnection.StartAsync();

            StateHasChanged();
        }


		private void GetPagelayout(PageLayout instance) 
        {
            if(_pageLayout == null)
                _pageLayout = instance;
        }

        private void OnClickUser(ApplicationUser user, bool foolowing) 
        {
            _userToSend = user;
            _Foolowing = foolowing;
            _actualState = State.NEW_MESSAGE;
            StateHasChanged();
		}

        private async void OnClickConversation( Conversation conversation ) 
        {
            _actualState = State.MESSAGE;
            _conversation = conversation;

            if (_conversationUser != null)
                await _conversationUser.SetLoading();
            
            await InvokeAsync(StateHasChanged);
        }

        private void OnNewMessage( Conversation conversation ) 
        {
            _actualState = State.MESSAGE;
            _conversation = conversation;
            UserConversations userConversations = new UserConversations()
            {
                Conversation = conversation,
                ConversationId = _conversation.Id,
                User = AuthenticatedUser,
                UserId = AuthenticatedUser.Id
            };

            _pageMessageConversations.UpdateConversations( userConversations );

			StateHasChanged();
        }

        private void OnRemoveMessage(Conversation conversation) 
        {
            _pageMessageConversations.UpdateRemoveConversations(conversation);
        }

    }
}
