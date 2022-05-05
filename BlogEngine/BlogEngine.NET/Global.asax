<%@ Application Language="C#" %>
<%@ Import Namespace="BlogEngine.NET.App_Start" %>

<script RunAt="server">
    void Application_BeginRequest(object sender, EventArgs e)
    {
        var app = (HttpApplication)sender;
        BlogEngineConfig.Initialize(app.Context);
    }
        
    void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        BlogEngineConfig.SetCulture(sender, e);
    }

    protected void Application_PreSendRequestHeaders ()
    {
        var httpContext = HttpContext.Current;
        if (httpContext != null) {
            var cookieValueSuffix = "; SameSite=Strict";
            var cookies = httpContext.Response.Cookies;
            for (var i = 0; i < cookies.Count; i++)
            {
                var cookie = cookies[i]; cookie.Value += cookieValueSuffix;
            }
        }
    }
</script>