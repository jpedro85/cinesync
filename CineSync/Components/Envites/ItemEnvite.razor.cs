using Microsoft.AspNetCore.Components;
using CineSync.Data.Models;
using CineSync.Data;
using Microsoft.AspNetCore.Components.Web;
using CineSync.DbManagers;

namespace CineSync.Components.Envites
{
	public partial class ItemEnvite : ComponentBase
	{
		[Parameter,EditorRequired]
		public Invite Invite { get; set; } = default!;

		[Parameter,EditorRequired]
		public ApplicationUser Authenticateduser { get; set; } = default!;

		[Parameter]
		public string ItemUserStyle { get; set; } = string.Empty;

		[Parameter]
		public EventCallback<uint> OnRemove { get; set; }

        [Parameter]
        public EventCallback<Invite> OnAccept { get; set; }

        [Parameter]
        public EventCallback<Invite> OnDecline { get; set; }

        [Parameter]
		public bool ShowUser { get; set; } = true;

		[Parameter]
		public bool AllowCancel { get; set; } = true;


		[Inject]
		public InvitesManager EnvitesManager { get; set; } = default!;

		[Inject]
		public ConversationManager ConverssationManager { get; set; } = default!;

		private bool _isAutor = false;

		protected override void OnParametersSet()
		{
			_isAutor = Invite.Sender.Equals(Authenticateduser);
			StateHasChanged();
		}

		public async void OnclickHide(MouseEventArgs e) 
		{
			uint id = Invite.Id;
			if (await EnvitesManager.HideAsync(Authenticateduser.Id, Invite.Id) && OnRemove.HasDelegate) 
			{
				await OnRemove.InvokeAsync(id);

                Invite.HideBySender = Authenticateduser.Id == (Invite.Sender?.Id ?? "-1");
                Invite.HideByTarget = Authenticateduser.Id == (Invite.Target?.Id ?? "-1");
                StateHasChanged();
			}
		}

		public async void OnclickAccept(MouseEventArgs e)
		{
			if (Invite.State == InviteStates.ACDEPTED)
				return;

			if ( await EnvitesManager.AcceptAsync(Authenticateduser.Id,Invite.Id) ) 
			{
				if (await ConverssationManager.AddParticipants(Invite.ConversationId, Invite.Target.Id)) 
				{
					Invite.State = InviteStates.ACDEPTED;

					if(OnAccept.HasDelegate)
						await OnAccept.InvokeAsync(Invite);

                    StateHasChanged();
				}
				else
				{
					await EnvitesManager.MakeDefaultAsync(Authenticateduser.Id, Invite.Id);
				}
			}
		}

		public async void OnclickDecline(MouseEventArgs e)
		{
			if (Invite.State == InviteStates.DECLINED)
				return;

			if (await EnvitesManager.DeclineAsync(Authenticateduser.Id, Invite.Id))
			{
				Invite.State = InviteStates.DECLINED;

                if (OnAccept.HasDelegate)
                    await OnDecline.InvokeAsync(Invite);

                StateHasChanged();
			}
            else
            {
                await EnvitesManager.MakeDefaultAsync(Authenticateduser.Id, Invite.Id);
            }
        }
	}
}