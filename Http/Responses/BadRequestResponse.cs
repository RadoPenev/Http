using Http.HTTP;

namespace Http.Responses
{
    public class BadRequestResponse:Response
    {
        public BadRequestResponse() :base(StatusCode.BadRequest) { }
    }
}
