using CineSync.Components.Layout;
using CineSync.DbManagers;

namespace CineSync.Services
{
    public class LayoutService
    {
        private readonly UserManager _userManager;

        public MainLayout MainLayout { get; set; }
    }
}
