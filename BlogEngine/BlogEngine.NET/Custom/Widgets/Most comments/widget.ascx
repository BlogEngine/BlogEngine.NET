<%@ Control Language="C#" AutoEventWireup="true" Inherits="Widgets.MostComments.Widget" Codebehind="widget.ascx.cs" %>
<asp:Repeater runat="server" ID="rep">
    <HeaderTemplate>
        <table style="border-collapse: collapse">
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="vcard">
            <td style="padding-bottom: 7px">
                <asp:Image runat="server" ID="imgAvatar" class="photo" />
            </td>
            <td style="vertical-align: top; padding-left: 10px; line-height: 17px">
                <strong><asp:Literal runat="Server" ID="litName" /></strong><br />
                <asp:Literal runat="server" ID="litNumber" /> <asp:Image runat="server" ID="imgCountry" />
                <span class="adr"><span class="country-name"><asp:Literal runat="server" ID="litCountry" /></span>
                </span>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
