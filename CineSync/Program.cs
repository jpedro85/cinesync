using dotenv.net;

namespace CineSync
{
    public class Program
    {
        public static void Main(string[] args)
        {
			DotEnv.Load();

			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			ApplicationFacade serverConfigurationFacade = new ApplicationFacade( builder, "yyyy-MM-dd_HH-mm-ss");
			WebApplication app = serverConfigurationFacade.ConfigApplication();

			app.Run();
		}
    }
}