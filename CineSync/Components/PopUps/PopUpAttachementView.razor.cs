using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class PopUpAttachementView
    {
        [Parameter]
        public string Id { get; set; }

        public byte[] Attachment { get; set; }

        public string Name { get; set; }

        private PopUpLayout _popUpLayout;

        public void TrigerStatehasChanged()
        {
            InvokeAsync(StateHasChanged);
        }

        public void Open()
        {
			_popUpLayout.Open();
        }

        public void Close()
        {
            _popUpLayout.Close();
        }
    }
}
