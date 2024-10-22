﻿using CineSync.Components.Layout;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class Classification : ComponentBase
    {
		[CascadingParameter(Name = "PageLayout")]
		public PageLayout PageLayout { get; set; }



        [Inject]
        private MovieManager MovieManager { get; set; }

        [Inject]
        private CollectionsManager CollectionsManager { get; set; }


		[Parameter]
        public int MovieAPIId { get; set; }

        [Parameter]
        public uint MovieId { get; set; }

        [Parameter]
        public EventCallback OnRatingSaved { get; set; }


        public ApplicationUser AuthenticatedUser { get; set; }

        private int Rating { get; set; }

        
        private void SetRating(ChangeEventArgs e)
        {
            Rating = Convert.ToInt32(e.Value);
        }

        private async void SaveRating()
        {
            await MovieManager.AddRating(Rating, MovieAPIId, PageLayout!.AuthenticatedUser.Id);
            await OnRatingSaved.InvokeAsync();
        }
    }
}

