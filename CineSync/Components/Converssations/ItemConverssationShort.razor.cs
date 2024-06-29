using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Data;

namespace CineSync.Components.Converssations
{
    public partial class ItemConverssationShort
    {
        [Parameter, EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

        [Parameter,EditorRequired]
        public Conversation Conversation { get; set; } = default!;

		[Parameter]
		public EventCallback OnClick { get; set; } = default!;

		[Parameter]
		public int MaxGroupParticipantsToShow { get; set; } = 5;


		[Inject]
        public ConverssationManager ConverssationManager { get; set; } = default!;

		private bool _hasNewMessage = true;

		//TODO: UNCOMMENT
		//protected override async Task OnInitializedAsync()
		//{
		//    Conversation.Participants = await ConverssationManager.GetParticipants(Conversation);
		//}

		public void UpdateNotificationState( bool newState) 
		{
			_hasNewMessage = newState;
			InvokeAsync(StateHasChanged);
		}
	}
}
