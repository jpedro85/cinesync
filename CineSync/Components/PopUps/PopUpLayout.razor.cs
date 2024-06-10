using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace CineSync.Components.PopUps
{
    public partial class PopUpLayout : ComponentBase
    {
        [Inject]
        private IJSRuntime JS {  get; set; }

        [Parameter]
        public RenderFragment Header { get; set; }
        [Parameter]
        public RenderFragment Body { get; set; }
        [Parameter]
        public RenderFragment Footer { get; set; }
        [Parameter]
        public string Id { get; set; }

        public async void Close()
        {
            await JS.InvokeVoidAsync("hideModal", Id);
        }

        public async void Open()
        {
            await JS.InvokeVoidAsync("showModal", Id);
        }
	}
}
