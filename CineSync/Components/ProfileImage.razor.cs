using Microsoft.AspNetCore.Components;

public partial class ProfileImage : ComponentBase
{
    [Parameter]
    public EventCallback OnProfileImageChanged { get; set; }

    private async Task SaveProfileImage()
    {
        // Implement your logic to save the profile image here

        // Raise event to notify parent component about profile image change
        await OnProfileImageChanged.InvokeAsync();
    }
}

