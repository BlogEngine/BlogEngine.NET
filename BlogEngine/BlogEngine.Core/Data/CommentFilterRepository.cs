using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Hosting;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Comment filter repository
    /// </summary>
    public class CommentFilterRepository : ICommentFilterRepository
    {
        /// <summary>
        /// The comment filters.
        /// </summary>
        static protected ExtensionSettings Filters;

        /// <summary>
        /// Comment filters list
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="filter"></param>
        /// <param name="order"></param>
        /// <returns>List of comment filters</returns>
        public IEnumerable<CommentFilterItem> Find(int take = 10, int skip = 0, string filter = "", string order = "")
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            var filterList = new List<CommentFilterItem>();
            try
            {
                Filters = ExtensionManager.GetSettings("MetaExtension", "BeCommentFilters");
                DataTable dt = Filters.GetDataTable();

                foreach (DataRow row in dt.Rows)
                {
                    var f = new CommentFilterItem
                    {
                        Id = Guid.Parse(row["Id"].ToString()),
                        Action = row["Action"].ToString(),
                        Subject = row["Subject"].ToString(),
                        Operation = row["Operator"].ToString(),
                        Filter = row["Filter"].ToString()
                    };
                    filterList.Add(f);
                }

                if (take == 0) take = filterList.Count;
                if (string.IsNullOrEmpty(filter)) filter = "1==1";
                if (string.IsNullOrEmpty(order)) order = "Filter";

                var items = new List<CommentFilterItem>();
                var query = filterList.AsQueryable().Where(filter);

                foreach (var item in query.OrderBy(order).Skip(skip).Take(take))
                    items.Add(item);

                return items;
            }
            catch (Exception ex)
            {
                Utils.Log("CommentFilterRepository.Find(): " + ex.Message);
            }
            return filterList;
        }

        /// <summary>
        /// Find by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Comment filter</returns>
        public CommentFilterItem FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>New item</returns>
        public CommentFilterItem Add(CommentFilterItem item)
        {
            if (!Security.IsAuthorizedTo(BlogEngine.Core.Rights.AccessAdminPages))
                throw new System.UnauthorizedAccessException();

            try
            {
                Filters = ExtensionManager.GetSettings("MetaExtension", "BeCommentFilters");
                string id = Guid.NewGuid().ToString();

                int parLen = Filters.Parameters[0].Values.Count;
                for (int i = 0; i < parLen; i++)
                {
                    bool exists =
                        Filters.Parameters[1].Values[i] == item.Action &&
                        Filters.Parameters[2].Values[i] == item.Subject &&
                        Filters.Parameters[3].Values[i] == item.Operation &&
                        Filters.Parameters[4].Values[i] == item.Filter;
                    if (exists)
                    {
                        throw new System.ApplicationException("Item already exists");
                    }
                }

                string[] f = new string[] { 
                    id, 
                    item.Action, 
                    item.Subject, 
                    item.Operation, 
                    item.Filter };

                Filters.AddValues(f);
                ExtensionManager.SaveSettings("MetaExtension", Filters);

                return new CommentFilterItem
                {
                    Id = Guid.Parse(id),
                    Action = item.Action,
                    Subject = item.Subject,
                    Operation = item.Operation,
                    Filter = item.Filter
                };
            }
            catch (Exception ex)
            {
                Utils.Log("Error adding new comment filter", ex);
                throw;
            }
        }

        /// <summary>
        /// Update filter
        /// </summary>
        /// <param name="item">Filter</param>
        /// <returns>True on success</returns>
        public bool Update(CommentFilterItem item)
        {
            return true;
        }

        /// <summary>
        /// Remove all filters
        /// </summary>
        /// <returns>True on success</returns>
        public bool RemoveAll()
        {
            try
            {
                for (int i = 0; i < Filters.Parameters.Count; i++)
                {
                    var p = Filters.Parameters[i];
                    for (int j = p.Values.Count; j-- > 0; )
                    {
                        p.DeleteValue(j);
                    }
                }
                ExtensionManager.SaveSettings("MetaExtension", Filters);
            }
            catch (Exception ex)
            {
                Utils.Log("Error removing all comment filters", ex);
                throw;
            }

            return true;
        }

        /// <summary>
        /// Remove filter by id
        /// </summary>
        /// <param name="id">Filter id</param>
        /// <returns>True on success</returns>
        public bool Remove(Guid id)
        {
            int idx = 0;
            foreach (ExtensionParameter par in Filters.Parameters)
            {
                if (par.Name.ToUpper() == "ID")
                {
                    int i = 0;
                    foreach (var v in par.Values)
                    {
                        if (Guid.Parse(v) == id)
                        {
                            idx = i;
                            break;
                        }
                        i++;
                    }
                    break;
                }
            }
            foreach (ExtensionParameter par in Filters.Parameters)
            {
                par.DeleteValue(idx);
            }

            ExtensionManager.SaveSettings("MetaExtension", Filters);
            return true;
        }
    }
}
