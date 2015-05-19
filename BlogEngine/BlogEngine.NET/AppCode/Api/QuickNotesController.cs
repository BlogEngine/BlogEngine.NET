using BlogEngine.Core;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Notes;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

public class QuickNotesController : ApiController
{
    private QuickNotes UserNotes { get; set; }

    public QuickNotesController()
    {
        UserNotes = new QuickNotes(Security.CurrentUser.Identity.Name);
    }

    [Authorize]
    public IEnumerable<QuickNote> Get()
    {
        return UserNotes.Notes;
    }

    [Authorize]
    public QuickNote Get(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        else
            return UserNotes.Notes.Where(n => n.Id.ToString() == id).FirstOrDefault();
    }

    [Authorize]
    public QuickNote Post([FromBody]QuickNote item)
    {
        var note = HttpUtility.HtmlAttributeEncode(item.Note);
        return UserNotes.SaveNote("", note);
    }

    [Authorize]
    public void Put([FromBody]QuickNote item)
    {
        UserNotes.SaveNote(item.Id.ToString(), item.Note);
    }

    [Authorize]
    public JsonResponse Delete(string id)
    {
        UserNotes.Delete(id);
        return new JsonResponse { Success = true };
    }
}