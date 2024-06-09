using CineSync.Data.Models;
using CineSync.Components.Layout;
using CineSync.Components.Utils;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Concurrent;

namespace CineSync.Components.Comments
{
    public partial class Comments : ComponentBase, IDisposable
    {
        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        // Maxsize is 4MB
        private const long MaxFileSize = 4 * 1024 * 1024;

        private ICollection<Comment> CommentsList { get; set; } = new List<Comment>(0);

        private MainLayout MainLayout { get; set; }

        private Comment comment = new Comment();

        private ConcurrentDictionary<IBrowserFile, byte[]> selectedFilesWithPreviews = new ConcurrentDictionary<IBrowserFile, byte[]>();

        private ConcurrentBag<string> ErrorMessages = new ConcurrentBag<string>();

        protected override void OnInitialized()
        {
            MainLayout = LayoutService.MainLayout;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CommentsList = await CommentManager.GetCommentsOfMovie(MovieId);
                StateHasChanged();
            }
        }


        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
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

        private async Task ProcessFile(IBrowserFile attchment, string fileType)
        {
            try
            {
                byte[] buffer = await ImageConverter.ReadImageAsBase64Async(attchment, MaxFileSize);
                selectedFilesWithPreviews[attchment] = buffer;
            }
            catch (Exception ex)
            {
                ErrorMessages.Add($"An error occurred while reading the file {attchment.Name}: {ex.Message}");
            }
        }

        private async void HandleSubmit()
        {
            if (!string.IsNullOrWhiteSpace(comment.Content))
            {
                await AddComment();
            }
        }

        private async Task AddComment()
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

            await CommentManager.AddComment(comment, MovieId, MainLayout.AuthenticatedUser.Id);

            comment = new Comment();
            selectedFilesWithPreviews.Clear();

            StateHasChanged();
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

    }
}
