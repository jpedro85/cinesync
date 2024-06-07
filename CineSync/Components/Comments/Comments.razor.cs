using CineSync.Data.Models;
using CineSync.Components.Layout;
using CineSync.Components.Utils;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CineSync.Components.Comments
{
    public partial class Comments : ComponentBase
    {
        [Parameter]
        public int MovieId { get; set; }

        [Inject]
        private LayoutService LayoutService { get; set; }

        [Inject]
        private CommentManager CommentManager { get; set; }

        private const long MaxFileSize = 4 * 1024 * 1024;

        private ICollection<Comment> CommentsList { get; set; } = new List<Comment>(0);

        private MainLayout MainLayout { get; set; }

        private Comment comment = new Comment();

        private IBrowserFile selectedFile;

        private string selectedFilePreview;

        private string ErrorMessage;

        protected override async void OnInitialized()
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
            selectedFile = e.File;
            ErrorMessage = string.Empty;

            if (selectedFile == null) return;

            var fileType = selectedFile.ContentType;
            if (selectedFile.Size > MaxFileSize)
            {
                ErrorMessage = $"File size exceeds the limit of {MaxFileSize / (1024 * 1024)} MB. Please select a smaller file.";
                selectedFile = null;
            }
            else if (!fileType.StartsWith("image/"))
            {
                ErrorMessage = "Invalid file type. Please select an image.";
                selectedFile = null;
            }
            else
            {
                try
                {
                    byte[] buffer = await ImageConverter.ReadImageAsBase64Async(selectedFile, MaxFileSize);
                    selectedFilePreview = $"data:{fileType};base64,{Convert.ToBase64String(buffer)}";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"An error occurred while reading the file: {ex.Message}";
                    selectedFile = null;
                }
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

            if (selectedFile != null)
            {
                comment.Attachements = new List<CommentAttachment>(0);
                CommentAttachment attachment = new CommentAttachment()
                {
                    Attachment = await ImageConverter.ReadImageAsBase64Async(selectedFile, MaxFileSize),
                };
                comment.Attachements.Add(attachment);
            }

            await CommentManager.AddComment(comment, MovieId, MainLayout.AuthenticatedUser.Id);

            comment = null;
            comment = new Comment();

            selectedFile = null;
            selectedFilePreview = null;

            StateHasChanged();
        }

    }
}
