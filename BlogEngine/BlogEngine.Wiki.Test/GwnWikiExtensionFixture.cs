using System.Collections.Generic;
using System.Web;
using BlogEngine.Core;
using BlogEngine.Wiki.Test.Constants;
using BlogEngine.Wiki.Test.Helper;
using BlogEngine.Wiki.Test.Mocks;
using Gwn.BlogEngine.Library.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogEngine.Wiki.Test
{
    [TestClass]
    public class GwnWikiExtensionFixture
    {
        //:::[ See Artifacts\RequiredUpdatesToUseUnitTest.jpg for BlogEngine updates ]::://

        private readonly MockBlogHelper _mockBlog = new MockBlogHelper();

        [TestMethod]
        public void CanLoadExtension()
        {
            var extension = new GwnWikiExtension();

            // Create BlogList.xml so that infrastructure won't complain
            _mockBlog.MakeBlogList();

            var post = new MockPost // Initialize with minimum requirements
                           {
                               Content = _mockBlog.GetContent("PCL"), 
                               Id = _mockBlog.GetId()
                           };
            
            var content = "";
            extension.ExtensionEvent +=
                (s, e) =>
                    {
                        if (e.ProcessType == BeProcessType.Serving)
                            content = e.Body;
                    };

            extension.Post_Saving(post, // Send in mock content
                new SavedEventArgs(SaveAction.Update));

            extension.Post_Serving(post, // Send in mock content
                new ServingEventArgs(post.Content, ServingLocation.PostList));

            // Saves results to file and displays it
            _mockBlog.DisplayContent(content);

        }


        [TestInitialize]
        public void BlogEngineEnvironmentSetUp()
        {
            // Provides a mock HttpContext
            UnitTestHelper.SetHttpContextWithSimulatedRequest(
                UnitTestConstants.MockHost, 
                UnitTestConstants.MockApplication);

            // Setup BlogEngine environment
            HttpContext.Current
                .Items[BlogEngineConstants.CONTEXT_ITEM_KEY] = new List<Blog> { new Blog()};

            HttpContext.Current.Items[UnitTestConstants.DataPath] = 
                UnitTestHelper.GetBlogEnginePath("App_Data");

            HttpContext.Current.Items[UnitTestConstants.VirtualPath] = "";
        }

    }

}
