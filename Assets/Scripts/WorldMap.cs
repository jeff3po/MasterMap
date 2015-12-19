using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorldMap : MonoBehaviour, IScrollHandler
{
	public Transform mapScroller;
	public FloorTile floorTileTemplate;
	public Scrollbar zoomBar;
	public ControlPanel controlPanel;

	public Door doorTemplate;

	public List<Room> rooms = new List<Room>();

	FloorTile[,] worldGrid;

	public int columnCount;
	public int rowCount;

	public float maxZoom = 5;

	public List<Color> globalColors = new List<Color>();

	public enum EditMode
	{
		None,
		Room,
		Decoration,
		Play
	}

	public EditMode editMode = EditMode.None;

	void Start()
	{
		doorTemplate.gameObject.SetActive ( false );
		for ( int r=0;r<3;r++)
		{
			for ( int g=0;g<3;g++)
			{
				for ( int b=0;b<3;b++)
				{
					Color c = new Color ( r*0.33f, g*0.33f, b*0.33f);
					globalColors.Add ( c );
				}
			}
		}

		globalColors.Add ( new Color(1,1,1));
		floorTileTemplate.gameObject.SetActive ( false );

		worldGrid = new FloorTile[columnCount,rowCount];

		float tileWidth = floorTileTemplate.floorImage.rectTransform.sizeDelta.x;
		float tileHeight = floorTileTemplate.floorImage.rectTransform.sizeDelta.y;

		// Floor tile template starts centered in world. Build out then center
		for ( int x=0;x<columnCount;x++ )
		{
			for ( int y=0;y<rowCount;y++)
			{
				FloorTile newTile = Instantiate ( floorTileTemplate );
				newTile.gameObject.SetActive ( true );

				newTile.transform.SetParent ( floorTileTemplate.transform.parent );
				newTile.transform.localScale = Vector3.one;
				newTile.transform.localPosition = new Vector3 ( tileWidth * x, tileHeight * y, 0 );
				newTile.Setup ( this, x, y );
				newTile.name = x+"_"+y;
				worldGrid[x,y] = newTile;
			}
		}
	}

	public void Scroll ( Vector2 scroll )
	{
		if ( controlPanel.CanDrag )
		{
			mapScroller.transform.Translate ( scroll );
		}
	}


	float zoomFactor = 1.0f;

	public void OnScroll(PointerEventData data)
	{
		zoomFactor += data.scrollDelta.y * Time.deltaTime;
		if ( zoomFactor > maxZoom ) { zoomFactor = maxZoom;}
		if ( zoomFactor < 1 ) { zoomFactor = 1;}
		Zoom();
	}

	void Zoom ( bool updateZoomBar=true )
	{
		mapScroller.localScale = Vector3.one * zoomFactor;

		if ( updateZoomBar )
		{
			zoomBar.value = Helpers.RemapToRange ( 1,5,0,1, zoomFactor);
		}
	}

	public void ManualZoom()
	{
		zoomFactor = Helpers.RemapToRange ( 0, 1, 1, 5, zoomBar.value );
		Zoom(false);
	}

	public enum AddFloorState
	{
		undefined,
		Adding,
		Removing
	}

	public AddFloorState addState = AddFloorState.undefined;

	public void DrawOnThisTile( FloorTile tile )
	{
		// Which mode are we in?
		if ( controlPanel.drawingPanelMode == ControlPanel.DrawingPanelMode.Floor )
		{
			AddToAdjacentRoom ( tile );
		}
		else
		if ( controlPanel.drawingPanelMode == ControlPanel.DrawingPanelMode.Door )
		{
			DivideRoom ( tile );
		}
	}

	public void DivideRoom ( FloorTile tile )
	{
		FloorTile doorTile = null;
		// Must be touching exising floor tile, and it must have exactly two adjacent tiles axially aligned
		foreach ( Room r in rooms )
		{
			if ( r.tiles.Contains ( tile ) )
			{
				// HACK! For the moment, a door can only be placed in a single tile that merges two rooms
				List<FloorTile> axiallyAligned = r.GetAdjacent(tile, true);

				if (axiallyAligned.Count == 2 )
				{
					// Perfect for door. Temporarily remove this tile, 
					// reevaluate the rooms to split them then add the tile back to both rooms with the door flag
					doorTile = tile;
					doorTile.isDoor = true;
					r.RemoveFloorTile(tile);
					ReevaluateMap();

					Door newDoor = Instantiate ( doorTemplate );
					newDoor.gameObject.SetActive ( true );
					newDoor.transform.SetParent( doorTile.transform);
					newDoor.transform.localScale = Vector3.one;
					newDoor.transform.localPosition = Vector3.zero;
					if ( axiallyAligned[0].yPos != doorTile.yPos)
					{
						// Vertically aligned, so rotate door to match
						newDoor.transform.localRotation = Quaternion.Euler ( new Vector3 ( 0,0,90));
					}

					// Re-add to every possible adjacent 
					//(should only be the two divided by this door accoring to axiallyAligned check above)
					foreach ( Room newroom in rooms )
					{
						if ( newroom.IsAdajcent ( doorTile ))
						{
							newroom.AddFloorTile ( doorTile );
						}
					}
				}
				break;
			}
		}
	}

	public void AddToAdjacentRoom ( FloorTile tile )
	{
		// If a door has been set, the tile's room can't be altered
		if ( tile.isDoor ) { return; }

		// Clicking within an existing room toggles it
		if ( addState == AddFloorState.undefined || addState == AddFloorState.Removing)
		{
			bool reevaluate = false;
			foreach ( Room r in rooms )
			{
				if ( r.tiles.Contains ( tile ) )
				{
					addState = AddFloorState.Removing;
					r.RemoveFloorTile(tile);
					tile.SetColor ( Color.white );
					reevaluate = true;
					break;
				}
			}
			if ( reevaluate )
			{
				ReevaluateMap();
				return;
			}
		}

		if ( addState == AddFloorState.undefined || addState == AddFloorState.Adding )
		{
			List<Room> adjacentRooms = new List<Room>();
			foreach ( Room r in rooms )
			{
				if ( r.IsAdajcent(tile,true) )
				{
					addState = AddFloorState.Adding;
					adjacentRooms.Add ( r );
				}
			}

			if ( adjacentRooms.Count == 1 )
			{
				Room r = adjacentRooms[0];
				r.AddFloorTile ( tile );
				return;
			}
			else
			if ( adjacentRooms.Count > 1 )
			{
				Room r = adjacentRooms[0];
				r.AddFloorTile ( tile );
				for ( int i=1;i<adjacentRooms.Count;i++)
				{
					Room otherRoom = adjacentRooms[i];
					foreach ( FloorTile t in otherRoom.tiles )
					{
						r.AddFloorTile(t);
					}
				}
				// Clear out the latters from the main room list
				while ( adjacentRooms.Count > 1 )
				{
					rooms.Remove ( adjacentRooms[1] );
					adjacentRooms.RemoveAt(1);
				}

				ResetVisPanel();

				return;
			}

			Room newRoom = new Room(rooms.Count,this);
			controlPanel.visPanel.AddRoomToggle ( rooms.Count, newRoom.roomColor);
			rooms.Add( newRoom );
			addState = AddFloorState.Adding;
			newRoom.AddFloorTile( tile );
			ResetVisPanel();
		}
	}

	void ResetVisPanel()
	{
		// Go in reverse order to wipe out empty room definitions
		for ( int i=rooms.Count-1;i>=0;i--)
		{
			if ( rooms[i].tiles.Count == 0 )
			{
				rooms.RemoveAt ( i );
			}
		}

		controlPanel.visPanel.Reset();

		for ( int i=0;i<rooms.Count;i++)
		{
			controlPanel.visPanel.AddRoomToggle ( i, rooms[i].roomColor );
		}
	}

	void ReevaluateMap()
	{
		controlPanel.visPanel.Reset();

		// If we remove a tile, we'll fill this list and re-evaluate the entire map
		List<FloorTile> allActiveTiles = new List<FloorTile>();
		foreach ( Room r in rooms )
		{
			foreach ( FloorTile t in r.tiles )
			{
				allActiveTiles.Add ( t );
			}
		}

		// Go through re-adding all the tiles as though we're starting from scratch
		rooms.Clear();

		foreach ( FloorTile t in allActiveTiles )
		{
			addState = AddFloorState.Adding;
			AddToAdjacentRoom(t);
		}
	}

	public void SetRoomVisibility ( int index, bool vis )
	{
		foreach ( FloorTile t in rooms[index].tiles )
		{
			t.gameObject.SetActive ( vis );
		}
	}
}
