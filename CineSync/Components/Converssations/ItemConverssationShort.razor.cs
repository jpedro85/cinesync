using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Data;
using Microsoft.AspNetCore.SignalR.Client;

namespace CineSync.Components.Converssations
{
    public partial class ItemConverssationShort
    {
        [CascadingParameter(Name = "MessageHubConnection")]
        public HubConnection? MessageHubConnection { get; set; } = default;


        [Parameter, EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

        [Parameter,EditorRequired]
        public Conversation Conversation { get; set; } = default!;

		[Parameter]
		public EventCallback<Conversation> OnClick { get; set; } = default!;

		[Parameter]
		public int MaxGroupParticipantsToShow { get; set; } = 5;


		[Inject]
        public ConversationManager ConversationManager { get; set; } = default!;

		private bool _hasNewMessage = true;

        protected override void OnInitialized()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            MessageHubConnection?.On<Invite>("UpdateMyRequestState", UpdateInviteState);
        }

        public async void UpdateInviteState(Invite invite)
        {
            if (Conversation != null && Conversation.Id == invite.ConversationId)
            {
                Conversation.Invites.Where(i => i.Equals(invite))
                    .First().State = invite.State;

                Conversation.Participants = await ConversationManager.GetParticipants(Conversation);

                InvokeAsync(StateHasChanged);
            }
        }
    }
}
