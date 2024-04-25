using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
    public partial class Button
    {
        [Parameter]
        public string Title { get; set; } = "Back";
        public delegate void ButtonAction();
        public event ButtonAction? OnClick;

        private void OnMouseClick(MouseEventArgs e)
        {
            OnClick?.Invoke();
        }
    }
}
