using CineSync.Components.PopUps;
using Microsoft.AspNetCore.Components;
using CineSync.Data;

namespace CineSync.Components.Pages
{
    public partial class Profile
    
    {
        
        public  UsernameEdit newuserName {  get; set; }

        public ApplicationUser User { get; set; }
    }
}
