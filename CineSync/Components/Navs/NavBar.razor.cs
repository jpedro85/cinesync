using CineSync.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Navs
{
    public partial class NavBar
    {
        [Parameter]
        public bool HasSearch { get; set; } = false;

        [Parameter]
        public ApplicationUser? User { get; set; }

        public delegate void Change(bool status);
        
        private bool _isMenuOpen = false;
        public event Change? OnMenuChange;

        private bool _isNotificationOpen = false;
        public event Change? OnNotificationChange;

        private void OnclickNotification(MouseEventArgs e) 
        {
            _isNotificationOpen = !_isNotificationOpen;
            OnNotificationChange?.Invoke(_isNotificationOpen);
        }

        private void OnMenuClick(MouseEventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;
            OnMenuChange?.Invoke(_isMenuOpen);
        }
    }
}
