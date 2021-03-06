﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Door : TokenDeco 
{
	/// <summary>
	/// Which direction is it facing? (North-South or East-West for orientation, exact facing matters for barred door
	/// </summary>
	Facing facing;

	/// <summary>
	/// Is it locked?
	/// </summary>
	bool isLocked = false;

	/// <summary>
	/// Can it be opened from only one side?
	/// </summary>
	bool isBarred = false;

	/// <summary>
	/// Is it stuck shut?
	/// </summary>
	bool isStuck = false;

	public enum Facing
	{
		North,
		East,
		South,
		West
	}

	public override void SetupArchive()
	{
		base.SetupArchive();
		Category = "Doors";
	}

	public override string UniqueID()
	{
		return "Door"+ID;
	}

	public void Setup ( FloorTile tile, Facing fac )
	{
		gameObject.SetActive ( true );

		SetupArchive();

		homeTile = tile;
		transform.SetParent( tile.transform);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		tile.AttachDoor ( this );
		Name = "Door_"+tile.UniqueID();
		// And the gameobject in the scene, too
		name = "Door_"+tile.UniqueID();
		facing = fac;
		if ( fac == Facing.North || fac == Facing.South )
		{
			transform.localRotation = Quaternion.Euler ( new Vector3 ( 0,0,90));
		}

		// Setup token properties
		ActivityList defaultActivities = new ActivityList("default",true);
		ActivityList postExamineActivities = new ActivityList("postExamine",false);
		ActivityList postUnlockActivities = new ActivityList("postUnlock",false);

		defaultActivities.AddActivity ( new Activity ( "Examine", 5, "Int", postExamineActivities, true ));

		postExamineActivities.AddActivity ( new Activity ( "Pick Lock", 12, "Dex", postUnlockActivities, false, null, BreakLockpick ));
		postExamineActivities.AddActivity ( new Activity ( "Break Down", 15, "Str", postUnlockActivities, true ));

		postUnlockActivities.AddActivity ( new Activity ( "Open", 0, "Str", null, false, OpenDoor ));

		AddActivityList ( defaultActivities );
		AddActivityList ( postExamineActivities );
		AddActivityList ( postUnlockActivities );
	}

	void BreakLockpick()
	{
		Debug.Log ("Breal lockpick");
	}

	void OpenDoor()
	{
		Debug.Log ( "Open door!");
	}

	// - - - A R C H I V E - - - - -

	public override void PostInit()
	{
		homeTile = RoomManager.Instance.FindTileByID ( homeTileID );

		// Call setup from within to hook all the properties together
		Setup ( homeTile, facing );
	}

	public override void Init ( ref JSONNode data, int i )
	{
		SetupArchive();

		base.Init ( ref data, i );

		homeTileID = data [ Category ] [ i ] [ "tileID" ];
		string facingString = data [ Category ] [ i ] [ "facing" ];
		facing = (Facing) System.Enum.Parse ( typeof(Facing), facingString );
		isLocked = data [ Category ] [ i ] [ "locked" ].AsBool;
		isBarred = data [ Category ] [ i ] [ "barred" ].AsBool;
		isStuck = data [ Category ] [ i ] [ "stuck" ].AsBool;
	}


	public override void Export(ref JSONNode data, int i)
	{
		base.Export (ref data, i);

		data [ Category ] [ i ] [ "tileID" ] = homeTile.UniqueID();
		data [ Category ] [ i ] [ "facing" ] = facing.ToString();
		data [ Category ] [ i ] [ "locked" ].AsBool = isLocked;
		data [ Category ] [ i ] [ "barred" ].AsBool = isBarred;
		data [ Category ] [ i ] [ "stuck" ].AsBool = isStuck;
	}




	public override string ToString()
	{
		string states = string.Format ("   Locked: {0}  Barred: {1}   Stuck: {2} - homeTile: {3}", isLocked, isBarred, isStuck, homeTile.UniqueID());
		string message = string.Format ( "{0} - facing {1} ", Name, facing.ToString() );
		message += states;
		return message;
	}

}
