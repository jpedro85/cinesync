using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CineSync.Data.Models
{
	public class Invite
	{
		[Key]
		public uint Id { get; set; }

		public uint ConversationId { get; set; }

		[JsonIgnore]
		public Conversation Conversation { get; set; } = default!;

		public DateTime CreatedTimestanp { get; set; }

		public InviteTypes Type { get; set; } = InviteTypes.MESSAGE;

		[Required]
		public ApplicationUser Sender { get; set; } = default!;

		[Required]
		public ApplicationUser Target { get; set; } = default!;

		public InviteStates State { get; set; } = InviteStates.DEFAULT;

		public bool HideBySender { get; set; } = false;

        public bool HideByTarget { get; set; } = false;

        public string Name { get; set; } = string.Empty;

		public string GetTypeAsString() 
		{
			switch (Type)
			{
				case InviteTypes.MESSAGE:
					return "Message Exchange";
				case InviteTypes.GROUP:
					return "Join Group";
				default:
					return "";
			}
		}

		public string GetStateAsString()
		{
			switch (State)
			{
				case InviteStates.DEFAULT:
					return "Pending";
				case InviteStates.DECLINED:
					return "Declined";
				case InviteStates.ACDEPTED:
					return "Accepted";
				default:
					return "";
			}
		}

		public override bool Equals(object? obj)
		{

			if (obj == null || !(obj is Invite))
			{
				return false;
			}

			return ((Invite)obj).Id == this.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}