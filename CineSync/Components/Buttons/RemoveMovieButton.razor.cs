using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
    public partial class RemoveMovieButton
    {


        public delegate void ButtonAction();
        public event ButtonAction? OnClick;

        private void OnMouseClick(MouseEventArgs e)
        {
            OnClick?.Invoke();
        }
    }
}