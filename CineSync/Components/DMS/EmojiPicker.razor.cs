using Microsoft.AspNetCore.Components;

namespace CineSync.Components.DMS
{
    public partial class EmojiPicker : ComponentBase
    {
        [Parameter] 
        public EventCallback<string> OnEmojiSelected { get; set; }

        private List<string> Emojis = new List<string> { "😊", "😂", "😍", "😭", "😒", "👍", "❤️", "🤔", "😎", "😢" };
    }
}