using System.ComponentModel.DataAnnotations;
using CineSync.Components.Account;
using CineSync.Components.Layout;
using CineSync.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace CineSync.Components.PopUps
{
    public partial class ChangePasswordPopUp : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        private PageLayout PageLayout { get; set; }
        
        [SupplyParameterFromForm] 
        private InputModel Input { get; set; } = new();
        
        [Inject]
        private IdentityRedirectManager RedirectManager { get; set; }
        
        [Inject] 
        private UserManager<ApplicationUser> UserManager { get; set; }

        private PopUpLayout PopUpLayout;
        
        private string? message;

        private ApplicationUser user;
        
        private bool hasPassword;

        protected override async Task OnInitializedAsync()
        {
            user = PageLayout.AuthenticatedUser;
            hasPassword = await UserManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                RedirectManager.RedirectTo("Account/Manage/SetPassword");
            }
        }

        private async Task OnValidSubmitAsync()
        {
            message = string.Empty;
            var changePasswordResult = await UserManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                message = $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}";
                return;
            }

            message = "Password has changed successfully";
        }
        
        private void ClearMessage()
        {
            message = string.Empty;
        }


        private sealed class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; } = "";

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; } = "";

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}