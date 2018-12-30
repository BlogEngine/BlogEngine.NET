using System.Collections.Generic;
using System.Linq;
using Gwn.BlogEngine.Library.Entities;

namespace Gwn.BlogEngine.Library.Extensions
{
    public static class BeRecordExtension
    {
        /// <summary>
        /// Finds the command.
        /// </summary>
        /// <param name="recordList">The record list.</param>
        /// <param name="commandToFind">The command to find.</param>
        /// <returns></returns>
        public static BeSettingRecord FindCommand(this List<BeSettingRecord> recordList, string commandToFind)
        {
            return recordList.FirstOrDefault(p => p.Command.ToLower() == commandToFind.ToLower());
        }

        /// <summary>
        /// Finds the command parameter.
        /// </summary>
        /// <param name="recordList">The record list.</param>
        /// <param name="commandParameterToFind">The command parameter to find.</param>
        /// <returns></returns>
        public static BeSettingRecord FindCommandParameter(this List<BeSettingRecord> recordList, string commandParameterToFind)
        {
            return recordList.FirstOrDefault(p => p.CommandParameter.ToLower() == commandParameterToFind.ToLower());
        }
    }
}
