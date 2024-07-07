using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyModel;
using System.Security.Cryptography;

namespace CineSync.Components.PopUps
{
    public partial class PopUpAttachementView
    {
        static int counter = 0;

        [Parameter]
        public string Id { get; set; }
        private string _id = string.Empty;

        public byte[] Attachment { get; set; }

        public string Name { get; set; }

        private PopUpLayout _popUpLayout;

		protected override void OnInitialized()
		{
			counter++;
			_id = Id + counter.ToString();
		}

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
