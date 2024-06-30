using CineSync.Controllers.MovieEndpoint;
using CineSync.Core.Adapters.ApiAdapters;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;

namespace CineSync.Components.Buttons
{
    public partial class SearchButton
    {

        [Inject]
        private HttpClient _client { get; set; }

        [Parameter]
        public string SearchInput { get; set; } = string.Empty;

		[Parameter]
		public bool AllowNotEmpty { get; set; } = false;

		[Parameter]
        public string PlaceHolder { get; set; } = "Search";

        [Parameter]
		public int Heigth { get; set; } = 50;

		[Parameter]
		public int InputWidth { get; set; } = 200;

        [Parameter]
        public string Width { get; set; } = "0";

		[Parameter]
		public int SearchButtonWidth { get; set; } =  70;

		[Parameter]
		public int SearchButtonIconSize{ get; set; } = 20;

        [Parameter]
        public int PadingBorder { get; set; } = 20;

        [Parameter]
        public bool Animation { get; set; } = true;

		[Parameter]
        public EventCallback<string> OnSearch { get; set; }

        [Parameter]
        public bool ShowButton {  get; set; } = true;

        public delegate void Onkeydown(string search);

        [Parameter]
        public Onkeydown OnInput { get; set; } = (e) => { };

        //public delegate void searchresulthandler( string searchresults );
        //public event searchresulthandler? onsearch;

        private void OnInputChange(ChangeEventArgs e)
        {
            SearchInput = e.Value.ToString();
            OnInput(SearchInput ?? "");
        }

        private void SearchHandler( KeyboardEventArgs  e )
        {
            if ( OnSearch.HasDelegate && e.Key == "Enter" && ( AllowNotEmpty || !string.IsNullOrEmpty(SearchInput) ) )
            {
                OnSearch.InvokeAsync(SearchInput);
            }
        }

        private void SearchHandler( MouseEventArgs e )
        {
            if(OnSearch.HasDelegate && !string.IsNullOrEmpty(SearchInput) )
                OnSearch.InvokeAsync(SearchInput);
        }

	}
}
