﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Token subclass specific to non-structural elements of the map (i.e. furniture).
/// Activities can be assigned to these tokens, providing players a network of actions to perform on the object
/// </summary>
public class TokenDeco : TokenBase 
{
	public Dictionary<string,ActivityList> activityLists = new Dictionary<string, ActivityList>();

	public override void Interact()
	{
		base.Interact();

		if ( WorldMap.Instance.editMode != WorldMap.EditMode.Play ) 
		{
			return;
		}

		if ( activityLists.Count == 0 ) { return; }
			
		string display = string.Format("Interacting with {0}", Name );
		foreach ( ActivityList alist in activityLists.Values )
		{
			if ( alist.IsAccessible )
			{
				// Show entries
				foreach ( string act in alist.activities.Keys )
				{
					WorldMap.Instance.chooser.AddButton(act,ChooseThisActivity);
					display += "\n"+act;
				}
			}
		}
		Debug.Log ( display );
	}

	void ChooseThisActivity ( string act )
	{
		foreach (ActivityList list in activityLists.Values )
		{
			list.activities.TryGetValue ( act, out currentActivity );
			if ( currentActivity != null )
			{
				break;
			}
		}

		if ( currentActivity == null )
		{
			Debug.LogError ( "Couldn't find "+act);
			return;
		}

		// Roll for it
		Debug.Log ( "Roll for "+act );

		// TODO: Character stat for rolling
//		int statMod = stats.SkillModifier ( currentAttack.abilityForRoll );
//		statMod += currentAttack.plusToHit;
		int targetRoll = currentActivity.difficulty;

		SpinnerPanel.Instance.SpinTheWheel ( new Dice ( 1,20,0), activityResult, targetRoll, "to "+currentActivity.Name );
	}

	Activity currentActivity = null;

	void activityResult ( int result, bool success )
	{
		Debug.Log ( currentActivity.Name+" success: "+success);
		if ( success )
		{
			if ( currentActivity.activityListAvailableOnSuccess != null )
			{
				currentActivity.activityListAvailableOnSuccess.MakeAccessible(true);

				// Automatically re-build the updated menu
				Interact();
			}
		}
	}

	public void AddActivityList ( ActivityList act )
	{
		activityLists.Add ( act.title, act );
	}
}
