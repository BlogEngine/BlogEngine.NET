using BlogEngine.Core;

namespace BlogEngine.Wiki.Test.Mocks
{
    public class MockPost : Post
    {
        public static string GetPost(string postName)
        {
            var postToReturn="[NotAssigned]";

            switch(postName)
            {
                case "PCL":
                    postToReturn =
                        @"<p>[Page:PortableClassLibrary]</p>
                        <p>This is a test [Multi-Targeted|MultiTargeting] to see [how it will] work and here is another page [PasswordManager] and its value for [MultiTargeting] again</p>
                        <p>&nbsp;</p>";
                    break;
                case "BitWise": postToReturn =
                    @"<h2>[Name:BitWise]</h2>
                    Uses the [PortableClassLibrary] to share code on all platforms<br>
                    This is a test [Multi-Targeted|MultiTargeted] to see how it will<br> 
		            work and here is another page [PasswordManager] and its value<br>";
                    break;


            }
            
            return postToReturn;
        }

    }
}
