using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace CineSync.Components.Layout
{
	public partial class Loading : ComponentBase
	{
		[Parameter]
		public int FontSize { get; set; } = 80;

		[Parameter]
		public int LineHeight { get; set; } = 60;

		[Parameter]
		public int LetterSpacing { get; set; } = 8;

		[Parameter]
		public int MarginLeterLine { get; set; } = 25;

		[Parameter]
		public int FontPalceHeight { get; set; } = 40;

		[Parameter]
		public int MaxWidth { get; set; } = 480;
		public string _style { get; set; } = string.Empty;

		protected override void OnInitialized()
		{
			_style = $"margin-bottom:{MarginLeterLine}px;letter-spacing:{LetterSpacing}px;line-height:{LineHeight}px;font-size:{FontSize}px;";
		}
	}
}
