using System.Web.Http;

public class SetupController : ApiController
{
    public string Get(string version)
    {
        var updater = new BlogEngine.Core.Data.Services.Updater();
        return updater.Check(version);
    }
}
