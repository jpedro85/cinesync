﻿using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Services;

namespace CineSync.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        public UsernameEdit newuserName { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }

        private ICollection<MovieCollection>? movieCollections { get; set; }

        private UserImage? UserImage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
            movieCollections = await CollectionManager.GetUserCollections(AuthenticatedUser.Id);
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == AuthenticatedUser.Id);
                StateHasChanged();
            }
        }
    }

}
