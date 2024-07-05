using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;
using Microsoft.VisualStudio.TextTemplating;

namespace CineSync.DbManagers
{
	public class InvitesManager : DbManager<Invite>
	{
		private IRepositoryAsync<ApplicationUser> _userRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="InvitesManager"/> class.
		/// </summary>
		/// <param name="unitOfWork">The unit of work to manage database transactions.</param>
		/// <param name="logger">The logger strategy for logging operations.</param>
		public InvitesManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
		{
			this._userRepository = unitOfWork.GetRepositoryAsync<ApplicationUser>();
		}

		/// <summary>
		/// Creates a new message invite.
		/// </summary>
		/// <param name="senderId">The ID of the user sending the invite.</param>
		/// <param name="targetId">The ID of the user receiving the invite.</param>
		/// <param name="conversation">The user converssation.</param>
		/// <returns>The created <see cref="Invite"/> if successful, otherwise null.</returns>
		public async Task<Invite?> CreatMessgeEnviteAsync(string senderId, string targetId, Conversation conversation)
		{
			ApplicationUser? sender = await _userRepository.GetFirstByConditionAsync(u => u.Id == senderId);
			if (sender == null) return null;

			ApplicationUser? target = await _userRepository.GetFirstByConditionAsync(u => u.Id == targetId);
			if (target == null) return null;

			Invite newInvite = new Invite();
			newInvite.Type = InviteTypes.MESSAGE;
			newInvite.State = InviteStates.DEFAULT;
			newInvite.CreatedTimestanp = DateTime.Now;
			newInvite.Conversation = conversation;
			newInvite.ConversationId = conversation.Id;
			newInvite.Sender = sender;
			newInvite.Target = target;
			newInvite.Name = "MessageTo_" + target.Id;
			await _repository.InsertAsync(newInvite);

			return await _unitOfWork.SaveChangesAsync() ? newInvite : null;
		}

		public async Task<bool> ResendInviteAsync(string autorId,Invite invite) 
		{
			Invite? _invite = await _repository.GetFirstByConditionAsync(i =>
								i.Id == invite.Id &&
								i.Sender.Id == autorId,
								"Sender"
							);

			if (_invite == null)
				return false;

			_invite.State = InviteStates.DEFAULT;

			return await _unitOfWork.SaveChangesAsync();

		}

		/// <summary>
		/// Creates a group invite.
		/// </summary>
		/// <param name="groupName">The name of the group for the invites.</param>
		/// <param name="senderId">The ID of the user sending the invites.</param>
		/// <param name="conversation">The user converssation.</param>
		/// <param name="tragetsId">An array of IDs of the users receiving the invites.</param>
		/// <returns>A collection of created <see cref="Invite"/> instances if successful, otherwise null.</returns>
		public async Task<ICollection<Invite?>> CreatGroupEnvitesAsync(string groupName, string senderId, Conversation conversation, params string[] tragetsId)
		{
			ApplicationUser? sender = await _userRepository.GetFirstByConditionAsync(u => u.Id == senderId);
			if (sender == null) return [];

			ApplicationUser?[] targets = new ApplicationUser[tragetsId.Length];
			for (int i = 0; i < tragetsId.Length; i++)
			{
				targets[i] = await _userRepository.GetFirstByConditionAsync(u => u.Id == tragetsId[i]);
				if (targets[i] == null) return [];
			}

			Invite?[] envites = new Invite?[targets.Length];
			Task[] envitesTasks = new Task[targets.Length];

			for (int i = 0; i < tragetsId.Length; i++)
			{
				envitesTasks[i] = Task.Run(async () =>
				{
					envites[i] = await CreatGroupEnviteAsync(
											sender,
											targets[i]!,
											conversation,
											groupName
										);

				});

			}

			Task.WaitAll(envitesTasks);
			return envites;
		}

		/// <summary>
		/// Creates a single group invite.
		/// </summary>
		/// <param name="sender">The user sending the invite.</param>
		/// <param name="target">The user receiving the invite.</param>
		/// <param name="conversation">The user converssation.</param>
		/// <param name="name">The name of the group.</param>
		/// <returns>The created <see cref="Invite"/> if successful, otherwise null.</returns>
		private async Task<Invite?> CreatGroupEnviteAsync(ApplicationUser sender, ApplicationUser target, Conversation conversation, string name)
		{
			Invite newInvite = new Invite();
			newInvite.Type = InviteTypes.GROUP;
			newInvite.State = InviteStates.DEFAULT;
			newInvite.CreatedTimestanp = DateTime.Now;
			newInvite.Conversation = conversation;
			newInvite.ConversationId = conversation.Id;
			newInvite.Sender = sender;
			newInvite.Target = target;
			newInvite.Name = name;
			await _repository.InsertAsync(newInvite);

			return await _unitOfWork.SaveChangesAsync() ? newInvite : null;
		}

		/// <summary>
		/// Sets ist status to default.
		/// </summary>
		/// <param name="userId">The ID of the user accepting the invite.</param>
		/// <param name="enviteId">The ID of the invite to be accepted.</param>
		/// <returns>True if the invite was successfully accepted, otherwise false.</returns>
		public async Task<bool> MakeDefaultAsync(string userId, uint enviteId)
		{
			Invite? envite = await _repository.GetFirstByConditionAsync(e => e.Id == enviteId);
			if (envite == null)
				return false;
			else if (userId != envite.Target.Id)
				return false;

			envite.State = InviteStates.DEFAULT;

			return await _unitOfWork.SaveChangesAsync();
		}

		/// <summary>
		/// Accepts an invite by setting its status to accepted and associating it with a conversation.
		/// </summary>
		/// <param name="userId">The ID of the user accepting the invite.</param>
		/// <param name="enviteId">The ID of the invite to be accepted.</param>
		/// <returns>True if the invite was successfully accepted, otherwise false.</returns>
		public async Task<bool> AcceptAsync(string userId, uint enviteId)
		{
			Invite? envite = await _repository.GetFirstByConditionAsync(e => e.Id == enviteId);
			if (envite == null)
				return false;
			else if (userId != envite.Target.Id)
				return false;

			envite.State = InviteStates.ACDEPTED;

			return await _unitOfWork.SaveChangesAsync();
		}

		/// <summary>
		/// Decline an invite by setting its status to decline and associating it with a conversation.
		/// </summary>
		/// <param name="userId">The ID of the user accepting the invite.</param>
		/// <param name="enviteId">The ID of the invite to be accepted.</param>
		/// <returns>True if the invite was successfully accepted, otherwise false.</returns>
		public async Task<bool> DeclineAsync(string userId, uint enviteId)
		{
			Invite? envite = await _repository.GetFirstByConditionAsync(e => e.Id == enviteId);
			if (envite == null)
				return false;
			else if (userId != envite.Target.Id)
				return false;

			envite.State = InviteStates.DECLINED;

			return await _unitOfWork.SaveChangesAsync();
		}

        /// <summary>
        /// hide an invite by setting hide true.
        /// </summary>
        /// <param name="userId">The ID of the user accepting the invite.</param>
        /// <param name="enviteId">The ID of the invite to be accepted.</param>
        /// <returns>True if the invite was successfully accepted, otherwise false.</returns>
        public async Task<bool> HideAsync(string userId, uint enviteId)
        {
            Invite? envite = await _repository.GetFirstByConditionAsync(e => e.Id == enviteId, "Sender" , "Target");
			if (envite == null)
				return false;
			else if (userId == envite.Target.Id)
			{
				envite.HideByTarget = true;
			}
			else if (userId == envite.Sender.Id)
			{
				envite.HideBySender = true;
			}
			else
				return false;

            return await _unitOfWork.SaveChangesAsync();
        }

    }
}