using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action that can be taken.  
/// </summary>
public class Activity 
{
	public int difficulty = 10;

	public enum ActivityState
	{
		Unattempted, 	// Hasn't been attempted
		Successful,		// Attempted and succeeded
		Unsuccessful,	// Attempted withot success, but can be re-attempted
		Failed			// Permanently failed
	}

	public Activity ( int diff=0 )
	{
		difficulty = diff;
	}

	public ActivityState activityState = ActivityState.Unattempted;

	/// <summary>
	/// Some activities can be tried more than once
	/// </summary>
	public bool supportsMultipleAttempts = true;

	/// <summary>
	///  When successful, this activity list opens up
	/// </summary>
	public ActivityList activityListAvailableOnSuccess = null;

	// Auto-roll, ignore crit
	public bool Attempt()
	{
		int ignoreCrit = 0;
		return Attempt ( -1, out ignoreCrit);
	}

	/// <summary>
	/// Ignore crit
	/// </summary>
	public bool Attempt ( int roll )
	{
		int ignoreCrit = 0;
		return Attempt ( roll, out ignoreCrit);
	}

	/// <summary>
	/// Auto-roll
	/// </summary>
	public bool Attempt ( out int crit )
	{
		return Attempt ( -1, out crit );
	}

	/// <summary>
	/// Try it.
	/// </summary>
	public bool Attempt ( int roll, out int critical )
	{
		critical = 0;

		// Sanity check to prevent disallowed attempts
		if ( activityState == ActivityState.Failed ) 
		{
			return false;
		}

		if ( roll == -1 )
		{
			// TODO: Use player stats for modifier / advantage
			roll = DiceRoller.Roll ( 1, 20, 0, out critical );
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
		}
		else
		{
			// Fsilure isn't necessarily the end
			if ( supportsMultipleAttempts )
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
}
