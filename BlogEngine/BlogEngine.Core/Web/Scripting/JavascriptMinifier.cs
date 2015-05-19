using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace BlogEngine.Core.Web.Scripting
{
    /// <summary>
    /// Helper class for performing minification of Javascript and CSS.
    /// </summary>
    /// <remarks>
    /// 
    /// This class is basically a wrapper for the AjaxMin library(lib/AjaxMin.dll).
    /// http://ajaxmin.codeplex.com/
    /// 
    /// There are no symbols that come with the AjaxMin dll, so this class gives a bit of intellisense 
    /// help for basic control. AjaxMin is a pretty dense library with lots of different settings, so
    /// everyone's encouraged to use it directly if they want to.
    /// 
    /// </remarks>
    public sealed class JavascriptMinifier
    {
        private Microsoft.Ajax.Utilities.Minifier ajaxMinifier = new Microsoft.Ajax.Utilities.Minifier();

        /// <summary>
        /// Creates a new Minifier instance.
        /// </summary>
        public JavascriptMinifier()
        {
            this.RemoveWhitespace = true;
            this.PreserveFunctionNames = true;
            this.VariableMinification = Core.Web.Scripting.VariableMinification.None;
        }

        #region "Methods"

        /// <summary>
        /// Builds the required CodeSettings class needed for the Ajax Minifier.
        /// </summary>
        /// <returns></returns>
        private CodeSettings CreateCodeSettings()
        {
            var codeSettings = new CodeSettings();
            codeSettings.MinifyCode = false;
            codeSettings.OutputMode = (this.RemoveWhitespace ? OutputMode.SingleLine : OutputMode.MultipleLines);

            // MinifyCode needs to be set to true in order for anything besides whitespace removal
            // to be done on a script.
            codeSettings.MinifyCode = this.ShouldMinifyCode;
            if (this.ShouldMinifyCode)
            {
                switch (this.VariableMinification)
                {
                    case Core.Web.Scripting.VariableMinification.None:
                        codeSettings.LocalRenaming = LocalRenaming.KeepAll;
                        break;

                    case Core.Web.Scripting.VariableMinification.LocalVariablesOnly:
                        codeSettings.LocalRenaming = LocalRenaming.KeepLocalizationVars;
                        break;

                    case Core.Web.Scripting.VariableMinification.LocalVariablesAndFunctionArguments:
                        codeSettings.LocalRenaming = LocalRenaming.CrunchAll;
                        break;
                }
                // This is being set by default. A lot of scripts use eval to parse out various functions
                // and objects. These names need to be kept consistant with the actual arguments.
                codeSettings.EvalTreatment = EvalTreatment.MakeAllSafe;


                // This makes sure that function names on objects are kept exactly as they are. This is
                // so functions that other non-minified scripts rely on do not get renamed.
                codeSettings.PreserveFunctionNames = this.PreserveFunctionNames;
            }
            return codeSettings;
        }

        /// <summary>
        /// Gets the minified version of the passed in script.
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public string Minify(string script)
        {
            if (this.ShouldMinify)
            {
                if (String.IsNullOrEmpty(script))
                {
                    return string.Empty;
                }
                else
                {
                    return this.ajaxMinifier.MinifyJavaScript(script, this.CreateCodeSettings());
                }
            }

            return script;
        }

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets whether this Minifier instance should minify local-scoped variables.
        /// </summary>
        /// <remarks>
        /// 
        /// Setting this value to LocalVariablesAndFunctionArguments can have a negative impact on some scripts.
        /// Ex: A pre-minified jQuery will fail if passed through this. 
        /// 
        /// </remarks>
        public VariableMinification VariableMinification { get; set; }

        /// <summary>
        /// Gets or sets whether this Minifier instance should preserve function names when minifying a script.
        /// </summary>
        /// <remarks>
        /// 
        /// Scripts that have external scripts relying on their functions should leave this set to true. 
        /// 
        /// </remarks>
        public bool PreserveFunctionNames { get; set; }

        /// <summary>
        /// Gets or sets whether the <see cref="BlogEngine.Core.Web.Scripting.JavascriptMinifier"/> instance should remove
        /// whitespace from a script.
        /// </summary>
        public bool RemoveWhitespace { get; set; }

        private bool ShouldMinifyCode
        {
            get
            {
                //  return true;
                return ((!PreserveFunctionNames) || (this.VariableMinification != Core.Web.Scripting.VariableMinification.None));
            }
        }

        private bool ShouldMinify
        {
            get
            {
                return ((this.RemoveWhitespace) || (this.ShouldMinifyCode));
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the way variables should be minified by a Minifier instance.
    /// </summary>
    public enum VariableMinification
    {
        /// <summary>
        /// No minification will take place.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only variables that are local in scope to a function will be minified.
        /// </summary>
        LocalVariablesOnly = 1,

        /// <summary>
        /// Local scope variables will be minified, as will function parameter names. This can have a negative impact on some scripts, so test if you use it! 
        /// </summary>
        LocalVariablesAndFunctionArguments = 2

    }
}
