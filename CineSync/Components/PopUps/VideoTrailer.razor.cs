using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class VideoTrailer : ComponentBase
    {
        [Parameter]
        public string TrailerLink { get; set; }

    }
}
