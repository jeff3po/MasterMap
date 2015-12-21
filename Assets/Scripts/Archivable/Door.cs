using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Door : Archivable 
{
	/// <summary>
	/// Handy shortcut to the manager
	/// </summary>
	RoomManager roomManager;

	/// <summary>
	/// Where it lives
	/// </summary>
	FloorTile homeTile;

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
		roomManager = FindObjectOfType<RoomManager>();
	}

	public override string UniqueID()
	{
		return "Door"+ID;
	}

	public void Setup ( RoomManager manager, FloorTile tile, Facing fac )
	{
		gameObject.SetActive ( true );

		SetupArchive();

		homeTile = tile;
		roomManager = manager;
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
	}



	// - - - A R C H I V E - - - - -

	public override void PostInit()
	{
		homeTile = roomManager.FindTileByID ( homeTileID );
		if ( roomManager == null ) { Debug.LogError ( "Can't find room manager!");}

		// Call setup from within to hook all the properties together
		Setup ( roomManager, homeTile, facing );
	}

	string homeTileID = "";

	public override void Init (ref JSONNode data, int i )
	{
		SetupArchive();

		base.Init (ref data, i );

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
		string states = string.Format ("   Locked: {0}  Barred: {1}   Stuck: {2}", isLocked, isBarred, isStuck);
		string message = string.Format ( "{0} - facing {1} ", Name, facing.ToString() );
		message += states;
		return message;
	}

}
