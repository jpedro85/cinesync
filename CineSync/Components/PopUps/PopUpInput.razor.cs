using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;

namespace CineSync.Components.PopUps
{
    public partial class PopUpInput : ComponentBase
    {
        [Parameter]
        public string Tilte { get; set; }

        [Parameter]
        public string Question { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; }

        [Parameter]
        public OnSaveChange OnSave { get; set; }

        private string Value { get; set; }


        public delegate void OnSaveChange(string value);

        public void Save(MouseEventArgs e) 
        {
            if(OnSave != null)
                OnSave(Value);
        }
    }
}
