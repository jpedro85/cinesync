using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.IdentityModel.Tokens;
using Mono.TextTemplating;
using System.Dynamic;
using System.Formats.Tar;

namespace CineSync.Components.Converssations
{
    public partial class PageMessageConversations : ComponentBase
    {
        [CascadingParameter(Name = "MessageHubConnection")]
        public HubConnection MessageHubConnection { get; set; } = default!;


        [Parameter, EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

        [Parameter, EditorRequired]
        public PopUpSearchUser.NewMessageFunc OnClickUser { get; set; } = default!;

        [Parameter, EditorRequired]
        public EventCallback<Conversation> OnClickConversation { get; set; } = default!;

        [Parameter, EditorRequired]
        public EventCallback<Conversation> OnClickInviteConversation { get; set; } = default!;

        [Inject]
        private InvitesManager EnvitesManager { get; set; } = default!;

        [Inject]
        private DbManager<UserConversations> DbUserConversations { get; set; } = default!;

        [Inject]
        private ILookupNormalizer KeyNormalizer { get; set; } = default!;


        public IEnumerable<UserConversations> Conversations { get; private set; } = new List<UserConversations>();
        public IEnumerable<Invite> InvitesFromMe { get; private set; } = new List<Invite>();
        public IEnumerable<Invite> InvitesToMe { get; private set; } = new List<Invite>();

        private IEnumerable<UserConversations> ConversationsFiltered = new List<UserConversations>();
        private IEnumerable<Invite> InvitesFromMeFiltered = new List<Invite>();
        private IEnumerable<Invite> InvitesToMeFiltered = new List<Invite>();

        //private ApplicationUser _userToSend = default!;
        //private bool _Foolowing = false;
        private bool _isLoading = false;
        private bool _initialized = false;
        private string _shearchImput = string.Empty;

        private Task _lastTask = default!;

        private string[] _tabs = { "Messages", "MyRequests", "Requests" };
        private string _activeTab = "Messages";


        private void Initialize()
        {
            if (_initialized) return;

            _isLoading = true;
            UpdateConversations();
            SubscribeEvents();

            _initialized = true;
        }

        private void OnSearch(string search)
        {

            if( search.IsNullOrEmpty()) 
            {
                ConversationsFiltered = Conversations;
                InvitesFromMeFiltered = InvitesFromMe;
                InvitesToMeFiltered = InvitesToMe;
            }
            else 
            {
                _shearchImput = search;
                string searchNormalize = KeyNormalizer.NormalizeName(search.Trim());

                if (_activeTab == _tabs[0]) // Messages
                {
                    ConversationsFiltered = Conversations.Where( c => 
                            c.Conversation.Participants.Any(u => 
                                !u.User.Equals(AuthenticatedUser) &&
                                u.User.NormalizedUserName!.Contains(searchNormalize)) == true
                    );
                }
                else if (_activeTab == _tabs[1]) // MyRequests
                {
                    InvitesFromMeFiltered = InvitesFromMe.Where( i =>
                            i.Target.NormalizedUserName!.Contains(searchNormalize)
                    ); 
                }
                else if (_activeTab == _tabs[2]) // Requests
                {
                    InvitesToMeFiltered = InvitesToMe.Where(i =>
                            i.Target.NormalizedUserName!.Contains(searchNormalize)
                    );
                }
            }

            StateHasChanged();
        }

        private async void OnTabMessageChange(string actviteTab)
        {
            _activeTab = actviteTab;
            _isLoading = true;
            _shearchImput = string.Empty;

            if (_lastTask != null)
                await _lastTask;

            if (_activeTab == _tabs[0]) // Messages
            {
                _lastTask = Task.Run(UpdateConversations);
            }
            else if (_activeTab == _tabs[1]) // MyRequests
            {
                _lastTask = Task.Run(UpdateMyRequests);
            }
            else if (_activeTab == _tabs[2]) // Requests
            {
                _lastTask = Task.Run(UpdateRequests);
            }

            await InvokeAsync(StateHasChanged);
        }
        private async void UpdateConversations()
        {
            Conversations = await DbUserConversations.GetByConditionAsync(
                                uc => uc.UserId == AuthenticatedUser.Id
                                && !uc.Conversation.IsGroupConversation,
                                "Conversation",
                                "Conversation.Participants",
                                "Conversation.Participants.User",
                                "Conversation.Invites",
                                "Conversation.Invites.Target"
                            );

            ConversationsFiltered = Conversations;

            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async void UpdateRequests()
        {
            InvitesToMe = await EnvitesManager.GetByConditionAsync(i =>
                            (i.Type == InviteTypes.MESSAGE)
                            && i.Target.Equals(AuthenticatedUser),
                            "Target",
                            "Sender"
                        );

            InvitesToMeFiltered = InvitesToMe;

            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        private async void UpdateMyRequests()
        {
            InvitesFromMe = await EnvitesManager.GetByConditionAsync(i =>
                            (i.Type == InviteTypes.MESSAGE)
                            && i.Sender.Equals(AuthenticatedUser)
                            , "Sender"
                        );

            InvitesFromMeFiltered = InvitesFromMe;

            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }

        public void UpdateMyRequests(Invite invite)
        {
            InvitesFromMe = InvitesFromMe.Append(invite);
            InvitesFromMeFiltered = InvitesFromMeFiltered.Append(invite);

            InvokeAsync(StateHasChanged);
        }

        public void UpdateConversations(UserConversations userConversation)
        {
            Conversations = Conversations.Append(userConversation);
            ConversationsFiltered = ConversationsFiltered.Append(userConversation);

            InvokeAsync(StateHasChanged);
        }

        public void UpdateRemoveConversations(Conversation conversation)
        {
            Conversations = Conversations.Where(uc => uc.ConversationId != conversation.Id);
            ConversationsFiltered = ConversationsFiltered.Where(uc => uc.ConversationId != conversation.Id);

            InvokeAsync(StateHasChanged);
        }

        public void RemoveMyRequest(uint inviteId)
        {
            InvitesFromMe = InvitesFromMe.Where(i => i.Id != inviteId);
            InvitesFromMeFiltered = InvitesFromMeFiltered.Where(i => i.Id != inviteId);
        }

        public void RemoveRequest(uint inviteId)
        {
            InvitesFromMe = InvitesFromMe.Where(i => i.Id != inviteId);
            InvitesFromMeFiltered = InvitesFromMeFiltered.Where(i => i.Id != inviteId);
        }

        private void SubscribeEvents()
        {
            MessageHubConnection.On<Invite>("UpdateMyRequestState", UpdateMyRequestState);
            MessageHubConnection.On<Invite>("UpdateYourRequestState", UpdateRequestState);
        }

        private void UpdateMyRequestState( Invite invite ) 
        {
            if (!invite.Sender.Equals(AuthenticatedUser))
                return;

            Invite? localInvite = InvitesFromMe.Where(i => i.Equals(invite)).FirstOrDefault();

            if (localInvite == null) return;

            localInvite.State = invite.State;

            localInvite = InvitesFromMeFiltered.Where(i => i.Equals(invite)).First();
            localInvite.State = invite.State;

            if (_activeTab == _tabs[1]) // my Requests
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private void UpdateRequestState( Invite invite )
        {
            if (!invite.Target.Equals(AuthenticatedUser))
                return;

            Invite localInvite = InvitesToMe.Where(i => i.Equals(invite)).First();
            localInvite.State = invite.State;

            localInvite = InvitesToMeFiltered.Where(i => i.Equals(invite)).First();
            localInvite.State = invite.State;

            if (_activeTab == _tabs[2]) // Requests
            {
                InvokeAsync(StateHasChanged);
            }
        }

        private async void OnUpdateMyRequestState(Invite invite)
        {
            await MessageHubConnection.InvokeAsync("UpdateMyRequestState", invite);
        }

        private async void OnUpdateRequestState(Invite invite)
        {
            await MessageHubConnection.InvokeAsync("UpdateYourRequestState", invite);
        }
    }
}
