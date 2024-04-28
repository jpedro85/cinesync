using CineSync.Components.Buttons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
    public partial class Dropdown
    {
        Dropdown dropdown;
        public List<string> collections = new List<string> { "Coleção 1", "Coleção 2", "Coleção 3" };
        public int i = 4;

     
        private void AddNewCollection()
        {
            collections.Add($"Coleção{++i}");
            dropdown.StateHasChanged();
        }
		
	}
}

