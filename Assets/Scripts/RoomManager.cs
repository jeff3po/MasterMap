using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// All things reoom-related
/// </summary>
public class RoomManager : MonoBehaviour
{
	public WorldMap map;

	public List<Room> rooms = new List<Room>();

	Room currentRoom = null;

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

	/// <summary>
	/// Make a new one
	/// </summary>
	public void AddRoom()
	{
		Room newRoom = new Room(rooms.Count,map);
		newRoom.name = "Roomname_"+rooms.Count;
		map.controlPanel.visPanel.AddRoomToggle ( rooms.Count, newRoom.roomColor);
		rooms.Add ( newRoom );
		SetCurrentRoom ( newRoom );
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
		currentRoom.name = nm;
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
			foreach ( FloorTile t in r.tiles )
			{
				if ( t.isDoor )
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

		if ( currentRoom == null ) { return; }

		if ( currentAddFloorState == AddFloorState.undefined || currentAddFloorState == AddFloorState.Removing)
		{
			if ( currentRoom.tiles.Contains ( tile ) )
			{
				// Already exists, so toggle to remove mode and pull it out
				SetAddFloorState ( AddFloorState.Removing );
				if ( tile.isDoor )
				{
					tile.isDoor = false;
					Debug.Log ( "Destroying "+tile.attachedDoor.name );
					GameObject.Destroy ( tile.attachedDoor.gameObject );
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

			if ( r.tiles.Contains ( tile ) )
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
				if ( tile.isDoor == false )
				{
					tile.isDoor = true;

					Door newDoor = GameObject.Instantiate ( doorTemplate );
					newDoor.gameObject.SetActive ( true );
					newDoor.transform.SetParent( tile.transform);
					newDoor.transform.localScale = Vector3.one;
					newDoor.transform.localPosition = Vector3.zero;
					tile.attachedDoor = newDoor;
					newDoor.name = "Door_"+tile.name+" "+currentRoom.name+" -> "+otherRoom.name;
					Debug.Log ( "Adding "+newDoor.name );
					// Once a door has been added, no more drawing on this stroke
					SetAddFloorState ( AddFloorState.invalid );

					if ( axiallyAligned[0].yPos != tile.yPos)
					{
						// Vertically aligned, so rotate door to match
						newDoor.transform.localRotation = Quaternion.Euler ( new Vector3 ( 0,0,90));
					}
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

	bool resetAddFloor = false;
	public void ResetAddFloorState()
	{
		resetAddFloor = true;
	}

	public void SetAddFloorState ( AddFloorState state )
	{
		if ( state == currentAddFloorState ) { return; }
		Debug.Log ( "Changing state from "+currentAddFloorState.ToString()+" to "+state.ToString() );
		currentAddFloorState = state;
	}

	void Update()
	{
		if ( resetAddFloor )
		{
			resetAddFloor = false;
			SetAddFloorState ( AddFloorState.undefined );
		}

		string message = string.Format("State: {0}, touching: {1}", currentAddFloorState.ToString(), FloorTile.touching.ToString());
		SetDebugMessage ( message );
	}

	public Text debugMessage;
	void SetDebugMessage( string message )
	{
		debugMessage.text = message;
	}
}
