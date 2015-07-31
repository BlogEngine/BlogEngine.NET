using BlogEngine.Core.Providers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace BlogEngine.Core
{
    /// <summary>
    /// This class should generate new blog from scratch
    /// instead of using existing blog as a template
    /// This way it can be used by self-registed users
    /// to create pre-configured blog on the fly.
    /// </summary>
    public class BlogGenerator
    {
        /// <summary>
        /// Creates new blog
        /// </summary>
        /// <param name="blogName">Name of the blog</param>
        /// <param name="userName">Name of the first user which will be set up as blog admin</param>
        /// <param name="email">Email address</param>
        /// <param name="password">Password</param>
        /// <param name="message">Return message</param>
        /// <returns>New blog object</returns>
        public static Blog CreateNewBlog(string blogName, string userName, string email, string password, out string message)
        {
            message = null;
            blogName = blogName.Trim();

            if (!ValidateProperties(blogName, userName, email, out message))
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Validation for new blog failed.";
                }
                return null;
            }

            Blog newBlog = new Blog()
            {
                Name = blogName,
                StorageContainerName = blogName.ToLower(),
                Hostname = "",
                IsAnyTextBeforeHostnameAccepted = true,
                VirtualPath = "~/" + blogName.ToLower(),
                IsActive = true,
                IsSiteAggregation = false
            };

            bool setupResult = false;
            try
            {
                setupResult = BlogService.SetupNewBlog(newBlog, userName, email, password);
            }
            catch (Exception ex)
            {
                Utils.Log("BlogGenerator.CreateNewBlog", ex);
                message = "Failed to create new blog. Error: " + ex.Message;
                return null;
            }

            if (!setupResult)
            {
                message = "Failed during process of setting up new blog instance.";
                return null;
            }

            // save the blog for the first time.
            newBlog.Save();

            return newBlog;
        }

        /// <summary>
        /// Validates the blog properties.
        /// </summary>
        public static bool ValidateProperties(string blogName, string userName, string email, out string message)
        {
            message = null;

            if (string.IsNullOrWhiteSpace(blogName))
            {
                message = "Blog Name is Required.";
                return false;
            }

            Regex validChars = new Regex("^[a-z0-9-_]+$", RegexOptions.IgnoreCase);

            if (!string.IsNullOrWhiteSpace(blogName) && !validChars.IsMatch(blogName))
            {
                message = "Blog Name contains invalid characters.";
                return false;
            }

            string[] reserved = BlogConfig.ReservedBlogNames.Split('|');

            if (reserved != null && reserved.GetUpperBound(0) > 0)
            {
                for (int i = 0; i < reserved.GetUpperBound(0); i++)
                {
                    if (blogName.ToLower() == reserved[i].ToLower())
                    {
                        message = "This name is reserved and can not be used as a blog name, please pick another one.";
                        return false;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(userName) && !validChars.IsMatch(userName))
            {
                message = "User Name contains invalid characters.";
                return false;
            }

            if (Blog.Blogs.Where(b => b.Name.ToLower() == blogName.ToLower()).FirstOrDefault() != null)
            {
                message = "Blog with this name already exists; Please select different name.";
                return false;
            }

            if(Directory.Exists(HostingEnvironment.MapPath(Path.Combine(BlogConfig.StorageLocation, "blogs", blogName))))
            {
                message = "Blog with this name already exists; Please select different name";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copy template files for new blog
        /// </summary>
        /// <param name="blogName">Name of the blog</param>
        /// <param name="userName">User name</param>
        /// <param name="email">Email address</param>
        /// <param name="password">Password</param>
        /// <returns>True if successful</returns>
        public static bool CopyTemplateBlogFolder(string blogName, string userName, string email, string password)
        {
            string templateUrl = Path.Combine(BlogConfig.StorageLocation, "blogs", "_new");
            string newBlogUrl = Path.Combine(BlogConfig.StorageLocation, "blogs", blogName);
            string templateFolderPath = HostingEnvironment.MapPath(templateUrl);
            string newBlogFolderPath = HostingEnvironment.MapPath(newBlogUrl);
            try
            {

                if (!Directory.Exists(templateFolderPath))
                {
                    throw new Exception(string.Format("Template folder for new blog does not exist.  Directory not found is: {0}", templateFolderPath));
                }
            }
            catch (Exception ex)
            {
                Utils.Log("BlogGenerator.CopyTemplateBlogFolder", ex);
                throw;  // re-throw error so error message bubbles up.
            }

            // If newBlogFolderPath already exists, throw an exception as this may be a mistake
            // and we don't want to overwrite any existing data.
            try
            {
                if (Directory.Exists(newBlogFolderPath))
                {
                    throw new Exception(string.Format("Blog destination folder already exists. {0}", newBlogFolderPath));
                }
            }
            catch (Exception ex)
            {
                Utils.Log("BlogGenerator.CopyTemplateBlogFolder", ex);
                throw;  // re-throw error so error message bubbles up.
            }
            if (!Utils.CreateDirectoryIfNotExists(newBlogFolderPath))
                throw new Exception(string.Format("Can not create blog directory: {0}", newBlogFolderPath));

            // Copy the entire directory contents.
            DirectoryInfo source = new DirectoryInfo(templateFolderPath);
            DirectoryInfo target = new DirectoryInfo(newBlogFolderPath);

            try
            {
                Utils.CopyDirectoryContents(source, target, new List<string>() { BlogConfig.BlogInstancesFolderName });

                string msg = "";
                BlogGeneratorConfig.SetDefaults(out msg);

                string pubDate = DateTime.Now.ToString(
                        "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var usersFile = newBlogFolderPath + @"\users.xml";
                ReplaceInFile(usersFile, "[UserName]", userName);
                ReplaceInFile(usersFile, "[Password]", Utils.HashPassword(password));
                ReplaceInFile(usersFile, "[Email]", email);

                var rolesFile = newBlogFolderPath + @"\roles.xml";
                ReplaceInFile(rolesFile, "[UserName]", userName);

                var settingsFile = newBlogFolderPath + @"\settings.xml";
                ReplaceInFile(settingsFile, "[BlogName]", blogName);
                ReplaceInFile(settingsFile, "[Description]", BlogGeneratorConfig.BlogDescription);

                // generate unique category id
                var categoriesFile = newBlogFolderPath + @"\categories.xml";
                var catId = Guid.NewGuid().ToString();
                ReplaceInFile(categoriesFile, "[CategoryId]", catId);

                var postFile = newBlogFolderPath + @"\posts\" + BlogGeneratorConfig.PostsFileName;
                ReplaceInFile(postFile, "[CategoryId]", catId);
                ReplaceInFile(postFile, "[PubDate]", pubDate);
                ReplaceInFile(postFile, "[Author]", userName);

                var postTitle = "";

                if (BlogGeneratorConfig.PostTitle.Contains("{0}"))
                {
                    postTitle = string.Format(BlogGeneratorConfig.PostTitle, blogName);
                }
                else
                {
                    postTitle = BlogGeneratorConfig.PostTitle;
                }

                ReplaceInFile(postFile, "[Title]", postTitle);
                ReplaceInFile(postFile, "[Slug]", Utils.RemoveIllegalCharacters(postTitle));
                
                if (BlogGeneratorConfig.PostContent.Contains("{0}") && BlogGeneratorConfig.PostContent.Contains("{1}"))
                {
                    ReplaceInFile(postFile, "[Post]", string.Format(BlogGeneratorConfig.PostContent, userName,
                        Utils.RelativeWebRoot + blogName + "/Account/login.aspx"));
                }
                else
                {
                    ReplaceInFile(postFile, "[Post]", BlogGeneratorConfig.PostContent);
                }

                // update post with new GUID
                var newPostFile = newBlogFolderPath + @"\posts\" + Guid.NewGuid().ToString() + ".xml";
                File.Move(postFile, newPostFile);
            }
            catch (Exception ex)
            {
                Utils.Log("BlogGenerator.CopyTemplateBlogFolder", ex);
                throw;  // re-throw error so error message bubbles up.
            }

            return true;
        }

        static void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            var cnt = 0;
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            cnt = content.Length;
            reader.Close();

            //content = Regex.Replace(content, searchText, replaceText);
            content = content.Replace(searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
    }
}
