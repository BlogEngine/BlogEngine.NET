using BlogEngine.Core.Data.Contracts;
using BlogEngine.Core.Data.Models;
using BlogEngine.Core.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Data;

namespace BlogEngine.Core.Data
{
    /// <summary>
    /// Custom filter repo
    /// </summary>
    public class CustomFilterRepository : ICustomFilterRepository
    {
        /// <summary>
        /// The custom filters.
        /// </summary>
        static protected ExtensionSettings CustomFilters;

        /// <summary>
        /// Get list of custom filters
        /// </summary>
        /// <returns>List of filters</returns>
        public IEnumerable<CustomFilter> GetCustomFilters()
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();

            var filterList = new List<CustomFilter>();
            try
            {
                CustomFilters = ExtensionManager.GetSettings("MetaExtension", "BeCustomFilters");
                DataTable dt = CustomFilters.GetDataTable();

                foreach (DataRow row in dt.Rows)
                {
                    var f = new CustomFilter
                    {
                        Name = row["Name"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Checked = int.Parse(row["Checked"].ToString()),
                        Spam = int.Parse(row["Cought"].ToString()),
                        Mistakes = int.Parse(row["Reported"].ToString())
                    };

                    var ext = ExtensionManager.GetExtension(f.Name);
                    f.Enabled = ext == null ? true : ext.Enabled;

                    filterList.Add(f);
                }
            }
            catch (Exception ex)
            {
                Utils.Log("CustomFilterRepository.GetCustomFilters(): " + ex.Message);
            }
            return filterList;
        }

        /// <summary>
        /// Reset counters for custom filter
        /// </summary>
        /// <param name="filterName">Filter name</param>
        /// <returns>Json response</returns>
        public JsonResponse ResetCounters(string filterName)
        {
            if (!Security.IsAuthorizedTo(Rights.AccessAdminPages))
                throw new UnauthorizedAccessException();
            try
            {
                if (!string.IsNullOrEmpty(filterName))
                {
                    // reset statistics for this filter
                    for (int i = 0; i < CustomFilters.Parameters[0].Values.Count; i++)
                    {
                        if (CustomFilters.Parameters[1].Values[i] == filterName)
                        {
                            CustomFilters.Parameters[2].Values[i] = "0";
                            CustomFilters.Parameters[3].Values[i] = "0";
                            CustomFilters.Parameters[4].Values[i] = "0";
                        }
                    }
                    ExtensionManager.SaveSettings("MetaExtension", CustomFilters);
                }
                return new JsonResponse() { Success = true, Message = string.Format("Counters for {0} reset", filterName) };
            }
            catch (Exception ex)
            {
                Utils.Log(string.Format("CustomFilterRepository.ResetCounters: {0}", ex.Message));
                return new JsonResponse() { Message = "Error resetting counters" };
            }
        }
    }
}
