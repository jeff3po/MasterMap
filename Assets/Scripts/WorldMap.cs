using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class WorldMap : MonoBehaviour, IScrollHandler
{
	public RoomManager roomManager;

	public Image mapBG;

	/// <summary>
	/// Screen navigation - scrolling
	/// </summary>
	public RectTransform mapScroller;

	/// <summary>
	/// Screen navigation - zooming
	/// </summary>
	public RectTransform mapZoomer;

	/// <summary>
	///  For one-finger zooming
	/// </summary>
	public Scrollbar zoomBar;

	/// <summary>
	/// Bottom bar UI
	/// </summary>
	public ControlPanel controlPanel;

	FloorTile[,] worldGrid;

	public int columnCount;
	public int rowCount;

	public float maxZoom = 5;

	public List<Color> globalColors = new List<Color>();

	public Vector3 targetScrollPos = Vector3.zero;
	float zoomFactor = 1.0f;

	/// <summary>
	/// Pixel size of interface window
	/// </summary>
	public Vector2 mapSize;

	public enum EditMode
	{
		None,
		Room,
		Decoration,
		Play
	}

	public EditMode editMode = EditMode.None;

	public void SetEditMode ( EditMode mode )
	{
		editMode = mode;
		controlPanel.ShowPanel ( mode );
	}

	void Start()
	{
		roomManager.Init();

		// Define a standard pallete of colors
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
		// End with pure white
		globalColors.Add ( new Color(1,1,1));

		// Build the world according to specs
		worldGrid = new FloorTile[columnCount,rowCount];

		Vector2 tilesize = roomManager.floorTileSize;
		float tileWidth = tilesize.x;
		float tileHeight = tilesize.y;

		// Floor tile template starts centered in world. Build out then center
		for ( int x=0;x<columnCount;x++ )
		{
			for ( int y=0;y<rowCount;y++)
			{
				FloorTile newTile = Instantiate ( roomManager.floorTileTemplate );
				newTile.gameObject.SetActive ( true );

				newTile.transform.SetParent ( roomManager.floorTileTemplate.transform.parent );
				newTile.transform.localScale = Vector3.one;
				newTile.transform.localPosition = new Vector3 ( (tileWidth) + (tileWidth * x), (tileHeight)+(tileHeight * y), 0 );
				newTile.Setup ( this, x, y );
				newTile.name = x+"_"+y;
				worldGrid[x,y] = newTile;
				newTile.transform.SetAsFirstSibling();
				roomManager.allTiles.Add ( newTile );
			}
		}

		mapSize = mapBG.rectTransform.sizeDelta;
		targetScrollPos.x = -mapSize.x/2;
		targetScrollPos.y = -mapSize.y/2;
	}

	void Update()
	{
		float scrollJump = 64.0f;
		if ( Input.GetKeyDown(KeyCode.UpArrow)) { targetScrollPos += Vector3.up * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.DownArrow)) { targetScrollPos += Vector3.down * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.LeftArrow)) { targetScrollPos += Vector3.left * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.RightArrow)) { targetScrollPos += Vector3.right * scrollJump; }

		if ( targetScrollPos.x > -mapSize.x/2 ) { targetScrollPos.x = -mapSize.x/2; }
		if ( targetScrollPos.y > -mapSize.y/2 ) { targetScrollPos.y = -mapSize.y/2; }

		Vector3 delta = targetScrollPos - mapScroller.localPosition;
		delta *= Time.deltaTime * 9.0f;
		mapScroller.localPosition += delta;

		if ( Input.GetKeyDown ( KeyCode.S ) )
		{
			// HACK! Quicksave
			SaveMap();
		}
		if ( Input.GetKeyDown ( KeyCode.L ) )
		{
			// HACK! Quicksave
			LoadMap();
		}
	}


	// - - - S C R E E N   N A V I G A T I O N - - - -

	public void Scroll ( Vector2 scroll )
	{
		if ( controlPanel.CanDrag == false ) { return; }
		targetScrollPos += (Vector3)scroll * 3.0f;
	}

	public void OnScroll(PointerEventData data)
	{
		zoomFactor += data.scrollDelta.y * Time.deltaTime;
		if ( zoomFactor > maxZoom ) { zoomFactor = maxZoom;}
		if ( zoomFactor < 1 ) { zoomFactor = 1;}
		Zoom();
	}

	void Zoom ( bool updateZoomBar=true )
	{
		mapZoomer.localScale = Vector3.one * zoomFactor;

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




	// - - -  R O O M   M A N A G E M E N T  - - - -

	public void SetRoomVisibility ( int index, bool vis )
	{
		roomManager.SetRoomVisibility (index, vis);
	}

	public void AddRoom()
	{
		roomManager.AddRoom();
	}

	public int RoomShift ( int dir )
	{
		return roomManager.RoomShift(dir);
	}

	public void SetCurrentRoom ( Room room )
	{
		roomManager.SetCurrentRoom(room);
	}

	public bool SetCurrentRoomName ( string nm )
	{
		return roomManager.SetCurrentRoomName ( nm );
	}

	public Room SetCurrentRoom ( int roomID )
	{
		return roomManager.SetCurrentRoom(roomID);
	}


	public void InfoPanel ( Door door )
	{
		// Info panel about door
		roomManager.SetDebugMessage ( door.ToString() );
	}




	// - - -  I N T E R A C T I O N  - - - 
	/// <summary>
	/// TODO: FSM to manage all the different modes
	/// </summary>
	public void ClickThisTile ( FloorTile tile )
	{
		// Depending on current mode, reaction to touching a tile can be very different
		//		Debug.Log ( "Clicked "+tile.name);

		// Dragging instead of manipulating 
		if ( controlPanel.CanDrag ) { return; }

		roomManager.InteractWithFloorTile ( tile );
	}

	public void LoadMap()
	{
		roomManager.LoadAll();
	}

	public void SaveMap()
	{
		roomManager.SaveAll();
	}
}