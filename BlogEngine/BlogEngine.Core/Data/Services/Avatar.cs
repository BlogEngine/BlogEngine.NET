﻿using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BlogEngine.Core.Data.Services
{
    /// <summary>
    /// The avatar
    /// </summary>
    public class Avatar
    {
        private static string _noAvatar = $"{Utils.AbsoluteWebRoot}Content/images/blog/noavatar.jpg";
        private static string _pingImg = $"{Utils.AbsoluteWebRoot}Content/images/blog/pingback.png";

        /// <summary>
        /// Get avatar image source
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="website">Website URL</param>
        /// <returns>Path to avatar image</returns>
        public static string GetSrc(string email, string website = "")
        {
            var src = "";

            // thumb of pingback/trackback
            if (email == "pingback" || email == "trackback")
                return _pingImg;

            // author with profile photo
            src = Photo(email);
            if (!string.IsNullOrEmpty(src))
                return src;

            // image from gravatar service
            src = Gravatar(email);
            if (!string.IsNullOrEmpty(src))
                return src;

            // theme specific noavatar
            src = ThemeNoAvatar(email);
            if (!string.IsNullOrEmpty(src))
                return src;

            // default noavatar if nothing worked
            return $"{Utils.AbsoluteWebRoot}Content/images/blog/noavatar.jpg";
        }

        static string ThemeNoAvatar(string email)
        {
            var themeAvatar =
                $"{Utils.ApplicationRelativeWebRoot}Custom/Themes/{BlogSettings.Instance.Theme}/noavatar.jpg";

            if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(themeAvatar)))
                return themeAvatar;

            return "";
        }

        static string Gravatar(string email)
        {
            if (email == null)
                return "";

            //var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5");
            var hash = GetHash(email.ToLowerInvariant().Trim());

            if (hash != null) 
                hash = hash.ToLowerInvariant();

            var gravatar = $"http://www.gravatar.com/avatar/{hash}.jpg?d=";

            switch (BlogSettings.Instance.Avatar)
            {
                case "identicon":
                    return gravatar + "identicon";
                case "wavatar":
                    return gravatar + "wavatar";
                case "retro":
                    return gravatar + "retro";
                case "mm":
                    return gravatar + "mm";
                case "blank":
                    return gravatar +  "blank";
                case "monsterid":
                    return gravatar + "monsterid";
                default:
                    return "";
            }
        }

        static string Photo(string email)
        {
            var pf = AuthorProfile.GetProfileByEmail(email) ?? new AuthorProfile();

            if (string.IsNullOrEmpty(pf.PhotoUrl))
                return "";

            var img = pf.PhotoUrl.Replace("\"", "");

            return img.StartsWith("http://") || img.StartsWith("https://") ? img :
                Utils.RelativeWebRoot + "image.axd?picture=/avatars/" + img;
        }

        static string GetHash(string value)
        {
            MD5 algorithm = MD5.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string md5 = "";
            for (int i = 0; i < data.Length; i++)
            {
                md5 += data[i].ToString("x2").ToUpperInvariant();
            }
            return md5;
        }
    }
}
