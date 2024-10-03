using Http.HTTP;

namespace Http.Responses
{
    public class UnauthorizedResponse:Response
    {
        public UnauthorizedResponse() :base(StatusCode.Unauthorized) { }
    }
}
