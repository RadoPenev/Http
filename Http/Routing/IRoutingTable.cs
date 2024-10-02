using Http.HTTP;

namespace Http.Routing
{
    public interface IRoutingTable
    {
        IRoutingTable Map(Method method,string path,Func<Request,Response> responseFunction);
   
    }
}
