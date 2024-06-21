using CineSync.Components.Layout;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using CineSync.Components.Utils;
using CineSync.Components.PopUps;

namespace CineSync.Components.Comments
{
    public partial class NewComment : ComponentBase, IDisposable
    {
        private static int _instanceConnter = 0;
        private int _instanceId = 0;

        [Parameter]
        public string TextAreaStyle { get; set; } = "";

        [Parameter]
        public string SelectedFileStyle { get; set; } = "";

        [Parameter]
        public string SelectedClass { get; set; } = "";

        // Maxsize is 4MB
        private const long MaxFileSize = 4 * 1024 * 1024;

        public Comment comment { get; set; } = new Comment();

        private ConcurrentDictionary<IBrowserFile, byte[]> selectedFilesWithPreviews = new ConcurrentDictionary<IBrowserFile, byte[]>();

        private ConcurrentDictionary<int, IBrowserFile> fileHashCodes = new ConcurrentDictionary<int, IBrowserFile>();

        private ConcurrentBag<string> ErrorMessages = new ConcurrentBag<string>();

        private ICollection<string> AuthenticatedUserRoles { get; set; } = [];

        private PopUpAttachementView _attachmentViwer;

        private bool _clicRemoveAttachment = false;

        protected override void OnInitialized()
        {
            _instanceConnter++;
            _instanceId = _instanceConnter;
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            Console.WriteLine($"Add:{_instanceId}");

            ErrorMessages.Clear();

            IEnumerable<Task>? tasks = e.GetMultipleFiles(e.FileCount).Select(async attachment =>
            {
                string fileType = attachment.ContentType;

                if (attachment.Size > MaxFileSize)
                {
                    ErrorMessages.Add($"File {attachment.Name} size exceeds the limit of {MaxFileSize / (1024 * 1024)} MB. Please select a smaller file.");
                }
                else if (!fileType.StartsWith("image/"))
                {
                    ErrorMessages.Add($"{attachment.Name} has invalid file type. Please select an image.");
                }
                else
                {
                    await ProcessFile(attachment, fileType);
                }
            });

            await Task.WhenAll(tasks);

        }

        private async Task ProcessFile(IBrowserFile attachment, string fileType)
        {
            try
            {
                byte[] buffer = await ImageConverter.ReadImageAsBase64Async(attachment, MaxFileSize);
                selectedFilesWithPreviews[attachment] = buffer;
                fileHashCodes[attachment.GetHashCode()] = attachment;
            }
            catch (Exception ex)
            {
                ErrorMessages.Add($"An error occurred while reading the file {attachment.Name}: {ex.Message}");
            }
        }

        private async void RemoveAttachment(int key)
        {
            int keyHashCode = key.GetHashCode();
            // IBrowserFile fileToRemove = selectedFilesWithPreviews.Keys.FirstOrDefault(file => file.GetHashCode() == key.GetHashCode());

            // if (fileToRemove != null)
            if (fileHashCodes.TryGetValue(keyHashCode, out var fileToRemove))
            {
                selectedFilesWithPreviews.TryRemove(fileToRemove, out _);
                fileHashCodes.TryRemove(keyHashCode, out _);
            }

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {

            if (selectedFilesWithPreviews != null)
            {
                foreach (var key in selectedFilesWithPreviews.Keys.ToList())
                {
                    selectedFilesWithPreviews.TryRemove(key, out _);
                }
            }

            if (ErrorMessages != null)
            {
                while (!ErrorMessages.IsEmpty)
                {
                    ErrorMessages.TryTake(out _);
                }
            }

        }

        private void OpenAttachement( byte[] attachment , string fileName) 
        {
            Console.WriteLine(fileName);
            _attachmentViwer.Attachment = attachment;
            _attachmentViwer.Name = fileName;
            _attachmentViwer.TrigerStatehasChanged();
            _attachmentViwer.Open();
        }

        public Comment GetComment()
        {
            comment.TimeStamp = DateTime.Now;
            comment.Attachements = new List<CommentAttachment>();

            foreach (var kvp in selectedFilesWithPreviews)
            {
                var attachment = new CommentAttachment()
                {
                    Attachment = kvp.Value,
                };
                comment.Attachements.Add(attachment);
            }

            return comment;
        }

        public void UpdateSpoilerState( bool newState ) 
        {
            comment.HasSpoiler = newState;
            StateHasChanged();
        }

        public void Reset()
        {
            comment = new Comment();
            selectedFilesWithPreviews.Clear();
            fileHashCodes.Clear();
            StateHasChanged();
        }

    }
}
