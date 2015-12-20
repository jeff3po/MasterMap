using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A room is a collection of floor tiles
/// </summary>
[System.Serializable]
public class Room
{
	public bool isVisible = true;

	public Color roomColor = Color.white;
	public string name;
	WorldMap worldMap;

	public Room ( int index, WorldMap map )
	{
		name = "Room"+index;
		roomColor = map.globalColors[index];
		worldMap = map;
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
		if ( ignoreDoors && tile.isDoor ) { return false; }
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
			if ( t.isDoor && allowDoors==false ) { continue; }
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
}
