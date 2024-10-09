using Demo.Controllers;
using Http.Routing;

namespace Http
{
    public class Startup
    {
        public static async Task Main()
        {

            await new HttpServer(routes => routes
                 .MapControllers()
                 ).Start();
        }
    }

}

