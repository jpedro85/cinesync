using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
    public partial class ToggleButton
    {
        [Parameter]
        public string Title { get; set; } = "Button";
        public bool state {  get; set; } 
        public delegate void ButtonAction();
        public event ButtonAction? OnChange;

        private void OnMouseClick(MouseEventArgs e)
        {
            OnChange?.Invoke();
        }
    }
}
