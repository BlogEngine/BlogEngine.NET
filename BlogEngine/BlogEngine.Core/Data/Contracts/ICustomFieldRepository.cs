using System.Collections.Generic;
using BlogEngine.Core.Data.Models;

namespace BlogEngine.Core.Data.Contracts
{
    /// <summary>
    /// Custom field repository
    /// </summary>
    public interface ICustomFieldRepository
    {
        /// <summary>
        /// List of items
        /// </summary>
        /// <param name="filter">Fileter expression</param>
        /// <returns>List of posts</returns>
        IEnumerable<CustomField> Find(string filter = "");
        /// <summary>
        /// Single item
        /// </summary>
        /// <param name="type">Type (theme, post etc)</param>
        /// <param name="id">Id, for example "standard" for a theme</param>
        /// <param name="key">Key in the key/value for a field</param>
        /// <returns></returns>
        CustomField FindById(string type, string id, string key);
        /// <summary>
        /// Add new item
        /// </summary>
        /// <param name="item">Custom field</param>
        /// <returns>Added field</returns>
        CustomField Add(CustomField item);
        /// <summary>
        /// Update item
        /// </summary>
        /// <param name="item">Custom field</param>
        /// <returns>True on success</returns>
        bool Update(CustomField item);
        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="type">Type (theme, post etc)</param>
        /// <param name="id">Id, for example "standard" for a theme</param>
        /// <param name="key">Key in the key/value for a field</param>
        /// <returns>True on success</returns>
        bool Remove(string type, string id, string key);
        /// <summary>
        /// Crear fields for object type (post, theme etc)
        /// </summary>
        /// <param name="type">Custom type</param>
        /// <param name="id">Object id</param>
        void ClearCustomFields(string type, string id);
    }
}
