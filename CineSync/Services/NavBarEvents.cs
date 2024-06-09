using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Services
{
    public class NavBarEvents
    {
        public delegate void Change(bool status);
        public delegate void SearchEventHandler(string query);

        public bool IsMenuOpen = false;
        public event Change? OnMenuChange;

        public bool IsNotificationOpen = false;
        public event Change? OnNotificationChange;

        public event Func<Task> OnRequestNavBarReRender;

        public void OnclickNotification(MouseEventArgs e)
        {
            Console.WriteLine("Testing event count:" + OnNotificationChange?.GetInvocationList().Count());
            IsNotificationOpen = !IsNotificationOpen;
            OnNotificationChange?.Invoke(IsNotificationOpen);
        }

        public void OnMenuClick(MouseEventArgs e)
        {
            IsMenuOpen = !IsMenuOpen;
            Console.WriteLine("Testing event count:" + OnMenuChange?.GetInvocationList().Count());
            OnMenuChange?.Invoke(IsMenuOpen);
        }

        public async Task RequestNavBarReRender()
        {
            if (OnRequestNavBarReRender != null)
            {
                await OnRequestNavBarReRender.Invoke();
            }
            else
            {
                Console.WriteLine("[ERROR] The ReRender function was not subscribed properly");
            }
        }

    }
}
