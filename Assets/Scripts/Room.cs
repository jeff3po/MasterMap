using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A room is a collection of floor tiles
/// </summary>
[System.Serializable]
public class Room
{
	public Color roomColor = Color.white;
	public string name;
	public WorldMap worldMap;

	public Room ( int index, WorldMap map )
	{
		worldMap = map;
		name = "Room"+index;
		roomColor = map.globalColors[index];
	}

	public List<FloorTile> tiles = new List<FloorTile>();

	public void RemoveFloorTile ( FloorTile tile )
	{
		tiles.Remove ( tile );
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
	public List<FloorTile> GetAdjacent ( FloorTile tile, bool axialOnly )
	{
		List<FloorTile> adjs = new List<FloorTile>();
		foreach (FloorTile t in tiles )
		{
			if ( t == tile ) { continue; }
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
			tile.SetColor ( roomColor );
		}
	}
}
