namespace CineSync.Components.Utils
{
    public static class ElapsedTimeCalculator
    {
        /// <summary>
        /// Calculates the time difference between two timestamps.
        /// </summary>
        /// <param name="startTimestamp">The start timestamp.</param>
        /// <param name="endTimestamp">The end timestamp.</param>
        /// <returns>A TimeSpan representing the difference between the two timestamps.</returns>
        public static TimeSpan CalculateDifference(DateTime startTimestamp, DateTime endTimestamp)
        {
            // Ensure the end timestamp is not earlier than the start timestamp
            if (endTimestamp < startTimestamp)
            {
                throw new ArgumentException("End timestamp must be greater than or equal to start timestamp.");
            }

            return endTimestamp - startTimestamp;
        }

        /// <summary>
        /// Formats the time difference into a human-readable string.
        /// </summary>
        /// <param name="timeDifference">The time difference to format.</param>
        /// <returns>A string representing the time difference in a human-readable format.</returns>
        public static string FormatTimeDifference(TimeSpan timeDifference)
        {
            if (timeDifference.TotalMinutes < 1)
            {
                return "just now";
            }
            if (timeDifference.TotalMinutes < 60)
            {
                return $"{(int)timeDifference.TotalMinutes} minutes ago";
            }
            if (timeDifference.TotalHours < 24)
            {
                return $"{(int)timeDifference.TotalHours} hours ago";
            }
            if (timeDifference.TotalDays < 30)
            {
                return $"{(int)timeDifference.TotalDays} days ago";
            }
            if (timeDifference.TotalDays < 365)
            {
                return $"{(int)(timeDifference.TotalDays / 30)} months ago";
            }

            return $"{(int)(timeDifference.TotalDays / 365)} years ago";
        }

		/// <summary>
		/// Formats the time difference into a human-readable string.
		/// </summary>
		/// <param name="timeDifference">The time difference to format.</param>
		/// <returns>A string representing the time difference in a human-readable format.</returns>
		public static string FormatTimeDifferenceShort(TimeSpan timeDifference)
		{
			if (timeDifference.TotalMinutes < 1)
			{
				return "just now";
			}
			if (timeDifference.TotalMinutes < 60)
			{
				return $"{(int)timeDifference.TotalMinutes}min ago";
			}
			if (timeDifference.TotalHours < 24)
			{
				return $"{(int)timeDifference.TotalHours}h ago";
			}
			if (timeDifference.TotalDays < 30)
			{
				return $"{(int)timeDifference.TotalDays}d ago";
			}
			if (timeDifference.TotalDays < 365)
			{
				return $"{(int)(timeDifference.TotalDays / 30)} months ago";
			}

			return $"{(int)(timeDifference.TotalDays / 365)} years ago";
		}
    }
}
