using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.Converssations
{
    public partial class ItemMessage : ComponentBase
    {
        [Parameter, EditorRequired]
        public Message Message { get; set; } = default!;

        [Parameter, EditorRequired]
        public EventCallback<Message> OnReply { get; set; }

        [Parameter, EditorRequired]
        public EventCallback<Message> OnRemove {  get; set; }
        
    }
}
