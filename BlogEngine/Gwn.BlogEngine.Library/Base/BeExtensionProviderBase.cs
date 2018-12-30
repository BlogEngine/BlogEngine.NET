using System;
using System.Collections.Generic;
using BlogEngine.Core;
using BlogEngine.Core.Web.Extensions;
using Gwn.BlogEngine.Library.Entities;
using Gwn.BlogEngine.Library.Events;
using Gwn.BlogEngine.Library.Extensions;
using Gwn.BlogEngine.Library.Interfaces;
using Gwn.BlogEngine.Library.Logger;

namespace Gwn.BlogEngine.Library.Base
{
    public class BeExtensionProviderBase : IBeProvider
    {
        #region Fields and Constants

        public const string LeaveAloneTemplate = "[{0}]";  // Template to display as is

        private static IBeLogger _logger;
        private static string _providerName;
        private static List<BeSettingRecord> _defaultSettings;
        private static readonly object SyncRoot = new object(); // Sync root
        private static readonly Dictionary<Guid, ExtensionSettings> 
            BlogsSettings = new Dictionary<Guid, ExtensionSettings>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public static IBeLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new DebugLogger();
                return _logger;
            }
            set { _logger = value; }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        protected static string ProviderName
        {
            get
            {
                if (_providerName == null)
                    throw new NotImplementedException("You must configure ProviderName in derived class (in static constructor)");
                return _providerName;
            }
            set { _providerName = value; }
        }

        /// <summary>
        /// Gets or sets the default settings.
        /// </summary>
        /// <value>The default settings.</value>
        protected static List<BeSettingRecord> DefaultSettings
        {
            get
            {
                if(_defaultSettings==null)
                    throw new NotImplementedException("You must configure DefaultSettings in derived class (in static constructor)");
                return _defaultSettings;
            }
            set { _defaultSettings = value; }
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        protected static ExtensionSettings Settings
        {
            get
            {
                var blogId = Blog.CurrentInstance.Id;

                if (!BlogsSettings.ContainsKey(blogId))
                {
                    lock (SyncRoot)
                    {
                        if (!BlogsSettings.ContainsKey(blogId))
                        {
                            // Create settings object using extension class name (required) 
                            var extensionSettings = new ExtensionSettings(ProviderName);

                            // Use extension method to initialize extension settings
                            // using the provided default settings
                            BlogsSettings[blogId] = extensionSettings.Initialize(DefaultSettings); 
                        }
                    }
                }
                
                // Unit test will use the serialized data from InitializationDataForSettings()
                // which is the default data above
                var settings = BlogsSettings[blogId] 
                    ?? InitializationDataForSettings<ExtensionSettings>();

                return settings;
            }
        }

        #endregion 

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GwnWikiProvider"/> class.
        /// </summary>
        public BeExtensionProviderBase()
        {
            // Force Settings to be initialized during construction
            var initSettings = Settings;
        }

        #endregion 

        /// <summary>
        /// Inserts the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        public void Insert(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.Insert()", GetType().Name);
            HandleInsert(sender, e);
        }


        /// <summary>
        /// Updates the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        public void Update(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.Update()", GetType().Name);
            HandleUpdate(sender, e);
        }


        /// <summary>
        /// Deletes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        public void Delete(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.Delete()", GetType().Name);
            HandleDelete(sender, e);
        }


        /// <summary>
        /// Saveds the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        public void Saved(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.Saved()", GetType().Name);
            HandleSaved(sender, e);
        }

        /// <summary>
        /// Used for Unit Test purposes if not running from BlogEngine web
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T InitializationDataForSettings<T>()
        {
            Logger.Log("{0}: BeExtensionProviderBase.InitializeDataForSetting","BeExtensionProviderBase");

            #region xmlData (serialized ExtensionSettings) 
            const string xmlData = @"<?xml version='1.0' encoding='utf-8'?>
                <ExtensionSettings xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                  <Delimiter>44</Delimiter>
                  <Help />
                  <Hidden>false</Hidden>
                  <Index>0</Index>
                  <IsScalar>false</IsScalar>
                  <KeyField>CommandParameter</KeyField>
                  <Name>GwnWikiExtension</Name>
                  <Parameters>
                    <KeyField>false</KeyField>
                    <Label>Id:</Label>
                    <MaxLength>100</MaxLength>
                    <Name>Id</Name>
                    <ParamType>String</ParamType>
                    <Required>false</Required>
                    <SelectedValue />
                    <Values>00000000-0000-0000-0000-000000000000</Values>
                    <Values>00000000-0000-0000-0000-000000000000</Values>
                    <Values>5d0d44a6-39be-4846-bfbd-d5e87c97860e</Values>
                    <Values>d397c6d3-e327-4bd1-bc35-8bce5ab4e7f3</Values>
                    <Values>d0367f2c-7acb-49d6-b815-968f84da539c</Values>
                  </Parameters>
                  <Parameters>
                    <KeyField>false</KeyField>
                    <Label>PermaLink:</Label>
                    <MaxLength>100</MaxLength>
                    <Name>PermaLink</Name>
                    <ParamType>String</ParamType>
                    <Required>false</Required>
                    <SelectedValue />
                    <Values>Not Assigned</Values>
                    <Values>Not Assigned</Values>
                    <Values>http://localhost:3202/BlogEngine/post.aspx?id=5d0d44a6-39be-4846-bfbd-d5e87c97860e</Values>
                    <Values>http://localhost:3202/BlogEngine/post.aspx?id=d397c6d3-e327-4bd1-bc35-8bce5ab4e7f3</Values>
                    <Values>http://localhost:3202/BlogEngine/post.aspx?id=d0367f2c-7acb-49d6-b815-968f84da539c</Values>
                  </Parameters>
                  <Parameters>
                    <KeyField>false</KeyField>
                    <Label>Command:</Label>
                    <MaxLength>200</MaxLength>
                    <Name>Command</Name>
                    <ParamType>String</ParamType>
                    <Required>true</Required>
                    <SelectedValue />
                    <Values>Page</Values>
                    <Values>Display</Values>
                    <Values>Page</Values>
                    <Values>Page</Values>
                    <Values>Page</Values>
                  </Parameters>
                  <Parameters>
                    <KeyField>true</KeyField>
                    <Label>Command Parameter:</Label>
                    <MaxLength>200</MaxLength>
                    <Name>CommandParameter</Name>
                    <ParamType>String</ParamType>
                    <Required>true</Required>
                    <SelectedValue />
                    <Values>PageDefault</Values>
                    <Values>DisplayDefault</Values>
                    <Values>PortableClassLibrary</Values>
                    <Values>PasswordManager</Values>
                    <Values>MultiTargeting</Values>
                  </Parameters>
                  <Parameters>
                    <KeyField>false</KeyField>
                    <Label>Display Template:</Label>
                    <MaxLength>100</MaxLength>
                    <Name>DisplayTemplate</Name>
                    <ParamType>String</ParamType>
                    <Required>false</Required>
                    <SelectedValue />
                    <Values>&lt;b&gt;&lt;i&gt;{0}&lt;/i&gt;&lt;/b&gt;</Values>
                    <Values>&lt;a target='_blank' href='[PermaLink]'&gt;{0}&lt;/a&gt;</Values>
                    <Values>PageDefault</Values>
                    <Values>PageDefault</Values>
                    <Values>PageDefault</Values>
                  </Parameters>
                  <ShowAdd>true</ShowAdd>
                  <ShowDelete>true</ShowDelete>
                  <ShowEdit>true</ShowEdit>
                </ExtensionSettings>";
            #endregion 

            if (typeof(T).Name.ToLower() == "String")
                return (T)(object)xmlData;

            if (typeof(T).Name == "ExtensionSettings")
                return (T) (object) xmlData.DeserializeObject<ExtensionSettings>();

            return default(T);
        }



        /// <summary>
        /// Handles the insert.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleInsert(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.HandleInsert()", GetType().Name);
            ExtensionManager.Save();
        }

        /// <summary>
        /// Handles the delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleDelete(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.HandleDelete()", GetType().Name);
            ExtensionManager.Save();
        }

        /// <summary>
        /// Handles the update.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleUpdate(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.HandleUpdate()", GetType().Name);
            ExtensionManager.Save();
        }

        /// <summary>
        /// Handles the saved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Gwn.BlogEngine.Library.Events.BeEventArgs"/> instance containing the event data.</param>
        protected virtual void HandleSaved(object sender, BeEventArgs e)
        {
            Logger.Log("{0}: BeExtensionProviderBase.HandleSaved()", GetType().Name);
            ExtensionManager.Save();
        }



    }
}
