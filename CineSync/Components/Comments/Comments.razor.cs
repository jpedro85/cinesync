using System;
using System.Threading.Tasks;

namespace CineSync.Components.Comments
{
    public partial class Commentss
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string CommentText { get; set; }
        public DateTime Created { get; set; }

        // Method to add a new comment
        public async Task AddComment()
        {
            // Assuming you have a service or repository to handle data operations,
            // you would call it here to add the new comment to your database or storage.

            // For demonstration purposes, let's just output the comment to the console.
            Console.WriteLine($"New comment added: {CommentText}");
            
                   }

    }
}
