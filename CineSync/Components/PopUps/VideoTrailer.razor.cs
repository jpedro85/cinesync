using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class VideoTrailer : ComponentBase
    {
        [Parameter]
        public string TrailerLink { get; set; }

        private PopUpLayout PopupLayout { get; set; }

        private bool _stop = false;
        private const byte delayToClosePopUp = 100;

        public void Open()
        {
            _stop = false;
            StateHasChanged();
            PopupLayout.Open();
        }

        public async void Close()
        {
            _stop = true;
            PopupLayout.Close();

            await Task.Delay(delayToClosePopUp);
            await InvokeAsync(StateHasChanged);
        }
    }
}
