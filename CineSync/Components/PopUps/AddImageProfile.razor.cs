using CineSync.Data;
using CineSync.DbManagers;
using CineSync.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace CineSync.Components.PopUps
{
    public partial class AddImageProfile : ComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }

        [Inject]
        public LayoutService LayoutService { get; set; }

        private const long MaxFileSize = 4 * 1024 * 1024;

        public ApplicationUser AuthenticatedUser { get; set; }

        private IBrowserFile selectedFile;

        private string ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = LayoutService.MainLayout.AuthenticatedUser;
        }

        private void HandleSelected(InputFileChangeEventArgs e)
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
        }

        private async Task UploadProfilePic()
        {
            if (selectedFile == null)
            {
                ErrorMessage = "Please select a valid image file.";
                return;
            }

            try
            {
                var buffer = new byte[selectedFile.Size];
                await selectedFile.OpenReadStream(MaxFileSize).ReadAsync(buffer);

                await SaveFileToDatabase(buffer);
                await JSRuntime.InvokeVoidAsync("window.location.reload");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading file: {ex.Message}";
            }
        }

        private async Task SaveFileToDatabase(byte[] fileData)
        {
            string userId = AuthenticatedUser.Id;
            string contentType = selectedFile.ContentType;
            await UserManager.ChangeProfilePictureAsync(userId, fileData, contentType);
        }
    }
}
