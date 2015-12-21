using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// A room is a collection of floor tiles
/// </summary>
public class Room : Archivable
{
	public RoomManager roomManager;

	public bool isVisible = true;

	public Color roomColor = Color.white;

	public void Setup ( int index, WorldMap map )
	{
		Name = "Room"+index;
		roomColor = map.globalColors[index];
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
	public List<FloorTile> tiles = new List<FloorTile>();

	public void SetVisible ( bool vis, bool semitransparent = false )
	{
		isVisible = vis;
		foreach ( FloorTile t in tiles )
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
		foreach ( FloorTile t in tiles )
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
		foreach (FloorTile t in tiles )
		{
			if ( t == tile ) { continue; }
			if ( t.IsDoorway && allowDoors==false ) { continue; }
			int deltaX = Mathf.Abs (t.xPos - tile.xPos);
			int deltaY = Mathf.Abs (t.yPos - tile.yPos);
			if ( deltaX + deltaY <= 1 ) { adjs.Add ( t ); }
		}
		return adjs;
	}

	public void AddFloorTile ( FloorTile tile )
	{
		if ( tiles.Contains (tile) == false )
		{
			tiles.Add ( tile );
			tile.AddToRoom(this);
		}
	}

	public void RemoveFloorTile ( FloorTile tile )
	{
		tiles.Remove ( tile );
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
			AddFloorTile ( roomManager.FindTileByID ( s ) );
		}
	}

	public override void Init(ref JSONNode data, int i )
	{
		tileIDs.Clear();

		base.Init (ref data, i );
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
		foreach ( FloorTile t in tiles )
		{
			data [ Category ] [ i ] [ "tile" ] [tileCount ] = t.UniqueID();
			tileCount++;
		}
		data [ Category ] [ i ] [ "tileCount" ].AsInt = tileCount;
	}
}
