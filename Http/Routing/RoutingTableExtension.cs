using Http.Attributes;
using Http.Controllers;
using Http.HTTP;
using System.ComponentModel.DataAnnotations;
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


        public static IRoutingTable MapControllers(this IRoutingTable routingTable)
        {
            IEnumerable<MethodInfo> controllerActions = GetControllerActions();

            foreach (var controllerAction in controllerActions)
            {
                string controllerName = controllerAction
                    .DeclaringType
                    .Name.Replace(nameof(Controller), string.Empty);

                string actionName = controllerAction.Name;
                string path = $"/{controllerName}/{actionName}";

                var responseFunction = GetResponseFunction(controllerAction);

                Method httpMethod = Method.GET;

                var actionMethodAttribute = controllerAction.GetCustomAttribute<HttpMethodAttribute>();

                if (actionMethodAttribute!=null)
                {
                    httpMethod = actionMethodAttribute.HttpMethod;
                }

                routingTable.Map(httpMethod, path, responseFunction);
            }
            return routingTable;
        }

        private static Func<Request, Response> GetResponseFunction(MethodInfo controllerAction)
            => request =>
        {
            var controllerInstance = CreateController(controllerAction.DeclaringType, request);
            var parameterValues = GetParameterValues(controllerAction, request);

            return (Response)controllerAction.Invoke(controllerInstance, parameterValues);
        };

        private static object[] GetParameterValues(MethodInfo controllerAction, Request request)
        {
            var actionParameters = controllerAction
                 .GetParameters()
                 .Select(p => new
                 {
                     p.Name,
                     p.ParameterType
                 })
                 .ToArray();

            var parameterValues = new object[actionParameters.Length];

            for (int i = 0; i < actionParameters.Length; i++)
            {
                var parameter = actionParameters[i];
                if (parameter.ParameterType.IsPrimitive||parameter.ParameterType==typeof(string))
                {
                    var parameterValue = request.GetValue(parameter.Name);
                    parameterValues[i] = Convert.ChangeType(parameterValue,parameter.ParameterType);
                }
                else
                {
                    var parameterValue=Activator.CreateInstance(parameter.ParameterType);
                    var parameterProperties=parameter.ParameterType.GetProperties();

                    foreach (var property in parameterProperties)
                    {
                        var propertyValue = request.GetValue(property.Name);
                        property.SetValue(parameter,Convert.ChangeType(propertyValue,property.PropertyType));
                    }

                    parameterValues[i] = parameterValue;
                }
            }

            return parameterValues;
        }

        private static string GetValue(this Request request, string name)
        => request.Query.GetValueOrDefault(name) ??
            request.Form.GetValueOrDefault(name);

        private static IEnumerable<MethodInfo> GetControllerActions()
        => Assembly
            .GetEntryAssembly()
            .GetExportedTypes()
            .Where(t => !t.IsAbstract == true)
            .Where(t => t.IsAssignableTo(typeof(Controller)))
            .Where(t => t.Name.EndsWith(nameof(Controller)))
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => m.ReturnType.IsAssignableTo(typeof(Response)))
            ).ToList();

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
