using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// A room is a collection of floor tiles
/// </summary>
public class Room : Archivable
{
	public bool isVisible = true;

	public Color roomColor = Color.white;

	public void Setup ( int index )
	{
		Name = "Room"+index;
		roomColor = WorldMap.Instance.globalColors[index];
	}

	public override void SetupArchive()
	{
		base.SetupArchive();
		Category = "Rooms";
	}

	public override string UniqueID()
	{
		return Name+ID;
	}

	/// <summary>
	/// All the tiles that make up this room
	/// </summary>
	public Dictionary<string,FloorTile> tiles = new Dictionary<string, FloorTile>();

	public void SetVisible ( bool vis, bool semitransparent = false )
	{
		isVisible = vis;
		foreach ( FloorTile t in tiles.Values )
		{
			if ( semitransparent )
			{
				t.SetAlpha ( 0.2f );
			}
			else
			{
				t.SetVisible ( vis );
				t.SetAlpha ( 1.0f );
			}
		}
	}

	public bool IsAdajcent(FloorTile tile, bool ignoreDoors=false)
	{
		if ( ignoreDoors && tile.IsDoorway ) { return false; }
		foreach ( FloorTile t in tiles.Values )
		{
			int deltaX = Mathf.Abs (t.xPos - tile.xPos);
			int deltaY = Mathf.Abs (t.yPos - tile.yPos);
			if ( deltaX + deltaY <= 1 ) { return true; }
		}
		return false;
	}

	/// <summary>
	/// AxialOnly ignores diagonals 
	/// </summary>
	public List<FloorTile> GetAdjacent ( FloorTile tile, bool axialOnly, bool allowDoors=true )
	{
		List<FloorTile> adjs = new List<FloorTile>();
		int x = tile.xPos;
		int y = tile.yPos;

		adjs.Add ( FindTileByPos ( x-1, y) );
		adjs.Add ( FindTileByPos ( x+1, y) );
		adjs.Add ( FindTileByPos ( x, y-1) );
		adjs.Add ( FindTileByPos ( x, y+1) );

		if ( axialOnly == false )
		{
			adjs.Add ( FindTileByPos ( x-1,y-1));
			adjs.Add ( FindTileByPos ( x+1,y-1));
			adjs.Add ( FindTileByPos ( x-1,y+1));
			adjs.Add ( FindTileByPos ( x+1,y+1));
		}

		List<FloorTile> valids = new List<FloorTile>();
		foreach ( FloorTile t in adjs )
		{
			if ( t == null ) { continue; }
			if (t.IsDoorway && allowDoors == false ) { continue; }
			valids.Add ( t );
		}

		return valids;
	}

	FloorTile FindTileByPos ( int x, int y )
	{
		// Use same furmula as FloorTile uniqueID
		string uniqueID = x+"-"+y;
		FloorTile t = null;
		tiles.TryGetValue ( uniqueID, out t);
		return t;
	}

	public void AddFloorTile ( FloorTile tile )
	{
		if ( tiles.ContainsKey ( tile.UniqueID()) == false )
		{
			tiles.Add ( tile.UniqueID(), tile );
			// And attach tp this room
			tile.AddToRoom ( this );
		}
	}

	public void RemoveFloorTile ( FloorTile tile )
	{
		tiles.Remove ( tile.UniqueID() );
		// And update room
		tile.RemoveFromRoom ( this );
	}



	// - - - A R C H I V E - - - - -

	/// <summary>
	/// Used only during Init to carry IDs across into postInit
	/// </summary>
	List<string> tileIDs = new List<string>();

	public override void PostInit()
	{
		// Find matching tiles in tile list
		foreach ( string s in tileIDs )
		{
			AddFloorTile ( RoomManager.Instance.FindTileByID ( s ) );
		}
	}

	public override void Init ( ref JSONNode data, int i )
	{
		tileIDs.Clear();

		base.Init ( ref data, i );
		int tileCount = data [ Category ] [ i ] [ "tileCount" ].AsInt;
		for ( int tc=0;tc<tileCount;tc++)
		{
			tileIDs.Add ( data [ Category ] [ i ] [ "tile" ] [ tc ] );
		}
	}


	public override void Export(ref JSONNode data, int i)
	{
		base.Export (ref data, i);

		int tileCount = 0;
		string output = "Room "+i+" Tiles: ";
		foreach ( FloorTile t in tiles.Values )
		{
			data [ Category ] [ i ] [ "tile" ] [tileCount ] = t.UniqueID();
			output += " "+t.UniqueID();
			tileCount++;
		}
//		Debug.Log ( output );
		data [ Category ] [ i ] [ "tileCount" ].AsInt = tileCount;
	}
}
