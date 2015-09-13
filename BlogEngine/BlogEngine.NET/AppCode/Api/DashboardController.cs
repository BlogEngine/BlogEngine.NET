using App_Code;
using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.ViewModels;
using System.Net;
using System.Web.Http;

public class DashboardController : ApiController
{
    readonly IDashboardRepository repository;

    public DashboardController(IDashboardRepository repository)
    {
        if (!WebUtils.CheckRightsForAdminSettingsPage(true))
            throw new HttpResponseException(HttpStatusCode.Unauthorized);

        this.repository = repository;
    }

    public DashboardVM Get()
    {
        return repository.Get();
    }
}