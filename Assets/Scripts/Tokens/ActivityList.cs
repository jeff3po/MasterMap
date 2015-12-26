using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

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

	public ActivityList ( ref JSONNode data, int tokenIndex, int listIndex )
	{
		activities.Clear();

		int activityCount = data [ "ActivityList" ] [ tokenIndex ] [ "activityCount" ] [ listIndex ].AsInt;
		for ( int actIndex=0;actIndex<activityCount;actIndex++ )
		{
			Activity activity = new Activity ( ref data, tokenIndex, listIndex, actIndex );
			activities.Add ( activity.Name, activity );
		}
	}


	public void Export ( ref JSONNode data, int tokenIndex, int listIndex )
	{
		data [ "ActivityList" ] [ tokenIndex ] [ "accessible" ].AsBool = accessible;
		if ( activities != null && activities.Values.Count > 0 )
		{
			int activityCount = 0;
			foreach ( Activity act in activities.Values )
			{
				act.Export ( ref data, tokenIndex, listIndex, activityCount );
				activityCount ++;
			}
			data [ "ActivityList" ] [ tokenIndex ] [ "activityCount" ] [listIndex].AsInt = activityCount;
		}
	}
}
