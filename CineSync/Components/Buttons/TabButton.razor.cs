using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Buttons
{
    public partial class TabButton : ComponentBase
    {
        private string _active = "";
        private string _Animation = "";

        public delegate void Action();

        [Parameter]
        public Action OnActionClick { get; set; } = () => {};

		[Parameter]
		public bool StartActive { get; set; } = false;

		[Parameter]
        public string Text { get; set; } = "Tab Button";

		protected override void OnInitialized()
		{
			if(StartActive)
            {
				_active = "active";
			}
		}

		private void OnMove( bool enter ) 
        {
            if (enter)
                _Animation = "active";
            else
                _Animation = "";
        }

        private void OnClick()
        {
            _active = "active";
            OnActionClick();
            InvokeAsync(() => { StateHasChanged(); });
        }

        public void deactivate()
        {
            _active = "";
            InvokeAsync(() => { StateHasChanged(); });
        }
    }
}
