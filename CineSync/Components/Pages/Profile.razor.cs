using CineSync.Core;
using CineSync.Components.PopUps;
using CineSync.Components.Account.Component;
using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Data.Models;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Mail;
using CineSync.Components.Layout;

namespace CineSync.Components.Pages
{
    public partial class Profile : ComponentBase
    {
        [Parameter]
        public string? UserId { get; set; }


        [Inject]
        public CommentManager DbCommentManage { get; set; }

        [Inject]
        public DbManager<Discussion> DbDiscussionsManage { get; set; }

        [Inject]
        public ApplicationDbContext ApplicationDbContext { get; set; }

        [Inject]
        public DbManager<UserLikedComment> DbUserLikedComment { get; set; }

        [Inject]
        public DbManager<UserDislikedComment> DbUserDislikedComment { get; set; }

        [Inject]
        public DbManager<UserLikedDiscussion> DbUserLikedDiscussion { get; set; }

        [Inject]
        public DbManager<UserDislikedDiscussion> DbUserDislikedDiscussion { get; set; }

        [Inject]
        public CollectionsManager CollectionManager { get; set; }

        [Inject]
        public UserImageManager UserImageManager { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }


        public UsernameEdit newuserName { get; set; }

        public ApplicationUser? User { get; set; }

        public ApplicationUser AuthenticatedUser { get; set; }



        public UserImage? UserImage { get; set; }

        private ICollection<MovieCollection>? _movieCollections = null;

        private ICollection<Comment>? _comments = null;

        private ICollection<UserLikedComment> _likedComents = new List<UserLikedComment>();

        private ICollection<UserDislikedComment> _dislikedComents = new List<UserDislikedComment>();

        private ICollection<UserLikedDiscussion> _likedDiscussions = new List<UserLikedDiscussion>();

        private ICollection<UserDislikedDiscussion> _dislikedDiscussions = new List<UserDislikedDiscussion>();

        private ICollection<Discussion>? _discussions = null;

        private string _activeTab = "Collections";

        private string[] _tabNames = { "Collections", "Comments", "Discussions", "Following", "Followers" };

        private bool _visit = false;

        private bool _invalid = false;

        private PageLayout _pageLayout;
        private bool _initialized = false;

        protected override async Task OnInitializedAsync()
        {
        }

        private async void Initialize()
        {
            AuthenticatedUser = _pageLayout.AuthenticatedUser!;

            if (string.IsNullOrEmpty(UserId) || UserId == AuthenticatedUser.Id || UserId == "0")
            {
                _visit = false;
                User = AuthenticatedUser;
                if (UserId == "0")
                {
                    _invalid = true;
                    return;
                }
                Console.WriteLine($"User{User?.Following?.Count},{User?.Followers?.Count}");
            }
            else
            {
                User = await UserManager.GetFirstByConditionAsync(u => u.Id == UserId, "Following", "Followers");
                Console.WriteLine($"User{User?.Following?.Count},{User?.Followers?.Count}");

                if (User == null)
                {
                    _invalid = true;
                    return;
                }
                else
                {
                    _visit = true;
                }

            }

            _initialized = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !_invalid)
            {
                FetchUserImage();
            }
        }

        private async void FetchUserImage()
        {
            UserImage = await UserImageManager.GetFirstByConditionAsync(image => image.UserId == User!.Id);
            StateHasChanged();
        }

        private async void OnProfileEdit()
        {
            if (UserImage == null)
            {
                FetchUserImage();
            }
            StateHasChanged();
            await _pageLayout.NavBar.ReRender();
        }

        private void UpdateMovieCollections()
        {
            _movieCollections = CollectionManager.GetUserCollections(User!.Id).Result;
            StateHasChanged();
        }

        private void UpdateComments()
        {
            _comments = DbCommentManage.GetByConditionAsync(
                        comment =>
                        comment.Autor.Id == User.Id, "Attachements"
                        ).Result.ToList();

            _likedComents = DbUserLikedComment.GetByConditionAsync(
                        likedComment =>
                        likedComment.Comment.Autor.Id == User!.Id &&
                        likedComment.UserId == AuthenticatedUser.Id
                        ).Result.ToList();

            _dislikedComents = DbUserDislikedComment.GetByConditionAsync(
                        dislikedComment =>
                        dislikedComment.Comment.Autor.Id == User!.Id &&
                        dislikedComment.UserId == AuthenticatedUser.Id
                        ).Result.ToList();

            StateHasChanged();
        }

        private void UpdateDiscussions()
        {
            _discussions = DbDiscussionsManage.GetByConditionAsync(discussion => discussion.Autor.Id == User!.Id, "Comments")
                          .Result
                          .ToList();

            _likedDiscussions = DbUserLikedDiscussion.GetByConditionAsync(
                        likedComment =>
                        likedComment.Discussion.Autor.Id == User!.Id &&
                        likedComment.UserId == AuthenticatedUser.Id
                        ).Result.ToList();

            _dislikedDiscussions = DbUserDislikedDiscussion.GetByConditionAsync(
                        dislikedComment =>
                        dislikedComment.Discussion.Autor.Id == User!.Id &&
                        dislikedComment.UserId == AuthenticatedUser.Id
                        ).Result.ToList();

            StateHasChanged();
        }

        private async void Follow()
        {
            if (await UserManager.Follow(AuthenticatedUser.Id, User!.Id))
            {
                if (AuthenticatedUser.Following == null)
                    AuthenticatedUser.Following = new List<ApplicationUser>();

                StateHasChanged();
                //LayoutService.MainLayout.TriggerMenuReRender();
            }
        }

        private async void UnFollow()
        {
            if (await UserManager.UnFollow(AuthenticatedUser.Id, User!.Id))
            {
                if (AuthenticatedUser.Following == null)
                    AuthenticatedUser.Following = new List<ApplicationUser>();

                AuthenticatedUser.Following = AuthenticatedUser.Following.Where(u => u.Id != User!.Id).ToList();

                StateHasChanged();
                //LayoutService.MainLayout.TriggerMenuReRender();

            }
        }

        private void OnTabChange(string tabName)
        {
            _activeTab = tabName;
            InvokeAsync(StateHasChanged);
        }

        private void GetPagelayout(PageLayout instance)
        {
            if (_pageLayout == null)
                _pageLayout = instance;
        }

    }
}
