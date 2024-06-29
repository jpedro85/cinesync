using CineSync.Components.Buttons;
using Humanizer;
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

        [Parameter,EditorRequired]
        public TabChange OnTabChange { get; set; } = (s) => { };

        [Parameter]
        public string Background_Style { get; set; } = "";

        [Parameter]
        public int FontSize { get; set; } = 24;

        private TabButton[] TabButtons;

        protected override void OnInitialized()
        {
            TabButtons = new TabButton[TabNames.Length];
            _activeTab = ActiveTab;
        }

		public void ChangeTab( string TabName) 
        {
            int TabIndex = 0;
            foreach (var item in TabNames)
            {
                if (item == TabName)
                    break;

                TabIndex++;
            }

            ChangeTab(TabIndex);
            OnTabChange.Invoke(TabName);
		}

        private void ChangeTab(int indice)
        {
			TabButtons[_activeTab].deactivate();
			_activeTab = indice;
			OnTabChange(TabNames[indice]);
		}
	}
}
