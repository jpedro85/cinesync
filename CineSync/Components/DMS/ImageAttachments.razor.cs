using System.Collections.Concurrent;
using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CineSync.Components.DMS
{
    public partial class ImageAttachments : ComponentBase, IDisposable
    {
        [Parameter]
        public string SelectedFileStyle { get; set; } = string.Empty;

        [Parameter]
        public string SelectedClass { get; set; } = string.Empty;

        [Parameter] 
        public ConcurrentDictionary<IBrowserFile, byte[]> SelectedFilesWithPreviews { get; set; }

        [Parameter]
        public EventCallback<int> OnRemove { get; set; }

        private bool _clickRemoveAttachment = false;
        
        private PopUpAttachementView _attachmentViwer;
        
        private void OpenAttachment(byte[] attachment, string fileName)
        {
            _attachmentViwer.Attachment = attachment;
            _attachmentViwer.Name = fileName;
            _attachmentViwer.TrigerStatehasChanged();
            _attachmentViwer.Open();
        }
        
        public void Dispose()
        {
            foreach (var key in SelectedFilesWithPreviews.Keys.ToList())
            {
                SelectedFilesWithPreviews.TryRemove(key, out _);
            }
        }
    }
}