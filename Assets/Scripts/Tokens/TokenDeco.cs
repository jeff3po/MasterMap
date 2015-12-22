using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TokenDeco : TokenBase 
{
	/// <summary>
	/// Things that can be done to this particular piece of furniture
	/// </summary>
	public List<string> ActivityCategory = new List<string>();
//	{
//		Examine,
//		SitOn,
//		SleepOn,
//		Unlock,
//		Open,
//		Close,
//		Break
//	}

	public Dictionary<string,Activity> activities = new Dictionary<string, Activity>();

	void Start()
	{
		
	}

	public void AddActivity ( string activityName, int diff )
	{
		activities.Add ( activityName, new Activity(diff) );
	}

	public bool PerformActivity ( string activityName, int roll )
	{
		Activity activity = null;
		// Find it
		activities.TryGetValue ( activityName, out activity );
		if ( activity == null ) { Debug.LogError ( "No activity: "+activityName); return false; }
		return activity.Attempt(roll);
	}
	// If it can be opened, it can have inventory

}
