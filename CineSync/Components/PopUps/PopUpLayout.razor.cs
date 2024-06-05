using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace CineSync.Components.PopUps
{
    public partial class PopUpLayout : ComponentBase
    {
        [Parameter]
        public RenderFragment Header { get; set; }
        [Parameter]
        public RenderFragment Body { get; set; }
        [Parameter]
        public RenderFragment Footer { get; set; }
        [Parameter]
        public string Id { get; set; }
	}
}
