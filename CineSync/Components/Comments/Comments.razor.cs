using Microsoft.AspNetCore.Components.Forms;
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
   

        //void AddComment()
        //{
        //    // Add the new comment to the list
        //    comments.Add(new Commentss
        //    {
        //        UserId = newComment.UserId,
        //        CommentText = newComment.CommentText
        //    });
        //    Console.WriteLine(newComment.UserId);
        //    Console.WriteLine(newComment.CommentText);

        //    // Clear the new comment
        //    newComment = new Commentss();

        //    // Debug statement to check the contents of the comments list
        //    Console.WriteLine("Comments count after adding comment: " + comments.Count);
        //}

    }
}
