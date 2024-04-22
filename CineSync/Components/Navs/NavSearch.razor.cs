using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Navs
{
    public partial class NavSearch
    {
        private string _searchActive = string.Empty;

        private string _currentSearch = string.Empty;

        public delegate void Searchhandler(string search);
        public delegate void openMenu();

        public event Searchhandler? OnSearch;

        private void OutSearch()
        {
            _searchActive = "";
        }

        private void OnSearchClick(MouseEventArgs e)
        {
            if (_currentSearch != string.Empty)
                OnSearch?.Invoke(_currentSearch);
        }

        private void ClickInput(MouseEventArgs e)
        {
            _searchActive = "Active";
        }
    }
}
