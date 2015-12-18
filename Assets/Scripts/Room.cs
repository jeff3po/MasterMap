using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A room is a collection of floor tiles
/// </summary>
public class Room
{
	public Color roomColor = Color.white;
	public string name;
	WorldMap worldMap;

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

	public bool IsAdajcent(FloorTile tile)
	{
		foreach ( FloorTile t in tiles )
		{
			int deltaX = Mathf.Abs (t.xPos - tile.xPos);
			int deltaY = Mathf.Abs (t.yPos - tile.yPos);
			if ( deltaX + deltaY <= 1 ) { return true; }
		}
		return false;
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
