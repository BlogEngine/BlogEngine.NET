<Query Kind="Program" />

void Main() {
	var content = 
		@"[Page:PortableClassLibrary]</p>
		<p>This is a test [Multi-Targeted|MultiTargeted] to see how it will work and here is another page [PasswordManager] and its value</p>
		<p>&nbsp;</p>";

	var returnList = new List<GwnWikiRecord>();
	foreach (var r in Regex.Matches(content.Replace("]","]\r\n"), @"\[.*]"))
	{
		var match = r.Cast<Match>().Value.Replace("]","").Replace("[","");
		var command = match.Split(':');
		// If we have a command, i.e., Page:PortableClassLibrary
		if(command.Length>1){
			returnList.Add( new GwnWikiRecord { // Create Wiki record
				Command=command[0], 	      // First element is Command
				CommandParameter=command[1],  // Second CommandParameter
				DisplayValue = command[1].SplitCamelCase()
			});
			continue;	// we're done - continue next match
		}
		var display = match.Split('|'); // Did we provide DisplayValue?
		var isDisplaySplit = display.Length>1; // Set flag for use below
		returnList.Add(new GwnWikiRecord  {	   // Create wiki record
	    	Command = "Display",
			CommandParameter = display.Length>1 
			    ? display[1]  // If a display value provided - use it
				: display[0], // otherwise use Command as CommandParameter
		   	DisplayValue = isDisplaySplit 
			  	? display[0]  // If display value provided = use it
				: display[0].SplitCamelCase() // otherwise split on case
		   });
	}
	returnList.Dump();
}
public class GwnWikiRecord {
	public string Command { get; set; }			// Wiki Command
	public string CommandParameter {get;set;}	// Parameter
	public string DisplayValue { get; set; }	// Value to display
}
public static class ObjectExtension {
	public static T Cast<T>(this object sender)	
	{
		if (sender is T) return (T)sender;
		return default(T);
	}
	public static string SplitCamelCase(this string stringToSplit)	
	{
		var result = string // string.Join will delimit results with " "
			.Join(" ", Regex.Split (stringToSplit, @"(?=[A-Z])"));
		return result.TrimStart(); // Trim off any leading spaces
	}
}