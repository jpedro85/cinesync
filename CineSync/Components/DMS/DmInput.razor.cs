using System.Collections.Concurrent;
using CineSync.Components.PopUps;
using CineSync.Components.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.DMS
{
    public partial class DmInput : ComponentBase, IDisposable
    {
        // Maxsize is 4MB
        private readonly long MaxFileSize = 4 * 1024 * 1024;

        private string message = string.Empty;

        private ConcurrentDictionary<IBrowserFile, byte[]> selectedFilesWithPreviews = new();

        private ConcurrentDictionary<int, IBrowserFile> fileHashCodes = new();

        private ConcurrentBag<string> ErrorMessages = new();

        private PopUpAttachementView _attachmentViwer;

        private bool _clickRemoveAttachment = false;

        private ICollection<string> emojis = new[] { "😊", "😂", "😍", "😭", "😒", "👍" };

        private bool showEmojiPicker = false;


        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"Sending message: {message}");
                message = string.Empty;
            }
        }

        private void HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !e.ShiftKey)
            {
                SendMessage();
            }
        }

        private void ToggleEmojiPicker()
        {
            showEmojiPicker = !showEmojiPicker;
        }

        private void AddEmoji(string emoji)
        {
            message += emoji;
            showEmojiPicker = false;
        }
        
        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            ErrorMessages.Clear();

            IEnumerable<Task>? tasks = e.GetMultipleFiles(e.FileCount).Select(async attachment =>
            {
                string fileType = attachment.ContentType;

                if (attachment.Size > MaxFileSize)
                {
                    ErrorMessages.Add(
                        $"File {attachment.Name} size exceeds the limit of {MaxFileSize / (1024 * 1024)} MB. Please select a smaller file.");
                }
                else if (!fileType.StartsWith("image/"))
                {
                    ErrorMessages.Add($"{attachment.Name} has invalid file type. Please select an image.");
                }
                else
                {
                    await ProcessFile(attachment, fileType);
                }

                StateHasChanged();
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

        private void OpenAttachment(byte[] attachment, string fileName)
        {
            _attachmentViwer.Attachment = attachment;
            _attachmentViwer.Name = fileName;
            _attachmentViwer.TrigerStatehasChanged();
            _attachmentViwer.Open();
        }

        public void Dispose()
        {
            foreach (var key in selectedFilesWithPreviews.Keys.ToList())
            {
                selectedFilesWithPreviews.TryRemove(key, out _);
            }

            while (!ErrorMessages.IsEmpty)
            {
                ErrorMessages.TryTake(out _);
            }
        }
    }
}