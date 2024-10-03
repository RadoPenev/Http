using Http.HTTP;

namespace Http.Responses
{
    public class NotFoundResponse:Response
    {
        public NotFoundResponse() :base(StatusCode.NotFound) { }
    }
}
