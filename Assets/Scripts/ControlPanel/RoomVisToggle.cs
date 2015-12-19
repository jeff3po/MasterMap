using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomVisToggle : MonoBehaviour 
{
	int index;
	public Toggle toggle;
	public Image bg;
	public WorldMap map;

	public bool isIndex ( int i )
	{
		return index == i;
	}

	public void Setup ( int i, Color c )
	{
		index = i;
		bg.color = c;
	}

	public void Toggled()
	{
		map.SetRoomVisibility ( index, toggle.isOn );
	}
}
