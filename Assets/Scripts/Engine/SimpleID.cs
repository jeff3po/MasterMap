using UnityEngine;
using System.Collections;

/// <summary>
/// This app doesn't need a huge GUID. A simple incrementing INT is adequate
/// </summary>
public static class SimpleID
{
	static int highestID = 0;

	public static int NewID
	{
		get
		{
			highestID++;
			return highestID;
		}
	}

	/// <summary>
	/// When importing, all IDs must be passed through here to reset the static highestID
	/// </summary>
	public static void ValidateID ( int id )
	{
		if ( id > highestID ) { highestID = id; }
	}
}
