using System.Collections.Concurrent;
using System.Net;
using CineSync.Components.PopUps;
using CineSync.Components.Utils;
using CineSync.Data;
using CineSync.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace CineSync.Components.DMS
{
    public partial class DmInput : ComponentBase, IDisposable
    {
		[Parameter, EditorRequired]
		public ApplicationUser AuthenticateUser { get; set; } = default!;

		[Parameter, EditorRequired]
        public EventCallback<Message> OnNewMessage { get; set; } = default!;

		[Parameter, EditorRequired]
		public EventCallback<Message> OnRemoveReply { get; set; }

		[Parameter]
        public string SelectedFileStyle { get; set; } = string.Empty;

		[Parameter]
		public string SelectedClass { get; set; } = string.Empty;


        // Maxsize is 4MB
        private readonly long MaxFileSize = 4 * 1024 * 1024;

        private ConcurrentDictionary<IBrowserFile, byte[]> selectedFilesWithPreviews = new();

        private ConcurrentDictionary<int, IBrowserFile> fileHashCodes = new();

        private ConcurrentBag<string> ErrorMessages = new();
        
        private bool _clickRemoveAttachment = false;

        private ICollection<string> emojis = new[] { "😊", "😂", "😍", "😭", "😒", "👍" };

        private bool showEmojiPicker = false;

        private bool _show = true;

        private Message newMessage = default!;

		protected override void OnInitialized()
		{
			newMessage = new Message()
			{
				Autor = AuthenticateUser,
			};
		}


		private void SendMessage()
        {
            ErrorMessages.Clear();
            if(selectedFilesWithPreviews.Count > 0) 
            {
                AddAttachments();
				OnNewMessage.InvokeAsync(newMessage);
				newMessage = new Message()
				{
					Autor = AuthenticateUser,
				};

                selectedFilesWithPreviews.Clear();
            }
            else if (!string.IsNullOrWhiteSpace(newMessage.Content)) 
            { 
				OnNewMessage.InvokeAsync(newMessage);
                newMessage = new Message()
                {
                    Autor = AuthenticateUser,
                };

                selectedFilesWithPreviews.Clear();
            }
        }

        private void AddAttachments() 
        {
			newMessage.Attachements = new List<MessageAttachement>();

			foreach (var kvp in selectedFilesWithPreviews)
			{
				var attachment = new MessageAttachement()
				{
					Attachment = kvp.Value,
				};
				newMessage.Attachements.Add(attachment);
			}
		}

		private void HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
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
			newMessage.Content += emoji;
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

        public void AddReply( Message message) 
        {
            newMessage.ReplayMessageId = message.Id;
            newMessage.ReplayMessage = message;
            InvokeAsync(StateHasChanged);
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