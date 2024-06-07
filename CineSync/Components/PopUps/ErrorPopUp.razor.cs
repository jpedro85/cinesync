using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CineSync.Components.PopUps
{
    public partial class ErrorPopUp : ComponentBase
    {
        [Parameter]
        public string Error { get; set; } = "";

        public PopUpLayout _layout;

        public void Close()
        {
            _layout.Close();
        }

        public void Open()
        {
            _layout.Open();
        }
    }
}
