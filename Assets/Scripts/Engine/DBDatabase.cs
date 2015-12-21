using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.IO;

/// <summary>
/// Since all databases are static, they can't be derived from a base class, 
/// so this is merely a helper class for saving/loading databases using the same method
/// </summary>
public static class DBDatabase 
{
	public static JSONNode ImportDB ( bool fromAsset, string jsonString )
	{
		if ( string.IsNullOrEmpty ( jsonString ) ) { Debug.LogError ( "No data found" ); return null; }
		
		JSONNode data = JSON.Parse ( jsonString );
		
		return data;
	}

	public static JSONNode ImportDB ( string path )
	{
		// Get string from text asset
		string jsonString = "";
		
		using ( StreamReader sr = new StreamReader ( path ) )
		{
			// Only the first line matters
			jsonString = sr.ReadLine();
		}
		
		if ( string.IsNullOrEmpty ( jsonString ) ) { Debug.LogError ( "No data found" ); return null; }
		
		JSONNode data = JSON.Parse ( jsonString );

		return data;
	}	

	public static void ExportDB ( string path, string jsonString )
	{
		using ( StreamWriter sw = new StreamWriter ( path ) )
		{
			sw.WriteLine ( jsonString );
			// Only the first line is imported. The remeaining lines are for debugging
			sw.WriteLine ("Saved: " + System.DateTime.Now);
			sw.Write (jsonToReadable (jsonString));
		}
	}

	/// <summary>
	/// Convert single-line JSON string to indented, human-readable document
	/// </summary>
	public static string jsonToReadable (string json)
	{
		int tc = 0;        //tab count
		string r = "";        //result
		bool q = false;     //quotes
		string tab = "\t";      //tab
		string nl = "\n";     //new line
		
		for (int i=0; i<json.Length; i++) {
			char c = json [i];
			if (c == '"' && json [i - 1] != '\\') { q = !q; }
			if (q) { r += c; continue; }
			
			if (c == '{' || c == '[') { tc++; r += nl; for (int tb=0; tb<tc; tb++) { r += tab; } } 
			else if (c == '}' || c == ']') { tc--; r += nl; } 
			else if (c == ',') { r += c; if (json [i + 1] != '{' && json [i + 1] != '[') { r += nl; for (int tb=0; tb<tc; tb++) { r += tab; } } }
			else if (c == ':') { r += c + " "; } 
			else { r += c; }
		}
		return r;
	}
}
