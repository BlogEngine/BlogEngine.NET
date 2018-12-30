using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using BlogEngine.Wiki.Test.Constants;
using BlogEngine.Wiki.Test.Mocks;
using Gwn.BlogEngine.Library.Extensions;

namespace BlogEngine.Wiki.Test.Helper
{
    public class MockBlogHelper
    {
        /// <summary>
        /// Displays the content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void DisplayContent(string content)
        {
            var filePath = UnitTestHelper.GetTestOutputPath(UnitTestConstants.BlogOutputPath);

            // Create the file
            content.WriteFile(filePath);

            // Launch the file 
            Process.Start(filePath);
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <returns></returns>
        public string GetContent(string name)
        {
            return MockPost.GetPost(name);
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <returns></returns>
        public Guid GetId()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Makes the blog list so that the BlogEngine environment won't complain
        /// (not currently used)
        /// </summary>
        public void MakeBlogList()
        {
            const string content = @"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
                <blogs>
                    <item id='96d5b379-7e1d-4dac-a6ba-1e50db561b04' name='Primary' hostName='' isAnyTextBeforeHostnameAccepted='False' storageContainerName='' virtualPath='~/' isPrimary='True' isActive='True' />
                    <item id='7b30c686-a3d6-4f45-9f0e-e3ac101dee97' name='Template' hostName='' isAnyTextBeforeHostnameAccepted='True' storageContainerName='template' virtualPath='~/template' isPrimary='False' isActive='False' />
                </blogs>";
            var xml = XElement.Parse(content);
            xml.Save(Directory.GetCurrentDirectory() + "\\" + BlogEngineConstants.BlogListFileName);
        }
    }
}
