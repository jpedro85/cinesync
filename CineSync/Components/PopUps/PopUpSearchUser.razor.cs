using CineSync.Data;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Collections;

namespace CineSync.Components.PopUps
{
    public partial class PopUpSearchUser : ComponentBase
    {
        [Parameter, EditorRequired]
        public ApplicationUser AuthenticatedUser { get; set; } = default!;

		[Parameter, EditorRequired]
        public string Id { get; set; } = string.Empty; 
        private string _id = "PopUpSearchUser_";

        [Parameter]
        public string Title { get; set; } = "Send Message to ..." ;

		[Parameter]
		public NewMessageFunc OnClickUser { get; set; } = (e,f) => { };
		public delegate void NewMessageFunc (ApplicationUser user, bool foolowing);

        [Inject]
        public UserManager DbUserManager { get; set; } = default!;

		[Inject]
		private ILookupNormalizer KeyNormalizer { get; set; } = default!;

		private PopUpLayout _popupLayout = default!;

		private string _searchContent = string.Empty;
		private bool _isLoading = false;
		private bool _MoreResultsLoading = false;
		private IEnumerable<ApplicationUser> _followingResults = [];
		private IEnumerable<ApplicationUser> _DbResults = [];
        public ICollection<ApplicationUser> _Following { get; set; } = [];


        protected override void OnInitialized()
        {
            _id = "PopUpSearchUser_" + Id;
			_Following = AuthenticatedUser.Following ?? []; 

		}

		public void Open() 
		{
			_popupLayout.Open();
		}

		public void Close() 
		{
			_popupLayout.Close();
		}

        private void OnClickSearch( string searchUsername ) 
        {
			_searchContent = searchUsername;
			_DbResults = [];
			_followingResults = [];

			if (searchUsername.IsNullOrEmpty()) 
				return;

			searchUsername = KeyNormalizer.NormalizeName( searchUsername.Trim() );

			Task.Run(() => { SearchUsername(searchUsername); });

			_isLoading = true;
			StateHasChanged();
        }

		private void SearchUsername(string searchUsername)
		{
            _followingResults = _Following.Where(u => u.Id != AuthenticatedUser.Id &&
													u.NormalizedUserName != null &&
													u.NormalizedUserName.Contains(searchUsername)
												);
			if (_followingResults.Count() == 0)
			{
				SearchUsernameInDb(searchUsername);
			}
			else
			{
				_isLoading = false;
				 InvokeAsync(StateHasChanged);
			}
		}

		private void OnClickMoreResults()
		{
			if (_searchContent.IsNullOrEmpty())
				return;

			var normalizedUsername = KeyNormalizer.NormalizeName( _searchContent.Trim() );

            Task.Run(() => { SearchUsernameInDb(normalizedUsername); });

			_MoreResultsLoading = true;
			StateHasChanged();
		}

        private async void SearchUsernameInDb ( string searchUsername ) 
        {
            _DbResults = await DbUserManager.GetByConditionAsync( u => u.Id != AuthenticatedUser.Id && 
																  u.NormalizedUserName != null &&
							                                      u.NormalizedUserName.Contains(searchUsername)
						                                        );

			_DbResults = _DbResults.Where( u => !_followingResults.Contains(u) );
			_isLoading = false;
			_MoreResultsLoading = false;
			await InvokeAsync(StateHasChanged);
		}

		private void OnInput( string search ) 
		{
			if (search.IsNullOrEmpty() )
			{
				_DbResults = [];
				_followingResults = [];
				InvokeAsync(StateHasChanged);
			}
		}

    }
}