<Query Kind="Program" />

void Main() {
	var content = 
		@"[Page:ISO-15926|ISO15926]</p>
		<p>Watch how the following DisplayTemplate gets mangled by the regular expression
		[Page:ISO15926]</p>
		[Page:CommandSplit]
		[SplitNicely]
		[ISO15926] gets mangled
		<p>This is a test [Multi-Targeted|MultiTargeted] to see how it will work and here is another page [PasswordManager] and its value</p>
		<p>&nbsp;</p>";
	var returnList = new List<BeSettingRecord>();
	foreach (var r in Regex.Matches(content.Replace("]","]\r\n"), @"\[.*]")){
		var match = r.Cast<Match>().Value.Replace("]","").Replace("[","");
		var display = match.Split('|'); 		// Did we provide DisplayTemplate?
		var isDisplaySplit = display.Length>1; 	// Set flag for use below
		var command = match.Split(':');			// Commands have colons
		// If we have a command (has a colon), i.e., [Page:Portable Class Library|PortableClassLibrary]
		if(command.Length>1) {
			display = command[1].Split('|'); 		// Did we provide DisplayTemplate?
			isDisplaySplit = display.Length>1; 		// Set flag for use below
			var displayTemplate = isDisplaySplit
			    ? display[0]  						// If display value provided then we use second element for parameter
				: display[0].SplitCamelCase(); 		// otherwise use provided command as CommandParameter
			var commandParameter = isDisplaySplit 
			    ? display[1]  // If display value provided then we use second element for parameter
				: display[0]; // otherwise use provided command as CommandParameter
			returnList.Add( new BeSettingRecord 
			{ // Create Wiki record
				Command=command[0], 	      		  // First element is always Command
				CommandParameter = commandParameter,  // CommandParameter
				DisplayTemplate = displayTemplate,	  // Display value
				Original = match
			});
			continue;	// we're done - continue next match
		}
		returnList.Add(new BeSettingRecord  		{	   // Create wiki record
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
	returnList.Dump();
}
public class BeSettingRecord  
{
	#region Ordinal indexes
	public const int IdIndex = 0;
	public const int PermaLinkIndex = 1;
	public const int CommandIndex = 2;
	public const int CommandParameterIndex = 3;
	public const int DisplayTemplateIndex = 4;
	#endregion

	/// <summary>
	/// Gets or sets the id.
	/// </summary>
	/// <value>The id.</value>
	[BeSetting("Id", "Id:")]
	public string Id { get; set; }                            // 1

	/// <summary>
	/// Gets or sets the perma link.
	/// </summary>
	/// <value>The perma link.</value>
	[BeSetting("PermaLink", "PermaLink:")]
	public string PermaLink { get; set; }                   // 2

	/// <summary>
	/// Gets or sets the command.
	/// </summary>
	/// <value>The command.</value>
	[BeSetting("Command", "Command:", "200", "true")]
	public string Command { get; set; }			            // 3

	/// <summary>
	/// Gets or sets the command parameter.
	/// </summary>
	/// <value>The command parameter.</value>
	[BeSetting("CommandParameter", "Command Parameter:", "200","true", "true")]
	public string CommandParameter { get; set; }            // 4

	/// <summary>
	/// Gets or sets the display value.
	/// </summary>
	/// <value>The display value.</value>
	[BeSetting("DisplayTemplate", "Display Template:")]
	public string DisplayTemplate { get; set; }             // 5

	/// <summary>
	/// Gets or sets the original.
	/// </summary>
	/// <value>The original.</value>
	public string Original { get; set;}

	/// <summary>
	/// Gets the extension data in the order they are presented
	/// above - important because we reflect over the property
	/// attributes to add them as parameters to extension settings
	/// </summary>
	/// <returns></returns>
	public string[] GetExtensionData()
	{
		return new string[]
				   {
					   Id.ToString(),                       // 1
					   PermaLink ?? "Not Assigned",         // 2
					   Command,                             // 3
					   CommandParameter,                    // 4
					   DisplayTemplate ?? "Not Assigned"    // 5
				   };
	}

	/// <summary>
	/// Gets the key field
	/// </summary>
	/// <value>The key field.</value>
	public string KeyField {
		get { return CommandParameter; }
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents this instance.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents this instance.
	/// </returns>
	public override string ToString()
	{
		return string.Format("Command: [{0}]  CommandParameter: [{1}]  DisplayValue: [{2}]  Original: [{3}]",
							 Command,
							 CommandParameter,
							 DisplayTemplate,
							 Original);
	}
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

	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Class)]
	public class BeSettingAttribute : Attribute
	{
		#region Fields
		private readonly string _name;
		private readonly string _label;
		private readonly string _maxLength;
		private readonly string _required;
		private readonly string _keyField;
		private readonly string _parameterType;
		#endregion 

		#region Properties
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get { return _name; } }

		/// <summary>
		/// Gets the label.
		/// </summary>
		/// <value>The label.</value>
		public string Label { get { return _label; } }
		
		/// <summary>
		/// Gets the length of the max.
		/// </summary>
		/// <value>The length of the max.</value>
		public string MaxLength { get { return _maxLength; } }
		
		/// <summary>
		/// Gets the required.
		/// </summary>
		/// <value>The required.</value>
		public string Required { get { return _required; } }
		
		/// <summary>
		/// Gets the key field.
		/// </summary>
		/// <value>The key field.</value>
		public string KeyField { get { return _keyField; } }
		
		/// <summary>
		/// Gets the type of the parameter.
		/// </summary>
		/// <value>The type of the parameter.</value>
		public string ParameterType{get { return _parameterType; }}
		#endregion 

		#region Constructors 
		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="required">The required.</param>
		/// <param name="keyField">The key field.</param>
		/// <param name="parType">Type of the par.</param>
		public BeSettingAttribute(string name, string label, string maxLength, string required, string keyField, string parType)
		{
			_name = name;
			_label = label;
			_maxLength = maxLength;
			_required = required;
			_keyField = keyField;
			_parameterType = parType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="required">The required.</param>
		/// <param name="keyField">The key field.</param>
		public BeSettingAttribute(string name, string label, string maxLength, string required, string keyField)
			: this(name,label,maxLength,required,keyField, null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="maxLength">Length of the max.</param>
		/// <param name="required">The required.</param>
		public BeSettingAttribute(string name, string label, string maxLength, string required)
			: this(name, label, maxLength, required, null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		/// <param name="maxLength">Length of the max.</param>
		public BeSettingAttribute(string name, string label, string maxLength) 
			: this(name,label,maxLength,null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="label">The label.</param>
		public BeSettingAttribute(string name, string label) 
			: this(name,label,null){}

		/// <summary>
		/// Initializes a new instance of the <see cref="BeSettingAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public BeSettingAttribute(string name)
			:this(name,null){ }
		#endregion 
	}
