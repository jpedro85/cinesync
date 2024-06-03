﻿using CineSync.Data.Models;
using CineSync.Components.Layout;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Comments
{
    public partial class Comments : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        private MainLayout MainLayout { get; set; }

        private static ICollection<Comment> CommentsList { get; set; } = new List<Comment>(0);

        private Comment comment = new Comment();

        protected override async void OnInitialized()
        {
            MainLayout = LayoutService.MainLayout;
            if(MovieId != null)
             CommentsList = await CommentManager.GetCommentsOfMovie(MovieId);
        }

        private async void HandleSubmit()
        {
            if (!string.IsNullOrWhiteSpace(comment.Content))
            {
                await AddComment();
            }
        }

        private async Task AddComment()
        {
            comment.TimeStamp = DateTime.Now;

            await CommentManager.AddComment(comment, MovieId, MainLayout.AuthenticatedUser.Id);

            CommentsList.Add(comment);
            comment = new Comment();
            StateHasChanged();
        }

        private async void AddLike(Comment commentAddLike)
        {
			await CommentManager.AddLikeAsync( commentAddLike );
            StateHasChanged();
        }

		private async void AddDeslike(Comment commentAddDesLike)
		{
            await CommentManager.AddDesLikeAsync(commentAddDesLike);
            StateHasChanged();
		}

	}
}
