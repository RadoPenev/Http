using Http.Controllers;
using Http.HTTP;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(Request request):base(request)
        {
            
        }

        public Response Index() => Text("Hello from the server!");
    }
}
