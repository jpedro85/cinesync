using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class ConversationManager : DbManager<Conversation>
	{
		private IRepositoryAsync<ApplicationUser> _userRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConversationManager"/> class, which manages the movie-related database operations.
		/// </summary>
		/// <param name="unitOfWork">The unit of work handling database transactions.</param>
		/// <param name="logger">The logger for recording operations and exceptions.</param>
		public ConversationManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
		{
			_userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
		}

		/// <summary>
		/// Creates a conversation.
		/// </summary>
		/// <param name="name">The name of the group for the invites.</param>
		/// <param name="isGroup">If the conversation isGroup </param>
		/// <param name="userIds">Participipants ids</param>
		/// <returns>A collection of created <see cref="Invite"/> instances if successful, otherwise null.</returns>
		public async Task<Conversation?> CreateConversation(string name, bool isGroup, params string[] userIds)
		{
			if (userIds.Length < 1)
				return null;

			ApplicationUser?[] participants = await FetchParticipants(userIds);

			if (participants.Any(p => p == null))
				return null;

			Conversation conversation = new Conversation();
			conversation.Name = name;
			conversation.IsGroupConversation = isGroup;
			conversation.Participants = new List<UserConversations>(participants.Length);

			foreach (ApplicationUser? user in participants)
			{
				conversation.Participants.Add(
					new UserConversations()
					{
						Conversation = conversation,
						ConversationId = conversation.Id,
						User = user!,
						UserId = user!.Id
					});
			}

			await _repository.InsertAsync(conversation);

			if (await _unitOfWork.SaveChangesAsync())
				return conversation;
			else
				return null;
		}

		/// <summary>
		/// Fetch participants
		/// </summary>
		/// <param name="userIds">Participipants ids</param>
		/// <returns>A collection of created <see cref="Invite"/> instances if successful, otherwise null.</returns>
		private async Task<ApplicationUser?[]> FetchParticipants(string[] userIds)
		{
			ApplicationUser?[] participants = new ApplicationUser?[userIds.Length];

			for (int i = 0; i < participants.Length; i++)
			{
				participants[i] = await _userRepository.GetFirstByConditionAsync(u => userIds.Contains(u.Id));
			}

			return participants;
		}

		/// <summary>
		/// Add participants
		/// </summary>
		/// <param name="userIds">Participipants ids</param>
		/// <returns>A collection of created <see cref="Invite"/> instances if successful, otherwise null.</returns>
		public async Task<bool> AddParticipants(uint conversationId, params string[] userIds)
		{
			ApplicationUser?[] participants = await FetchParticipants(userIds);

			if (participants.Any(p => p == null))
				return false;

			Conversation? conversation = await _repository.GetFirstByConditionAsync(c => c.Id == conversationId);

			if (conversation == null)
				return false;
			else if (conversation.Participants == null)
				conversation.Participants = new List<UserConversations>();

			foreach(var participant in participants) 
			{
				UserConversations userConversations = new UserConversations()
				{
					Conversation = conversation,
					User = participant!,
					UserId = participant!.Id,
					ConversationId = conversation.Id,
				};
				conversation.Participants.Add(userConversations);
			}

			return await _unitOfWork.SaveChangesAsync();
		}

		/// <summary>
		/// Retrives all participants.
		/// </summary>
		/// <param name="conversation">The conversarion to get particioants.</param>
		/// <returns>A collection of users <see cref="UserConversations"/>.</returns>
		public async Task<ICollection<UserConversations>> GetParticipants(Conversation conversation)
		{
			Conversation? fetchedConversation = await GetFirstByConditionAsync(c => c.Id == conversation.Id, "Participants", "Participants.User");

			if (fetchedConversation == null)
				return [];

			return fetchedConversation.Participants ?? new List<UserConversations>();
		}

		/// <summary>
		/// Retrives all messages.
		/// </summary>
		/// <param name="conversation">The conversarion to get messages.</param>
		/// <returns>A collection of users <see cref="Message"/>.</returns>
		public async Task<ICollection<Message>> GetMessages(Conversation conversation)
		{
			Conversation? fetchedConversation = await GetFirstByConditionAsync(c => c.Id == conversation.Id,
														"Messages",
														"Messages.Autor"
														//"Messages.Reactions",
														//"Messages.Attachements"
														);

			if (fetchedConversation == null)
				return [];

			return fetchedConversation.Messages ?? new List<Message>();
		}

        /// <summary>
        /// Retrives all messages.
        /// </summary>
        /// <param name="conversation">The conversarion to get messages.</param>
        /// <returns>A collection of users <see cref="Message"/>.</returns>
		public string GetGroupName(Conversation conversation) 
		{
			return conversation.IsGroupConversation ? "Group_" : "Message_" + conversation.Id.ToString() ;
		}
    }
}
