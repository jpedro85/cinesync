using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
    public partial class ToggleButton
    {
        [Parameter]
        public string Title { get; set; } = "Button";

        [Parameter]
        public bool InitialState {  get; set; } 

        public delegate void ButtonAction(bool state);

        [Parameter]
        public ButtonAction OnChange { get; set; }

        private void OnMouseClick()
        {
            InitialState = !InitialState;
            if(OnChange != null)
                OnChange(InitialState);
        }
    }
}
