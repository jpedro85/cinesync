using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Buttons
{
	public partial class RectButton2 : ComponentBase
	{
		[Parameter]
		public string Text { get; set; } = "Buttom";

		[Parameter]
		public string DataDismiss { get; set; } = "";

		[Parameter]
		public Action OnClick { get; set; } = () => { };

		public delegate void Action();
	}
}
