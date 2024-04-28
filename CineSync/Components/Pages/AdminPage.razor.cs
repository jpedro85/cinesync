namespace CineSync.Components.Pages
{
    public partial class AdminPage
    {
        private string SelectedTab { get; set; }= "";
        public String Complaints = "Complaints";
        public String AccountStatus="Account Status";
        public String Permissions = "Permissions";
        private void SelectTab(string tabName)
        {
            SelectedTab = tabName;
        }
    }
}
