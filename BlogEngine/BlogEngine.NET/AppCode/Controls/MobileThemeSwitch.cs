namespace App_Code.Controls
{
    using System;
    using System.Web.UI.WebControls;

    /// <summary>
    /// The <see cref="MobileThemeSwitch"/>
    ///   no longer used, needed so older themes won't break
    /// </summary>
    public class MobileThemeSwitch : LinkButton
    {
        public MobileThemeSwitch() { }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
    }
}