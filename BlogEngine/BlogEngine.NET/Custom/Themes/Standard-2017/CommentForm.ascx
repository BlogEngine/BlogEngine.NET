<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="BlogEngine.Core.Web.Controls.CommentFormBase" %>
<%@ Import Namespace="BlogEngine.Core" %>
<div class="comment-form well-global">
    <h3 class="well-global-title" id="addcomment"><%=Resources.labels.addComment %></h3>
    <div class="form-group">
        <input type="text" class="form-control" placeholder="<%=Resources.labels.name %>" name="txtName" id="txtName" />
    </div>
    <div class="form-group">
        <input type="text" placeholder="<%=Resources.labels.email %>" class="form-control" id="txtEmail" />
        <span id="gravatarmsg"></span>
    </div>
    <% if (BlogSettings.Instance.EnableWebsiteInComments)
        { %>
    <div class="form-group">
        <input type="text" placeholder="<%=Resources.labels.website%> (optional)" class="form-control" id="txtWebsite" />
    </div>
    <% } %>
    <div class="form-group" id="commentCompose">
        <textarea class="form-control" id="txtContent" placeholder="<%=Resources.labels.comment%>" cols="50" rows="7" name="txtContent"></textarea>
    </div>
    <% if (BlogSettings.Instance.ShowLivePreview)
        { %>
    <div class="form-group comment-preview">
        <label><%=Resources.labels.previewComment %></label>
        <div id="commentPreview">
            <img src="<%=Utils.RelativeWebRoot %>Content/images/blog/ajax-loader.gif" style="display: none" alt="Loading" />
        </div>
    </div>
    <% } %>
    <div id="commentCaptcha" class="form-group"></div>
    <div class="form-group">
        <label>
            <input type="checkbox" id="cbNotify" />
            <%=Resources.labels.notifyOnNewComments %>
        </label>
    </div>
    <input type="button" id="btnSaveAjax" value="<%=Resources.labels.saveComment %>" class="btn btn-primary btn-block" onclick="return BlogEngine.validateAndSubmitCommentForm()" />
</div>
