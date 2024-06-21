namespace CineSync.Services
{
    public class MenuService
    {
        public event Func<Task> OnRequestMenuReRender;

        public async void RequestMenuReRender()
        {
           if( OnRequestMenuReRender != null)
                await OnRequestMenuReRender.Invoke();
        }
    }
}
