using CineSync.Services;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Layout
{
    public partial class Menu : ComponentBase
    {
        [Inject]
        private NavBarEvents NavBarEvents { get; set; }

        private string IsActive { get; set; } = string.Empty;

        public void ChangeState( bool active)
        {
            Console.WriteLine("A:" + active);
            if (active)
            {
                IsActive = "Active";
            }
            else
            {
                IsActive = "";
            }

            InvokeAsync( () =>
            {
                StateHasChanged();
            });

        }


        protected override void OnInitialized()
        {
            NavBarEvents.OnMenuChange += this.ChangeState;
        }

    }
}
