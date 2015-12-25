using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class WorldMap : SingletonMonoBehaviour<WorldMap>, IScrollHandler
{
	public TokenCharacter tokenCharacterTemplate;

	public Chooser chooser;

	public RectTransform tokenLayer;

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

	FloorTile[,] worldGrid;

	public int columnCount;
	public int rowCount;

	public float maxZoom = 5;

	public List<Color> globalColors = new List<Color>();

	public Vector3 targetScrollPos = Vector3.zero;
	float zoomFactor = 1.0f;

	public Spinner spinnerTemplateOnDisk;

	/// <summary>
	/// Pixel size of interface window
	/// </summary>
	public Vector2 mapSize;

	public enum EditMode
	{
		Room, // Floors and doors setup
		Deco, // Stationary tokens setup
		Play  // Moving tokens with visibility and movement rules
	}

	public EditMode editMode = EditMode.Room;

	public void SetEditMode ( EditMode mode )
	{
		editMode = mode;
		ControlPanel.Instance.ShowPanel ( mode );
	}

	void Start()
	{
		SpinnerPanel.Instance.gameObject.SetActive ( false );

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
				newTile.Setup ( x, y );
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

	public FloorTile FindClosestTile ( Vector3 worldPos )
	{
		// TODO: More elegant than this
		float closestDist = 999999;
		FloorTile closestTile = null;
		foreach ( FloorTile t in roomManager.allTiles )
		{
			float distFromTile = ( t.transform.position - worldPos ).magnitude;
			if ( distFromTile < closestDist )
			{
				closestDist = distFromTile;
				closestTile = t;
			}
		}

		return closestTile;
	}

	void TestDice()
	{
		string message = "";
		int samplesize = 10000;
		List<int>rolls = new List<int>();
		for ( int i=0;i<samplesize;i++)
		{
			int r = DiceRoller.Roll ( 3,6 );
			rolls.Add ( r );
		}

		List<int> results = new List<int>();
		for ( int i=0;i<19;i++)
		{
			results.Add ( 0 );
		}

		foreach ( int i in rolls )
		{
			results[i]++;
		}

		for ( int i=3;i<19;i++)
		{
			float pct = (float)results[i] / (float)samplesize;
			message += " "+i+": "+(pct*100)+"\n";
		}
		Debug.Log ( message );
	}

	void Update()
	{
//		if ( Input.GetKeyDown(KeyCode.RightShift)) 
//		{ 
//			TestDice();
//		}
		float scrollJump = 64.0f;
		if ( Input.GetKeyDown(KeyCode.UpArrow)) { targetScrollPos += Vector3.up * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.DownArrow)) { targetScrollPos += Vector3.down * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.LeftArrow)) { targetScrollPos += Vector3.left * scrollJump; }
		if ( Input.GetKeyDown(KeyCode.RightArrow)) { targetScrollPos += Vector3.right * scrollJump; }

//		if ( targetScrollPos.x > -mapSize.x/2 ) { targetScrollPos.x = -mapSize.x/2; }
//		if ( targetScrollPos.y > -mapSize.y/2 ) { targetScrollPos.y = -mapSize.y/2; }

		Vector3 delta = targetScrollPos - mapScroller.localPosition;
		delta *= Time.deltaTime * 9.0f;
		mapScroller.localPosition += delta;
	}


	// - - - S C R E E N   N A V I G A T I O N - - - -

	public void Scroll ( Vector2 scroll )
	{
		if ( ControlPanel.Instance.CanDrag == false ) { return; }
		targetScrollPos += (Vector3)scroll * 2f / zoomFactor;
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
		if ( ControlPanel.Instance.CanDrag ) { return; }

		if ( editMode == EditMode.Play )
		{
			// Dragging around tokens
		}
		else
		if ( editMode == EditMode.Room )
		{
			roomManager.InteractWithFloorTile ( tile );
		}
		else
		if ( editMode == EditMode.Deco )
		{
			// manipulate the deco level
		}
	}

	public void DefineNewCharacter()
	{
		// Enable gen and reset it
		CharacterGenerator.Instance.Init();
	}

	public void SpawnNewCharacter ( CharacterStats stats )
	{
		TokenCharacter token = Instantiate ( tokenCharacterTemplate );
		token.gameObject.SetActive ( true );
		token.transform.localScale = Vector3.one;
		// Find a good spot
		FloorTile tile = BestTileForSpawn();
		token.transform.SetParent ( tokenLayer );
		token.transform.position = tile.transform.position;
		token.FindNewHome();
		token.stats = stats;
	}

	FloorTile BestTileForSpawn()
	{
		FloorTile theTile = null;

		Room goodRoom = roomManager.currentRoom;
		if ( goodRoom == null )
		{
			if ( roomManager.rooms.Count > 0 )
			{
				goodRoom = roomManager.rooms[0];
			}
			else
			{
				// No room, just grab the middle tile
				theTile = roomManager.allTiles [ roomManager.allTiles.Count/15];
			}
		}

		if ( goodRoom != null )
		{
			foreach ( FloorTile t in goodRoom.tiles.Values )
			{
				// TODO: Ensure tile isn't occupied, other sanity checks
				theTile = t;
			}
		}
		return theTile;
	}

}