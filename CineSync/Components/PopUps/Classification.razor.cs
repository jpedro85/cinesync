namespace CineSync.Components.PopUps
{
    public class ClassificationPop
    {
		public int Rating { get; set; }

        public void SetRating(int rating)
        {
            Rating = rating;
        }

		public void SaveRating()
        {
            // Implement save rating logic here
            // For demonstration purposes, we'll just print the rating to the console
            Console.WriteLine($"Rating saved: {Rating}");
        }
    }
}

