using CineSync.Components.Buttons;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Buttons
{
    public partial class TabButtonsBar : ComponentBase
    {
        [Parameter]
        public string[] TabNames { get; set; }
        [Parameter]
        public int ActiveTab { get; set; } = 0;
        private int _activeTab = 0;

        public delegate void TabChange( string TabName );

        [Parameter]
        public TabChange OnTabChange { get; set; } = (s) => { };

        [Parameter]
        public string Background_Style { get; set; } = "";

        private TabButton[] TabButtons;

        protected override void OnInitialized()
        {
            TabButtons = new TabButton[TabNames.Length];
            _activeTab = ActiveTab;
        }
    }
}
