using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using BlogEngine.Core.Web.Extensions;
using Gwn.BlogEngine.Library.Entities;

namespace Gwn.BlogEngine.Library.Extensions
{
    public static class BeStringExtension
    {
        /// <summary>
        /// Converts to the type of the parameter.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns></returns>
        public static ParameterType ToParameterType(this string stringToConvert)
        {
            try
            {
                if (stringToConvert == null)
                    return ParameterType.String;

                var result = Enum.Parse(typeof (ParameterType), stringToConvert);
                return (ParameterType) result;
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                return ParameterType.String;
            }
        }

        /// <summary>
        /// Converts to int32 - defaults to 100 characters.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns></returns>
        public static int ToInt32(this string stringToConvert)
        {
            try
            {
                if (string.IsNullOrEmpty(stringToConvert))
                    return 100;

                return Int32.Parse(stringToConvert.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                return 100;
            }
        }

        /// <summary>
        /// Converts to bool.
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns></returns>
        public static bool ToBool(this string stringToConvert)
        {
            try
            {
                if (string.IsNullOrEmpty(stringToConvert))
                    return false;

                return bool.Parse(stringToConvert.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                return false;
            }

        }

        /// <summary>
        /// Splits string on camel case.
        /// </summary>
        /// <param name="stringToSplit">The string to split.</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string stringToSplit)
        {
            var result = string // string.Join will delimit results with " "
                .Join(" ", Regex.Split(stringToSplit, @"(?=[A-Z])"));
            return result.TrimStart(); // Trim off any leading spaces
        }

        /// <summary>
        /// Gets the wiki commands - placed in library for reference 
        /// (possible reuse of concept)  Related blog follows:
        /// http://www.global-webnet.com/Blog/post/2011/09/10/Using-RegEx-and-LinqPad-to-parse-Wiki-style-commands-from-content.aspx
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static List<BeSettingRecord> GetWikiCommands(this string content)
        {
            var returnList = new List<BeSettingRecord>();
            foreach (var r in Regex.Matches(content.Replace("]", "]\r\n"), @"\[.*]"))
            {
                var match = r.Cast<Match>().Value.Replace("]", "").Replace("[", "");
                var display = match.Split('|'); 		// Did we provide DisplayTemplate?
                var isDisplaySplit = display.Length > 1; 	// Set flag for use below
                var command = match.Split(':');			// Commands have colons
                // If we have a command (has a colon), i.e., [Page:Portable Class Library|PortableClassLibrary]
                if (command.Length > 1)
                {
                    display = command[1].Split('|'); 		// Did we provide DisplayTemplate?
                    isDisplaySplit = display.Length > 1; 		// Set flag for use below
                    var displayTemplate = isDisplaySplit
                        ? display[0]  						// If display value provided then we use second element for parameter
                        : display[0].SplitCamelCase(); 		// otherwise use provided command as CommandParameter
                    var commandParameter = isDisplaySplit
                        ? display[1]  // If display value provided then we use second element for parameter
                        : display[0]; // otherwise use provided command as CommandParameter
                    returnList.Add(new BeSettingRecord
                    { // Create Wiki record
                        Command = command[0], 	      		  // First element is always Command
                        CommandParameter = commandParameter,  // CommandParameter
                        DisplayTemplate = displayTemplate,	  // Display value
                        Original = match
                    });
                    continue;	// we're done - continue next match
                }
                returnList.Add(new BeSettingRecord
                {	   // Create wiki record
                    Command = "Display",
                    CommandParameter = isDisplaySplit
                        ? display[1]  							// If a display value provided - use it
                        : display[0], 							// otherwise use Command as CommandParameter
                    DisplayTemplate = isDisplaySplit
                        ? display[0]  							// If display value provided = use it
                        : display[0].SplitCamelCase(),			// otherwise split on case
                    Original = match
                });
            }
            return returnList;
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filePath">The file path.</param>
        public static void WriteFile(this string content, string filePath)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            using (TextWriter tw = new StreamWriter(filePath))
                tw.WriteLine(content);
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public static string ReadFile(this string filePath)
        {
            using (TextReader tr = new StreamReader(filePath))
                return tr.ReadToEnd();
        }

    }
}
