using Microsoft.AspNetCore.Components.Web;
using static CineSync.Components.Navs.NavBar;

namespace CineSync.Services
{
    public class NavBarEvents
    {
        public delegate void Change(bool status);

        private bool _isMenuOpen = false;
        public event Change? OnMenuChange;

        private bool _isNotificationOpen = false;
        public event Change? OnNotificationChange;

        public void OnclickNotification(MouseEventArgs e)
        {
            _isNotificationOpen = !_isNotificationOpen;
            OnNotificationChange?.Invoke(_isNotificationOpen);
        }

        public void OnMenuClick(MouseEventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;
            bool a = OnMenuChange == null;
            Console.WriteLine("Event %b %b" + a + ";" + _isMenuOpen);
            OnMenuChange?.Invoke(_isMenuOpen);
        }


    }
}
