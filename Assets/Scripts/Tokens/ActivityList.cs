using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A list of activities. Contains prerequisite(s) before being available to player.
/// </summary>
public class ActivityList : MonoBehaviour 
{
	public List<Activity> possibleActivities = new List<Activity>();
	bool accessible = true;

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
		possibleActivities.Add ( activity );
	}
}
