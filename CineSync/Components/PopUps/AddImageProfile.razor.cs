using CineSync.Components.Layout;
using CineSync.Components.Utils;
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
        [CascadingParameter(Name="PageLayout")]
        public PageLayout PageLayout { get; set; }


        [Parameter]
        public EventCallback OnImageChange { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        public UserManager UserManager { get; set; }


        private const long MaxFileSize = 4 * 1024 * 1024;

        private ApplicationUser? AuthenticatedUser { get; set; }

        private IBrowserFile selectedFile;

        private PopUpLayout PopUpLayout;

        private string ErrorMessage;

        protected override async Task OnInitializedAsync()
        {
            AuthenticatedUser = PageLayout!.AuthenticatedUser;
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
                byte[] buffer = await ImageConverter.ReadImageAsBase64Async(selectedFile, MaxFileSize);

                await SaveFileToDatabase(buffer);
                await OnImageChange.InvokeAsync();
                PopUpLayout.Close();

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
