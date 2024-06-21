using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.Buttons
{
	public partial class RectColoredButton
	{
		[Parameter]
		public string Text { get; set; } = "Buttom";

		[Parameter]
		public string DataDismiss { get; set; } = "";

		[Parameter]
		public EventCallback<MouseEventArgs> OnClick { get; set; }

		[Parameter]
		public string Style { get; set; } = "";
	}
}
