using CineSync.Components.PopUps;
using CineSync.Data;
using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;
using CineSync.DbManagers;
using CineSync.Hubs;
using Microsoft.AspNetCore.SignalR.Client;

namespace CineSync.Components.Converssations
{
    public partial class ItemMessage : ComponentBase
    {
		[CascadingParameter(Name = "MessageHubConnection")]
		public HubConnection MessageHubConnection { get; set; } = default!;

        [Parameter, EditorRequired]
        public string Group { get; set; } = default!;

		[Parameter, EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

        [Parameter, EditorRequired]
        public Message Message { get; set; } = default!;

        [Parameter, EditorRequired]
        public EventCallback<Message> OnReply { get; set; }

		[Parameter, EditorRequired]
		public EventCallback<Message> OnOpenImojiPiker { get; set; }

		[Parameter, EditorRequired]
        public EventCallback<Message> OnRemove {  get; set; }

        [Inject]
        private MessageManager MessageManager { get; set; } = default!;


		private PopUpAttachementView _attachementView = default!;
        private bool _showImojiPicker = false;
        private bool _highlight = false;
        private bool _loading = true;

		protected override void OnInitialized()
		{
            _loading = true;
		}

        private async void GetInfo() 
        {
			Message.Attachements = await MessageManager.GetAttachments(Message);
            Message.Reactions = await MessageManager.GetReactions(Message);
			Message.ReplayMessage = await MessageManager.GetReplyMessage(Message);
			Message.SeenByUsers = await MessageManager.GetSeenByUsers(Message);

            _loading = false;
            await InvokeAsync(StateHasChanged);
		}

		private void OpenAttachment(byte[] attachment)
		{
			_attachementView.Attachment = attachment;
			_attachementView.Name = "View Attachement";
			_attachementView.TrigerStatehasChanged();
			_attachementView.Open();
		}

        private async void AddEmoji(string emoji)
        {

            if(Message.Reactions == null)
				Message.Reactions = new List<Reaction>();

            Message.Reactions.Add( new Reaction() 
                {
                    Autor = AuthenticatedUser,
                    ReactionContent = emoji,
			    }
            );;

            await MessageManager.UpdateEntity(Message, "Reactions");
            await MessageHubConnection.InvokeAsync("NotifyGroupNewReaction", Group ,Message.Id, emoji);

            _showImojiPicker = false;
			await InvokeAsync(StateHasChanged);
		}

        public void Highlight( bool state ) 
        {
            if (state == _highlight) return;

            _highlight = state;
            InvokeAsync(StateHasChanged);
		}

        public void CloseImojiPiker() 
        {
            _showImojiPicker = false;
            InvokeAsync(StateHasChanged);
        }

        public void AddReaction(string reaction) 
        {
            if (Message.Reactions == null)
				Message.Reactions = new List<Reaction>();

			Message.Reactions?.Add( new Reaction { ReactionContent = reaction });

            InvokeAsync(StateHasChanged);
        }
	}
}
