using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A list of activities. Contains prerequisite(s) before being available to player.
/// </summary>
public class ActivityList 
{
	public string title;
	public Dictionary<string,Activity> activities = new Dictionary<string, Activity>();
	bool accessible = true;

	public ActivityList ( string tit, bool access )
	{
		title = tit;
		accessible = access;
	}

	public bool IsAccessible
	{
		get { return accessible; }
	}

	public void MakeAccessible ( bool a )
	{
		accessible = a;
	}

	public void AddActivity ( Activity activity )
	{
		activities.Add ( activity.Name, activity );
	}
}
