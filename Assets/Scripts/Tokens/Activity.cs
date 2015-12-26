using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// An action that can be taken.  
/// </summary>
public class Activity 
{
	/// <summary>
	/// Displayed to player
	/// </summary>
	public string Name;

	/// <summary>
	/// Roll 1d20 vs ability to beat this value
	/// </summary>
	public int difficulty;

	/// <summary>
	/// Ability against which to roll
	/// </summary>
	string vsAbility;

	public enum ActivityState
	{
		Unattempted, 	// Hasn't been attempted
		Successful,		// Attempted and succeeded
		Unsuccessful,	// Attempted withot success, but can be re-attempted
		Failed			// Permanently failed
	}

	/// <summary>
	/// Current state of activity
	/// </summary>
	public ActivityState activityState = ActivityState.Unattempted;

	/// <summary>
	/// Some activities can be tried more than once
	/// </summary>
	public bool canRetry = true;

	/// <summary>
	///  When successful, this activity list opens up. Allows layered action sequences (Search for Traps -> Disarm Trap -> Open)
	/// </summary>
	public ActivityList activityListAvailableOnSuccess = null;

	/// <summary>
	/// If success needs to perform a specific action within a subclass, put it here
	/// </summary>
	System.Action successCallback;

	System.Action failCallback;

	/// <summary>
	/// Constructor
	/// </summary>
	public Activity ( string nm, int diff, string vsAbil, ActivityList onSuccess, bool retry, System.Action succCall=null, System.Action failCall=null )
	{
		Name = nm;
		difficulty = diff;
		vsAbility = vsAbil;
		activityListAvailableOnSuccess = onSuccess;
		canRetry = retry;
		successCallback = succCall;
		failCallback = failCall;
	}

	/// <summary>
	/// Auto-roll, ignore crit
	/// </summary>
	public bool Attempt( CharacterStats stats )
	{
		int ignoreCrit = 0;
		return Attempt ( -1, stats, out ignoreCrit);
	}

	/// <summary>
	/// Player provides roll, ignore crit
	/// </summary>
	public bool Attempt ( int roll, CharacterStats stats )
	{
		int ignoreCrit = 0;
		return Attempt ( roll, stats, out ignoreCrit);
	}

	/// <summary>
	/// Auto-roll, but fill in crit value
	/// </summary>
	public bool Attempt ( CharacterStats stats, out int crit )
	{
		return Attempt ( -1, stats, out crit );
	}

	/// <summary>
	/// Try it.
	/// </summary>
	public bool Attempt ( int roll, CharacterStats stats, out int critical )
	{
		critical = 0;

		// Sanity check to prevent disallowed attempts
		if ( activityState == ActivityState.Failed ) 
		{
			return false;
		}

		if ( roll == -1 )
		{
			// Include bonus from character if auto-rolling
			int bonus = stats.SkillModifier ( vsAbility );
			roll = DiceRoller.Roll ( 1, 20, bonus, out critical );
		}

		bool success = roll >= difficulty;

		// If successful, automatically unlock any associated list
		if ( success )
		{
			if ( activityListAvailableOnSuccess != null )
			{
				activityListAvailableOnSuccess.MakeAccessible ( true );
			}
		}

		if ( success )
		{
			activityState = ActivityState.Successful;
			if ( successCallback != null )
			{
				successCallback();
			}
		}
		else
		{
			if ( failCallback != null )
			{
				failCallback();
			}
			// Fsilure isn't necessarily the end
			if ( canRetry )
			{
				activityState = ActivityState.Unsuccessful;
			}
			else
			{
				activityState = ActivityState.Failed;
			}
		}

		return success;
	}

	public Activity ( ref JSONNode data, int tokenIndex, int listIndex, int actIndex )
	{
		Name = data [ "Activity" ] [ tokenIndex ] [ "name" ] [ listIndex ] [ actIndex ];
		difficulty = data [ "Activity" ] [ tokenIndex ] [ "diff" ] [ listIndex ] [ actIndex ].AsInt;
		vsAbility = data [ "Activity" ] [ tokenIndex ] [ "abil" ] [ listIndex ] [ actIndex ];
	}

	public void Export ( ref JSONNode data, int tokenIndex, int listIndex, int actIndex )
	{
		data [ "Activity" ] [ tokenIndex ] [ "name" ] [ listIndex ] [ actIndex ]= Name;
		data [ "Activity" ] [ tokenIndex ] [ "diff" ] [ listIndex ] [ actIndex ].AsInt = difficulty;
		data [ "Activity" ] [ tokenIndex ] [ "abil" ] [ listIndex ] [ actIndex ] = vsAbility;
	}
}
