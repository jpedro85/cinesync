using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MimeKit.Cryptography;
using Newtonsoft.Json.Linq;

namespace CineSync.Components.PopUps
{
    public partial class PopUpInput : ComponentBase
    {
        [Parameter]
        public string Id { get; set; } = default!;

        [Parameter]
        public string Tilte { get; set; } = default!;

        [Parameter]
        public string Question { get; set; } = default!;

        [Parameter]
        public string PlaceHolder { get; set; } = default!;

        [Parameter]
        public OnSaveChange OnSave { get; set; } = default!;

        private string Value { get; set; } = string.Empty;
        private string _error = string.Empty;
 
        public delegate void OnSaveChange(string value);

        private PopUpLayout _popUpLayout = default!;

        public void Save(MouseEventArgs e) 
        {
            if(OnSave != null)
                OnSave(Value);
        }

        public void Open() 
        {
            _popUpLayout.Open();
        }

        public void Close()
        {
            _popUpLayout.Close();
        }

        public void SetError(string error)
        {
            _error = error;
        }
    }
}
