using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// All things reoom-related
/// </summary>
public class RoomManager : SingletonMonoBehaviour<RoomManager>
{
	public WorldMap map;

	public List<Room> rooms = new List<Room>();
	public List<Door> doors = new List<Door>();

	public Room currentRoom = null;

	public Door doorTemplate;

	public List<FloorTile> allTiles = new List<FloorTile>();

	public enum AddFloorState
	{
		undefined, // Haven't started drawing yet
		Adding, // Have already added a tile, can only add more until touch event ends
		Removing, // Have already removed a tile, can only remove more until touch event ends
		invalid // Attempting to overlap rooms at points other than doors, interrupt drawing until touch event ends
	}

	/// <summary>
	/// When editing a room, touches can add/remove components based on this state
	/// </summary>
	public AddFloorState currentAddFloorState = AddFloorState.undefined;

	/// <summary>
	/// Every tile on the map starts here
	/// </summary>
	public FloorTile floorTileTemplate;

	// To show grid, swap out these sprites on every tile
	public Sprite gridFrame;
	public Sprite gridlessFrame;

	public void Init()
	{
		doorTemplate.gameObject.SetActive ( false );
		floorTileTemplate.gameObject.SetActive ( false );
		floorTileSize = floorTileTemplate.floorImage.rectTransform.sizeDelta;
	}

	public Vector2 floorTileSize = Vector2.zero;

	public Room MakeNewRoom()
	{
		GameObject go = new GameObject();
		Room newRoom = go.AddComponent<Room>();
		newRoom.SetupArchive();
		newRoom.Setup(rooms.Count);
		ControlPanel.Instance.visPanel.AddRoomToggle ( rooms.Count, newRoom.roomColor);
		rooms.Add ( newRoom );
		SetCurrentRoom ( newRoom );
		return newRoom;
	}
	/// <summary>
	/// Make a new one
	/// </summary>
	public void AddRoom()
	{
		Room newRoom = MakeNewRoom();
		// Force a default name when adding one from scratch
		newRoom.Name = "Roomname_"+rooms.Count;
	}

	/// <summary>
	/// Scroll through list of rooms one-by-one
	/// </summary>
	public int RoomShift ( int dir )
	{
		int currentIndex = GetRoomIndex(currentRoom);
		int newIndex = currentIndex + dir;
		if ( newIndex >= rooms.Count )
		{
			// Either end of list. No change
			return currentIndex;
		}
		else
		if ( newIndex < 0 )
		{
			SetCurrentRoom ( null );
			return -1;
		}

		SetCurrentRoom ( newIndex );

		return newIndex;
	}

	/// <summary>
	/// Find index in rooms list
	/// </summary>
	public int GetRoomIndex ( Room room )
	{
		if ( room == null ) { return -1; }

		for ( int i=0;i<rooms.Count;i++)
		{
			if ( room == rooms[i] )
			{
				return i;
			}
		}

		return -1;
	}

	public void SetCurrentRoom ( Room room )
	{
		currentRoom = room;

		foreach ( Room r in rooms )
		{
			if ( room == null )
			{
				r.SetVisible ( true );
			}
			else
			if ( r != currentRoom )
			{
				r.SetVisible ( false, true );
			}
			else
			{
				r.SetVisible ( true );
			}
		}

		// When a room is selected, show tile versions
		foreach ( FloorTile t in allTiles )
		{
			if ( room != null )
			{
				t.floorImage.sprite = gridFrame;
			}
			else
			{
				t.floorImage.sprite = gridlessFrame;
			}
		}
	}

	public Room SetCurrentRoom ( int roomID )
	{
		Room r = rooms[roomID];
		SetCurrentRoom ( r );
		return r;
	}


	public bool SetCurrentRoomName ( string nm )
	{
		if (currentRoom == null ) { return false; }
		currentRoom.Name = nm;
		return true;
	}

	/// <summary>
	/// Show or hide room by index
	/// </summary>
	public void SetRoomVisibility ( int index, bool vis )
	{
		rooms[index].SetVisible ( vis );

		// Go back through all doors and turn their tiles back on if either of their rooms is active
		foreach ( Room r in rooms )
		{
			if ( r.isVisible == false ) { continue; }
			foreach ( FloorTile t in r.tiles.Values )
			{
				if ( t.IsDoorway )
				{
					t.SetVisible ( true );
				}
			}
		}
	}

	/// <summary>
	/// Touched it. But what does that mean currently?
	/// </summary>
	public void InteractWithFloorTile ( FloorTile tile )
	{
		if ( currentAddFloorState == AddFloorState.invalid ) { return; }

		if ( tile.IsDoorway )
		{
			// Bring up door info, nothing else
			map.InfoPanel ( tile.attachedDoor );
			return;
		}

		if ( currentRoom == null ) 
		{
			return; 
		}

		if ( currentAddFloorState == AddFloorState.undefined || currentAddFloorState == AddFloorState.Removing)
		{
			if ( currentRoom.tiles.ContainsValue ( tile ) )
			{
				// Already exists, so toggle to remove mode and pull it out
				SetAddFloorState ( AddFloorState.Removing );
				if ( tile.IsDoorway )
				{
					doors.Remove ( tile.attachedDoor );
					tile.RemoveDoor();
				}
				currentRoom.RemoveFloorTile ( tile );
				// Reset current room to fix underlying room's version of this tile
				SetCurrentRoom ( currentRoom );
				return;
			}
		}

		Room otherRoom = null;

		// Automatically insert a door on valid tiles shared by exactly two rooms
		foreach ( Room r in rooms )
		{
			if ( r == currentRoom ) { continue; }

			if ( r.tiles.ContainsValue ( tile ) )
			{
				if ( otherRoom != null )
				{
					Debug.LogError ( "Too many overlapping rooms to add door to "+tile.name );
				}
				else
				{
					otherRoom = r;
				}
			}
		}

		// If overlapping other room, must be adjacent to at least one non-door tile within current room
		// I.e. can't drop a single tile in the center of another room.
		List<FloorTile> fromRoom = currentRoom.GetAdjacent(tile, true, false);

		// If not overllaping (or overlapping in a way that can make a door), go ahead and add it
		if ( otherRoom == null || fromRoom.Count > 0 )
		{
			if ( currentAddFloorState == AddFloorState.undefined || currentAddFloorState == AddFloorState.Adding)
			{
				SetAddFloorState ( AddFloorState.Adding );
				currentRoom.AddFloorTile ( tile );
			}
		}

		if ( otherRoom != null && fromRoom.Count > 0)
		{
			// HACK! For the moment, a door can only be placed in a single tile that merges two rooms
			List<FloorTile> axiallyAligned = otherRoom.GetAdjacent(tile, true, false);
			axiallyAligned.AddRange ( currentRoom.GetAdjacent(tile,true,false) );

			if (axiallyAligned.Count == 2 )
			{
				// Perfect for door. Temporarily remove this tile, 
				// reevaluate the rooms to split them then add the tile back to both rooms with the door flag

				// Add door only once
				if ( tile.IsDoorway == false )
				{
					// Once a door has been added, no more drawing on this stroke
					SetAddFloorState ( AddFloorState.invalid );

					// TODO: Figure out facing for barred doors (since they only lock in one direction)
					Door.Facing facing = Door.Facing.East;
					if ( axiallyAligned[0].yPos != tile.yPos)
					{
						// Vertically aligned, so rotate door to match
						facing = Door.Facing.South;
					}

					MakeNewDoor(tile,facing);
				}
			}
			else
			{
				// No non-door overlaps allowed. Take it back!
				currentRoom.RemoveFloorTile ( tile );
				SetAddFloorState ( AddFloorState.invalid );
				SetCurrentRoom ( currentRoom );
			}
		}
	}

	void MakeNewDoor ( FloorTile tile, Door.Facing facing )
	{
		Door newDoor = GameObject.Instantiate ( doorTemplate );
		// TODO: Locked, barred, stuck, trapped, etc
		newDoor.Setup ( tile, facing );

		doors.Add ( newDoor );
	}

	bool resetAddFloor = false;

	/// <summary>
	/// Since the pointerUp event happens between frames, the enum for AddFloorState is inaccessible at that instant.
	/// Wait until next frame to complete reset
	/// </summary>
	public void ResetAddFloorState()
	{
		resetAddFloor = true;
	}

	public void SetAddFloorState ( AddFloorState state )
	{
		if ( state == currentAddFloorState ) { return; }
		currentAddFloorState = state;
	}

	void Update()
	{
		if ( resetAddFloor )
		{
			resetAddFloor = false;
			SetAddFloorState ( AddFloorState.undefined );
		}

//		string message = string.Format("State: {0}, touching: {1}", currentAddFloorState.ToString(), FloorTile.touching.ToString());
//		SetDebugMessage ( message );
	}

	public Text debugMessage;
	public void SetDebugMessage( string message )
	{
		debugMessage.text = message;
	}

	public FloorTile FindTileByID ( string id )
	{
		// TODO: Change to dictionary
		foreach ( FloorTile t in allTiles )
		{
			if ( t.UniqueID() == id )
			{
				return t;
			}
		}
		Debug.LogWarning ( "Can't find tile "+id);
		return null;
	}


	// - - -  A R C H I V I N G  - - - -

	public void SaveByName ( string nm )
	{	
		// Some quick sanity checks before writing this out.
		// Go in reverse order so we can cull empty groups as we go
		for ( int i=rooms.Count-1;i>=0;i--)
		{
			Room r = rooms[i];
			if ( r.tiles.Count == 0 )
			{
				rooms.RemoveAt(i);
			}
		}

		// Construct the JSON version
		string blanknode = "{\"archiveType\":\"World\"}";
		JSONNode data = JSONNode.Parse (blanknode);

		int roomCount = 0;
		foreach ( Room r in rooms )
		{
			r.Export ( ref data, roomCount );
			roomCount ++;
		}
		data [ "World" ] ["roomCount"].AsInt = roomCount;

		// NOTE: Doors require rooms to be established first
		int doorCount = 0;
		foreach ( Door d in doors )
		{
			d.Export ( ref data, doorCount );
//			Debug.Log ( "Door "+doorCount+" "+d.ToString() );
			doorCount ++;
		}
		data [ "World" ] ["doorCount"].AsInt = doorCount;

		int charCount = 0;
		foreach ( TokenCharacter chars in WorldMap.Instance.characterTokens )
		{
			chars.Export ( ref data, charCount );
			charCount ++;
		}
		data [ "World" ] ["charCount"].AsInt = charCount;

		string jsonstring = data.ToString();

		PlayerPrefs.SetString ( nm, jsonstring );

		Debug.Log ( DBDatabase.jsonToReadable ( jsonstring ) );
	}

	public void LoadByName(string nm)
	{
		// Wipe out any previous
		foreach ( Room r in rooms ) { Destroy ( r.gameObject ); }
		rooms.Clear();
		foreach ( Door d in doors ) { Destroy ( d.gameObject ); }
		doors.Clear();
		foreach ( TokenCharacter token in WorldMap.Instance.characterTokens ) { Destroy ( token.gameObject ); }
		WorldMap.Instance.characterTokens.Clear();

		string jsonString = PlayerPrefs.GetString ( nm );

		if ( jsonString == "" )
		{
			Debug.LogError ( "Can't load "+nm );
			return;
		}
		Debug.Log ( "Loading: "+jsonString );

		JSONNode data = JSON.Parse ( jsonString );

		int roomCount = data [ "World" ] [ "roomCount" ].AsInt ;
		for ( int i=0;i<roomCount;i++)
		{
			Room newRoom = MakeNewRoom();
			newRoom.Init ( ref data, i );
			ControlPanel.Instance.drawingPanel.ResetPicker();
		}

		int doorCount = data [ "World" ] [ "doorCount" ].AsInt ;
		for ( int i=0;i<doorCount;i++)
		{
			Door newDoor = Instantiate ( doorTemplate );
			newDoor.Init ( ref data, i );
			doors.Add ( newDoor );
		}

		int charCount = data [ "World" ] ["charCount"].AsInt;
		for ( int i=0;i<charCount;i++ )
		{
			TokenCharacter token = Instantiate ( WorldMap.Instance.tokenCharacterTemplate);
			token.gameObject.SetActive ( true );
			token.transform.SetParent ( WorldMap.Instance.tokenLayer);
			token.transform.localScale = Vector3.one;
			token.Init ( ref data, i );
			WorldMap.Instance.characterTokens.Add ( token );
		}

		foreach ( Room r in rooms )
		{
			r.PostInit();
		}
		foreach ( Door d in doors )
		{
			d.PostInit();
		}
		foreach ( TokenCharacter ch in WorldMap.Instance.characterTokens )
		{
			ch.PostInit();
		}
	}

}
