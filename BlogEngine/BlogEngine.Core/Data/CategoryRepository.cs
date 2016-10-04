using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Category repository
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        /// <summary>
        /// Find items in collection
        /// </summary>
        /// <param name="take">Items per page, default 10, 0 to return all</param>
        /// <param name="skip">Items to skip</param>
        /// <param name="filter">Filter, for example filter=IsPublished,true,Author,Admin</param>
        /// <param name="order">Sort order, for example order=DateCreated,desc</param>
        /// <returns>List of items</returns>
        public IEnumerable<CategoryItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
                throw new UnauthorizedAccessException();

            // get post categories with counts
            var items = new List<CategoryItem>();
            foreach (var p in Post.ApplicablePosts)
            {
                foreach (var c in p.Categories)
                {
                    var tmp = items.FirstOrDefault(cat => cat.Id == c.Id);
                    if (tmp == null)
                        items.Add(new CategoryItem { Id = c.Id, Parent = OptionById(c.Parent), Title = c.Title, Description = c.Description, Count = 1 });
                    else
                        tmp.Count++;
                }
            }

            // add categories without posts
            foreach (var c in Category.Categories)
            {
                var x = items.Where(i => i.Id == c.Id).FirstOrDefault();
                if (x == null)
                    items.Add(new CategoryItem { Id = c.Id, Parent = OptionById(c.Parent), Title = c.Title, Description = c.Description, Count = 0 });
            }

            // return only what requested
            var query = items.AsQueryable();

            if(!string.IsNullOrEmpty(filter))
                query = items.AsQueryable().Where(filter);

            if (take == 0) 
                take = items.Count;

            if (string.IsNullOrEmpty(order))
                order = "Title";

            return query.OrderBy(order).Skip(skip).Take(take);
        }
        /// <summary>
        /// Get single item
        /// </summary>
        /// <param name="id">Item id</param>
        /// <returns>Object</returns>
        public CategoryItem FindById(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.ViewPublicPosts))
                throw new UnauthorizedAccessException();

            // get post categories
            var items = new List<CategoryItem>();
            foreach (var p in Post.ApplicablePosts)
            {
                foreach (var c in p.Categories)
                {
                    var tmp = items.FirstOrDefault(cat => cat.Id == c.Id);
                    if (tmp == null)
                        items.Add(new CategoryItem { Id = c.Id, Parent = OptionById(c.Parent), Title = c.Title, Description = c.Description, Count = 1 });
                    else
                        tmp.Count++;
                }
            }
            // add categories without posts
            foreach (var c in Category.Categories)
            {
                var x = items.Where(i => i.Id == c.Id).FirstOrDefault();
                if (x == null)
                    items.Add(new CategoryItem { Id = c.Id, Parent = OptionById(c.Parent), Title = c.Title, Description = c.Description, Count = 0 });
            }
            return items.Where(c => c.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item">Post</param>
        /// <returns>Saved item with new ID</returns>
        public CategoryItem Add(CategoryItem item)
        {
            if (!Security.IsAuthorizedTo(Rights.CreateNewPosts))
                throw new UnauthorizedAccessException();

            var cat = (from c in Category.Categories.ToList() where c.Title == item.Title select c).FirstOrDefault();
            if (cat != null)
                throw new ApplicationException("Title must be unique");

            try
            {
                var newItem = new Category();
                UpdateCoreFromJson(newItem, item);
                return GetJsonFromCore(newItem);
            }
            catch (Exception ex)
            {
                Utils.Log($"CategoryRepository.Add: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Item to update</param>
        /// <returns>True on success</returns>
        public bool Update(CategoryItem item)
        {
            if (!Security.IsAuthorizedTo(Rights.EditOwnPosts))
                throw new UnauthorizedAccessException();

            var cat = (from c in Category.Categories.ToList() where c.Title == item.Title && c.Id != item.Id select c).FirstOrDefault();
            if (cat != null)
                throw new ApplicationException("Title must be unique");

            var core = (from c in Category.Categories.ToList() where c.Id == item.Id select c).FirstOrDefault();

            if (core != null)
            {
                UpdateCoreFromJson(core, item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            if (!Security.IsAuthorizedTo(Rights.DeleteOwnPosts))
                throw new UnauthorizedAccessException();
            try
            {
                var core = (from c in Category.Categories.ToList() where c.Id == id select c).FirstOrDefault();
                if(core != null)
                {
                    foreach (Post post in Post.Posts)
                    {
                        if (post.Categories.Contains(core))
                        {
                            post.Categories.Remove(core);
                        }
                    }
                    core.Delete();
                    core.Save();  
                 
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Utils.Log($"CategoryRepository.Remove: {ex.Message}");
                return false;
            }
        }

        #region Private methods

        static Data.Models.CategoryItem GetJsonFromCore(Category core)
        {
            return new Data.Models.CategoryItem
            {
                Id = core.Id,
                Title = core.Title,
                Description = core.Description,
                Parent = OptionById(core.Parent)
            };
        }

        static void UpdateCoreFromJson(Category core, Data.Models.CategoryItem json)
        {
            try
            {
                core.Title = json.Title;
                core.Description = json.Description;
                //core.Parent = json.Parent == null || string.IsNullOrEmpty(json.Parent.OptionValue) ? Guid.Empty : Guid.Parse(json.Parent.OptionValue);

                if (json.Parent == null || string.IsNullOrEmpty(json.Parent.OptionValue))
                {
                    core.Parent = null;
                }
                else
                {
                    var pId = Guid.Parse(json.Parent.OptionValue);

                    // when A parent B, B can't be parent A 
                    var p = ParentId(pId);

                    if (pId.ToString() != p)
                    {
                        // A cant't be parent of A
                        if (core.Id != pId)
                            core.Parent = pId;
                    }
                }               
                core.Save();
            }
            catch (Exception ex)
            {
                Utils.Log("Error updating category: ", ex);
            }
        }

        static SelectOption OptionById(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return null;

            var cat = (from c in Category.Categories.ToList() where c.Id == id select c).FirstOrDefault();
            return new SelectOption { IsSelected = false, OptionName = cat.Title, OptionValue = cat.Id.ToString() };
        }

        static string ParentId(Guid? id)
        {
            if (id == null || id == Guid.Empty)
                return string.Empty;

            var cat = (from c in Category.Categories.ToList() where c.Id == id select c).FirstOrDefault();
            return cat == null || cat.Parent == null || cat.Parent == Guid.Empty ? "" : cat.Parent.ToString();
        }

        #endregion
    }
}
