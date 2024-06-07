using CineSync.Components.Buttons;

namespace CineSync.Components.Pages
{
    public partial class AdminPage
    {
        

        private TabButton TabButtonPermissions{ get; set; }
		private TabButton TabButtonComplaints { get; set; }
		private TabButton TabButtonAccountStatus { get; set; }

        private int Tab = 0;
    }
}
