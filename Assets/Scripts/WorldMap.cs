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

	List<Room> rooms = new List<Room>();

	FloorTile[,] worldGrid;

	public int columnCount;
	public int rowCount;

	public float maxZoom = 5;

	public List<Color> globalColors = new List<Color>();

	void Start()
	{
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

		Vector2 offset = new Vector2 ( -columnCount/4 * tileWidth, -rowCount/4 * tileHeight );

		foreach ( FloorTile t in worldGrid )
		{
			t.transform.Translate ( offset );
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

	public void AddToAdjacentRoom ( FloorTile tile )
	{
		List<Room> adjacentRooms = new List<Room>();
		foreach ( Room r in rooms )
		{
			if ( r.IsAdajcent(tile) )
			{
				adjacentRooms.Add ( r );
			}
		}

		if ( adjacentRooms.Count == 1 )
		{
			Room r = adjacentRooms[0];
			r.AddFloorTile ( tile );
			Debug.Log ( "Added tile "+tile.name+" to "+r.name );
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
			
			// And move all the remaining tiles into this room, too
			Debug.Log ( "Added tile "+tile.name+" to "+r.name );
			return;
		}


		Room newRoom = new Room(rooms.Count,this);
		rooms.Add( newRoom );
		newRoom.AddFloorTile( tile );
		Debug.Log ( "Added tile "+tile.name+" to "+newRoom.name );
	}
}
