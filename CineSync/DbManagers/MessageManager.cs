using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;
using CineSync.Data.Models;
using Microsoft.AspNetCore.SignalR.Protocol;
using System.Reflection;

namespace CineSync.DbManagers
{
    public class MessageManager : DbManager<Message>
    {
        private IRepositoryAsync<ApplicationUser> _userRepository;

        /// <summary>
		/// Initializes a new instance of the <see cref="MessageManager"/> class, which manages the movie-related database operations.
		/// </summary>
		/// <param name="unitOfWork">The unit of work handling database transactions.</param>
		/// <param name="logger">The logger for recording operations and exceptions.</param>
        public MessageManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
            _userRepository = _unitOfWork.GetRepositoryAsync<ApplicationUser>();
        }

        /// <summary>
		/// Creates a Message.
		/// </summary>
		/// <param name="message">The message to add.</param>
        /// <param name="conversationId">The id of the conversation.</param>
		/// <param name="userId"> The autor id <see cref="ApplicationUser"/> id </param>
		/// <returns>A created <see cref="Message"/> instance if successful, otherwise null.</returns>
		public async Task<Message?> CreateMessage( Message message, uint conversationId , string userId )
        {
            ApplicationUser? _autor = await _userRepository.GetFirstByConditionAsync(u => u.Id == userId);

            if ( _autor == null )
               return null;

            message.ConversationId = conversationId;
            message.Autor = _autor;

            await _repository.InsertAsync(message);

			if (await _unitOfWork.SaveChangesAsync()) 
				return message;
            else
                return null;
        }

		public async Task<ICollection<UserSeenMessages>> GetSeenByUsers(Message message)
		{
			Message? _dbMessage = await _repository.GetFirstByConditionAsync(m => m.Id == message.Id,
										"SeenByUsers"
									);

			if (_dbMessage == null) return [];

			return _dbMessage.SeenByUsers ?? [];
		}

		public async Task<ICollection<MessageAttachement>> GetAttachments(Message message)
		{
			Message? _dbMessage = await _repository.GetFirstByConditionAsync(m => m.Id == message.Id,
										"Attachements"
									);

			if (_dbMessage == null) return [];

			return _dbMessage.Attachements ?? [];
		}

		public async Task<ICollection<Reaction>> GetReactions(Message message)
		{
			Message? _dbMessage = await _repository.GetFirstByConditionAsync(m => m.Id == message.Id,
										"Reactions"
									);

			if(_dbMessage == null) return [];

			return _dbMessage.Reactions ?? [];
		}

		public async Task<Message?> GetReplyMessage(Message message)
		{
			Message? _dbMessage = await _repository.GetFirstByConditionAsync(m => m.Id == message.Id,
										"ReplayMessage",
										"ReplayMessage.Attachements",
										"ReplayMessage.Autor"
									);

			if (_dbMessage == null) return null;

			return _dbMessage.ReplayMessage;
		}
	}
}
