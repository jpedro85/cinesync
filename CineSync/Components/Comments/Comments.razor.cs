using CineSync.Data.Models;
using CineSync.Data;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Comments
{
    public partial class Commentss
    {
        public uint Id { get; set; }
        [Parameter]
        public ApplicationUser? Autor { get; set; }

        public long NumberOfLikes { get; set; } = 0;

        public long NumberOfDeslikes { get; set; } = 0;

        public string? Content { get; set; }

        public ICollection<CommentAttachment>? Attachements { get; set; }


    
    }
}
