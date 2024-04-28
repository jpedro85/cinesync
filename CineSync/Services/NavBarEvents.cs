using Microsoft.AspNetCore.Components.Web;
using static CineSync.Components.Navs.NavBar;

namespace CineSync.Services
{
    public class NavBarEvents
    {
        public delegate void Change(bool status);
		public delegate void SearchEventHandler( string query );

		public event SearchEventHandler? OnSearchFromNavBar;
		public event SearchEventHandler? OnSearchFromPage;

		public bool IsMenuOpen = false;
        public event Change? OnMenuChange;

        public bool IsNotificationOpen = false;
        public event Change? OnNotificationChange;

        public void OnclickNotification(MouseEventArgs e)
        {
            IsNotificationOpen = !IsNotificationOpen;
            OnNotificationChange?.Invoke(IsNotificationOpen);
        }

        public void OnMenuClick(MouseEventArgs e)
        {
            IsMenuOpen = !IsMenuOpen;
            OnMenuChange?.Invoke(IsMenuOpen);
        }

        public void OnClickSearch( string query )
        {
			OnSearchFromNavBar?.Invoke( query );
		}

        public void InvokeOnSearchFromPage(string query )
        {
            OnSearchFromPage?.Invoke(query);
		}

	}
}
