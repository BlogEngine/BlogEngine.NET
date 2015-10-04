using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Custom field repository
    /// </summary>
    public class CustomFieldRepository : ICustomFieldRepository
    {
        #region Intreface

        /// <summary>
        /// List of items
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <returns>List of posts</returns>
        public IEnumerable<CustomField> Find(string filter)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            if (string.IsNullOrEmpty(filter))
                filter = "1 == 1";

            var items = new List<CustomField>();
            var query = CustomFieldsParser.CachedFields.AsQueryable().Where(filter);

            foreach (var item in query)
                items.Add(item);

            return items;
        }

        /// <summary>
        /// Single item
        /// </summary>
        /// <param name="type">Type (theme, post etc)</param>
        /// <param name="id">Id, for example "standard" for a theme</param>
        /// <param name="key">Key in the key/value for a field</param>
        /// <returns></returns>
        public CustomField FindById(string type, string id, string key)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            var cf = Find("").Where(f => f.CustomType == type && f.ObjectId == id && f.Key == key).FirstOrDefault();
            return cf;
        }

        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item">Custom field</param>
        /// <returns>Added field</returns>
        public CustomField Add(CustomField item)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();
            try
            {
                if (AlreadyExists(item))
                    throw new ApplicationException("Custom field already exists");

                BlogEngine.Core.Providers.BlogService.SaveCustomField(item);
                CustomFieldsParser.ClearCache();
                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error adding custom field", ex);
                throw;
            }
        }

        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Custom field</param>
        /// <returns>True on success</returns>
        public bool Update(CustomField item)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();
            try
            {
                item.BlogId = BlogEngine.Core.Blog.CurrentInstance.Id;

                BlogEngine.Core.Providers.BlogService.SaveCustomField(item);
                CustomFieldsParser.ClearCache();
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log("Error updaging custom field", ex);
                return false;
            }
        }

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="type">Type (theme, post etc)</param>
        /// <param name="id">Id, for example "standard" for a theme</param>
        /// <param name="key">Key in the key/value for a field</param>
        /// <returns>True on success</returns>
        public bool Remove(string type, string id, string key)
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();
            try
            {
                var item = new CustomField
                {
                    CustomType = type,
                    BlogId = Blog.CurrentInstance.BlogId,
                    ObjectId = id,
                    Key = key
                };

                Providers.BlogService.DeleteCustomField(item);
                CustomFieldsParser.ClearCache();
                return true;
            }
            catch (Exception ex)
            {
                Utils.Log("Error updaging custom field", ex);
                return false;
            }
        }

        /// <summary>
        /// Crear fields for object type (post, theme etc)
        /// </summary>
        /// <param name="type">Custom type</param>
        /// <param name="id">Object id</param>
        public void ClearCustomFields(string type, string id)
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();
            try
            {
                Providers.BlogService.ClearCustomFields(Blog.CurrentInstance.BlogId.ToString(), type, id);
                CustomFieldsParser.ClearCache();
            }
            catch (Exception ex)
            {
                Utils.Log("Error updaging custom field", ex);
            }
        }

        bool AlreadyExists(CustomField item)
        {
            var field = CustomFieldsParser.CachedFields.Where(f => f.BlogId == item.BlogId
                    && f.CustomType == item.CustomType
                    && f.ObjectId == item.ObjectId
                    && f.Key == item.Key).FirstOrDefault();
            return field != null;
        }

        #endregion
    }
}
