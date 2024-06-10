using CineSync.Data.Models;
using CineSync.DbManagers;
using Microsoft.AspNetCore.Components;

namespace CineSync.Components.PopUps
{
    public partial class RemoveComment
    {
        public delegate void CallbackAction();

        [Parameter]
        public Action Callback { get; set; } = () => { };
    }
}
