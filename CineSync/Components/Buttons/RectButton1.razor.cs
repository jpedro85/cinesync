using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Buttons
{
    public partial class RectButton1 : ComponentBase
    {
        [Parameter]
        public string Text { get; set; } = "Buttom";

        [Parameter]
        public string DataDismiss { get; set; } = "";

        [Parameter]
        public Action OnClick {  get; set; } = () => { };

        public delegate void Action();

        [Parameter]
        public string DataToggle { get; set; } = "";

		[Parameter]
		public string DataTarget { get; set; } = "";

	}
}
