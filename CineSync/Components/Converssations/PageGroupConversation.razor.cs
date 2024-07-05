using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Components.Converssations
{
	public partial class PageGroupConversation : ComponentBase
	{
		[Parameter, EditorRequired]
		public ApplicationUser AuthenticatedUser { get; set; } = default!;

		[Parameter, EditorRequired]
		public PopUpSearchUser.NewMessageFunc OnClickUser { get; set; } = default!;

		[Inject]
		private InvitesManager EnvitesManager { get; set; } = default!;

		[Inject]
		private DbManager<UserConversations> DbUserConversations { get; set; } = default!;

		[Inject]
		private ILookupNormalizer KeyNormalizer { get; set; } = default!;


		private IEnumerable<UserConversations> Conversations = default!;
		private IEnumerable<Invite> InvitesFromMe = default!;
		private IEnumerable<Invite> InvitesToMe = default!;

		private IEnumerable<UserConversations> ConversationsFiltered = default!;
		private IEnumerable<Invite> InvitesFromMeFiltered = default!;
		private IEnumerable<Invite> InvitesToMeFiltered = default!;

		private ApplicationUser _userToSend = default!;
		private bool _Foolowing = false;
		private bool _isLoading = false;
		private bool _initialized = false;
		private string _shearchImput = string.Empty;

		private Task _lastTask = default!;

		private string[] _tabs = { "Groups", "MyInvites", "Invites" };
		private string _activeTab = "Groups";


		private void Initialize()
		{
			if (_initialized) return;

			_isLoading = true;
			UpdateConversations();

			_initialized = true;
		}

		private void OnSearch(string search)
		{


			_shearchImput = search;
			string searchTreated = search.Trim().ToLower();
			string searchNormalize = KeyNormalizer.NormalizeName(search);

			if (_activeTab == _tabs[0]) // Groups
            {
				//Conversations = Conversations.Where(
				//    c => c.Conversation.Participants.Any(u =>
				//        u.User.NormalizedUserName == searchNormalize) ||
				//    c.Conversation.Name.ToLower().Contains(searchTreated)
				//);
			}
			else if (_activeTab == _tabs[1]) // MyInvites
			{
				//InvitesFromMe = MyInvitesGroupMessage.Where(
				//    c => c.Conversation.Name.Contains(search) ||
				//    c.Conversation.Participants.Any(u =>
				//        u.User.NormalizedUserName == searchNormalize)
				//);
			}
			else if (_activeTab == _tabs[2]) // invites
			{
				//InvitesGroupMessageFiltered = InvitesGroupMessage.Where(
				//    c => c.Conversation.Name.Contains(search) ||
				//    c.Conversation.Participants.Any(u =>
				//        u.User.NormalizedUserName == searchNormalize)
				//);
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
								&& !uc.Conversation.IsGroupConversation
								, "Conversation"
								, "Conversation.Participants"
								, "Conversation.Participants.User"
							);

			ConversationsFiltered = Conversations;

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}

		private async void UpdateRequests()
		{
			InvitesToMe = await EnvitesManager.GetByConditionAsync(i =>
							(i.Type == InviteTypes.MESSAGE)
							&& i.Target.Equals(AuthenticatedUser)
							, "Target"
						);

			InvitesToMeFiltered = InvitesToMe;

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}

		private async void UpdateMyRequests()
		{
			InvitesFromMe = await EnvitesManager.GetByConditionAsync(i =>
							i.Type == InviteTypes.MESSAGE 
							&& i.Sender.Equals(AuthenticatedUser)
							, "Sender"
						);

			InvitesFromMeFiltered = InvitesFromMe;

			_isLoading = false;
			await InvokeAsync(StateHasChanged);
		}
	}
}
