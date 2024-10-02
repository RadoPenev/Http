using Http.Controllers;
using Http.HTTP;

namespace Http.Routing
{
    public static class RoutingTableExtension
    {
        public static IRoutingTable MapGet<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController,Response> controllerFunction) where TController : Controller
            => routingTable.Map(
                Method.GET,
                path, request=> controllerFunction(CreateController<TController>(request)));

        public static IRoutingTable MapPost<TController>(
            this RoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction) where TController : Controller
            => routingTable.Map(
                Method.POST,
                path, request => controllerFunction(CreateController<TController>(request)));


        private static TController CreateController<TController>(Request request)
        => (TController)Activator.CreateInstance(typeof(TController), new[] {request});
    }
}
