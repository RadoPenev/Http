using Http.Controllers;
using Http.HTTP;
using System.Reflection;

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
                path, Request=> controllerFunction(CreateController<TController>(Request)));

        public static IRoutingTable MapPost<TController>(
            this IRoutingTable routingTable,
            string path,
            Func<TController, Response> controllerFunction) where TController : Controller
            => routingTable.Map(
                Method.POST,
                path, Request => controllerFunction(CreateController<TController>(Request)));


        private static TController CreateController<TController>(Request Request)
        => (TController)Activator.CreateInstance(typeof(TController), new[] {Request});

        private static Controller CreateController(Type controllerType, Request request)
        {
            var controller=(Controller)Request.ServiceCollection.CreateInstance(controllerType);

            controllerType
                .GetProperty("Request", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(controller,request);

            return controller;
        }
    }
}
