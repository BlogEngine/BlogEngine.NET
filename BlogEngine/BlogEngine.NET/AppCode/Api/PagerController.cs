using System.Web.Http;

public class PagerController : ApiController
{
    public string Get()
    {
        return BlogEngine.Core.Pager.Render();
    }
}
