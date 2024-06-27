using CineSync.Components.Discussions;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;

namespace CineSync.Components.PopUps
{
    public partial class PopUpStartDiscussion : ComponentBase
    {
        [Parameter,EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

        [Parameter, EditorRequired]
        public Comment Comment { get; set; } = default!;

        [Parameter]
        public EventCallback<Discussion> OnCreate { get; set; } = default;


        [Inject]
        public MovieManager MovieManager { get; set; } = default!;

        [Inject]
        public DiscussionManager DiscussionManager { get; set; } = default!;

        [Inject]
        public CommentManager CommentManager { get; set; } = default!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;


        private static uint _inctanceCounter = 0;

        private uint _Id = 0;

        private PopUpLayout _popUpLayout = default! ;

        private NewDiscussion _popUpNewDiscussion = default! ;

        protected override void OnInitialized()
        {
           _Id = _inctanceCounter++;
        }

        public void Open() 
        {
            _popUpLayout.Open();
        }

        private void Close()
        {
            _popUpLayout.Close();
        }

        private async void CreateDiscussion() 
        {
            if ( Comment != null )
            {
                Discussion newDiscussion = _popUpNewDiscussion.Discussion;

                if (newDiscussion.Title.IsNullOrEmpty())
                    return;

                Comment firstComment = ( await CommentManager.GetFirstByConditionAsync( c => c.Equals(Comment) ) )!;

                uint? MovieId = firstComment.MovieId; 

                if (MovieId == null)
                {
                    Discussion? discussion = ( await DiscussionManager.GetFirstByConditionAsync(d => d.Id == firstComment.DiscussionId ) )!;
                    MovieId = discussion?.MovieId;
                }

                if (MovieId == null)
                {
                    Console.WriteLine("Comentario with movieId:null discussionId:null");
                    return;
                }

                Movie? movie = await MovieManager.GetFirstByConditionAsync(
                                movie => movie.Id == firstComment.MovieId 
                                );

                bool result = false;

                result = await DiscussionManager.AddDiscussion(newDiscussion, movie, AuthenticatedUser.Id);
                result = result && await CommentManager.AddCommentToDiscussion(firstComment, newDiscussion.Id);

                if( result ) 
                {
                    _popUpNewDiscussion.Reset();
                    Close();
                    if (OnCreate.HasDelegate) 
                    {
                        await OnCreate.InvokeAsync(newDiscussion);
                    }
                }

            }
            else
            {
                Console.WriteLine("No Comment.");
            }
        }
    }
}