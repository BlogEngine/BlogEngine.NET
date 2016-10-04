using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Page repository
    /// </summary>
    public class PageRepository : IPageRepository
    {
        /// <summary>
        /// Find page item
        /// </summary>
        /// <param name="take">Items per page</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter expression</param>
        /// <param name="order">Sort order</param>
        /// <returns></returns>
        public IEnumerable<PageItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPages))
                throw new UnauthorizedAccessException();

            if (string.IsNullOrEmpty(filter)) filter = "1==1";
            if (string.IsNullOrEmpty(order)) order = "DateCreated desc";

            var items = new List<PageItem>();
            var query = Page.Pages.AsQueryable().Where(filter);

            // 0 for all
            if (take == 0) take = Page.Pages.Count;

            foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                items.Add(Json.GetPage(item));

            return items;
        }

        /// <summary>
        /// Get single page
        /// </summary>
        /// <param name="id">Page id</param>
        /// <returns>Page object</returns>
        public PageDetail FindById(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPages))
                throw new UnauthorizedAccessException();
            try
            {
                return Json.GetPageDetail((from p in Page.Pages.ToList() where p.Id == id select p).FirstOrDefault());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Add new page
        /// </summary>
        /// <param name="detail">Page</param>
        /// <returns>Saved page with new ID</returns>
        public PageDetail Add(PageDetail detail)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewPages))
                throw new UnauthorizedAccessException();

            var page = new Page();
            if (Save(page, detail))
                return Json.GetPageDetail(page);

            return null;
        }

        /// <summary>
        /// Update page
        /// </summary>
        /// <param name="page">Page to update</param>
        /// <param name="action">Action to execute</param>
        /// <returns>True on success</returns>
        public bool Update(PageDetail page, string action)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewPages))
                throw new UnauthorizedAccessException();

            var corePage = (from p in Page.Pages.ToList() where p.Id == page.Id select p).FirstOrDefault();

            if (action == "publish")
            {
                corePage.IsPublished = true;
                corePage.Save();
                return true;
            }
            if (action == "unpublish")
            {
                corePage.IsPublished = false;
                corePage.Save();
                return true;
            }

            if (corePage != null && Save(corePage, page))
                return true;

            return false;
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewPages))
                throw new UnauthorizedAccessException();

            var page = (from p in Page.Pages.ToList() where p.Id == id select p).FirstOrDefault();

            if (page.HasChildPages)
                throw new ApplicationException("Can not delete; page has child pages");

            page.Delete();
            page.Save();
            return true;
        }

        #region private methods

        static bool Save(Page page, PageDetail detail)
        {
            page.Title = detail.Title;
            page.DateCreated = DateTime.ParseExact(detail.DateCreated, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            page.IsPublished = detail.IsPublished;
            page.ShowInList = detail.ShowInList;
            page.IsDeleted = detail.IsDeleted;
            page.Content = detail.Content;
            page.Description = GetDescription(detail.Description, detail.Content);
            page.Keywords = detail.Keywords;
            page.IsFrontPage = detail.IsFrontPage;
            page.SortOrder = detail.SortOrder;

            // if changing slug, should be unique
            if (page.Slug != detail.Slug)
                page.Slug = GetUniqueSlug(detail.Slug);

            if (detail.Parent.OptionValue == "none")
            {
                page.Parent = Guid.Empty;
            }
            else
            {
                if (detail.Parent != null && detail.Parent.OptionValue != null)
                {
                    try
                    {
                        page.Parent = Guid.Parse(detail.Parent.OptionValue);
                    }
                    catch (Exception)
                    {
                        Utils.Log("Error parsing parent ID while saving page");
                    }
                }
            }

            page.Save();
            return true;
        }

        static string GetUniqueSlug(string slug)
        {
            string s = Utils.RemoveIllegalCharacters(slug.Trim());

            // will do for up to 100 unique post titles
            for (int i = 1; i < 101; i++)
            {
                if (IsUniqueSlug(s))
                    break;

                s = $"{slug}{i}";
            }
            return s;
        }

        private static bool IsUniqueSlug(string slug)
        {
            return Page.Pages
                .FirstOrDefault(p => p.Slug != null && p.Slug.ToLower() == slug.ToLower()) == null ? true : false;
        }

        // if description not set, use first 100 chars in the post
        static string GetDescription(string desc, string content)
        {
            if (string.IsNullOrEmpty(desc))
            {
                var p = Utils.StripHtml(content);

                if (p.Length > 100)
                    return p.Substring(0, 100);

                return p;
            }
            return desc;
        }

        #endregion
    }
}
