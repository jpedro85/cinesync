using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using CineSync.Components.Account;
using CineSync.Components.Layout;
using CineSync.Core.Email;
using CineSync.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;


namespace CineSync.Components.PopUps
{
    public partial class ChangeEmailPopUp : ComponentBase
    {
        [CascadingParameter(Name = "PageLayout")]
        private PageLayout PageLayout { get; set; }

        [SupplyParameterFromForm(FormName = "change-email")]
        private InputModel Input { get; set; } = new();
        
        [Inject]
        private UserManager<ApplicationUser> UserManager { get; set; }
        
        [Inject] 
        private IEmailSender EmailSender { get; set; }
        
        [Inject] 
        private IdentityUserAccessor UserAccessor { get; set; }
        
        [Inject] 
        private NavigationManager NavigationManager { get; set; }
        
        private PopUpLayout PopUpLayout { get; set; }

        private string message = string.Empty;
        
        private ApplicationUser user = default!;
        
        private string? email;
        
        private bool isEmailConfirmed;
        
        protected override async Task OnInitializedAsync()
        {
            user = PageLayout.AuthenticatedUser;
            email = user.Email;
            isEmailConfirmed = await UserManager.IsEmailConfirmedAsync(user);

            Input.NewEmail ??= email;
        }

        private async Task OnValidSubmitAsync()
        {
            if (Input.NewEmail is null || Input.NewEmail == email)
            {
                message = "Your email is unchanged.";
                StateHasChanged();
                return;
            }

            await SendConfirmationEmailLinkAsync(user);

            message = "Confirmation link to change email sent. Please check your email.";
            StateHasChanged();
        }

        private async Task SendConfirmationEmailLinkAsync(ApplicationUser user)
        {
            var userId = await UserManager.GetUserIdAsync(user);
            var code = await UserManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = NavigationManager.GetUriWithQueryParameters(
                NavigationManager.ToAbsoluteUri("Account/ConfirmEmailChange").AbsoluteUri,
                new Dictionary<string, object?> { ["userId"] = userId, ["email"] = Input.NewEmail, ["code"] = code });

            await EmailSender.SendEmailAsync(Input.NewEmail, "Confirm New Email Link",
                HtmlEncoder.Default.Encode(callbackUrl));
        }

        private sealed class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string? NewEmail { get; set; }
        }
    }
}