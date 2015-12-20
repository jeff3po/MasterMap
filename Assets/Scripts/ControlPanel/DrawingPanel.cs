using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DrawingPanel : ControlPanelSubpanel
{
	public WorldMap map;
	public Dropdown roomPicker;
	public InputField roomName;

	public void AddRoom()
	{
		map.AddRoom();

		ResetPicker();
	}

	void ResetPicker(string overridename="")
	{
		// Reset picker
		roomPicker.ClearOptions();
		List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
		data.Add ( new Dropdown.OptionData ( "None" ) );
		string current = "";
		foreach ( Room r in map.rooms )
		{
			data.Add ( new Dropdown.OptionData ( r.name ) );
			current = r.name;
		}

		roomPicker.AddOptions ( data );

		int lastIndex = roomPicker.options.Count-1;
		if ( overridename == "" ) 
		{ 
			overridename = current;
		}
		else
		{
			for ( int i=0;i<roomPicker.options.Count;i++)
			{
				if ( roomPicker.options[i].text == overridename )
				{
					lastIndex = i;
					break;
				}
			}
		}

		roomPicker.value = lastIndex;
		roomPicker.captionText.text = overridename;
		roomName.text = overridename;
	}

	public void ChangeRoomName()
	{
		if ( map.SetCurrentRoomName ( roomName.text ) )
		{
			ResetPicker(roomName.text);
		}
	}

	public void RoomUp()
	{
		int newRoomIndex = map.RoomShift(1);
		newRoomIndex++;
		roomName.text = roomPicker.options[newRoomIndex].text;
	}

	public void RoomDown()
	{
		int newRoomIndex = map.RoomShift(-1);
		newRoomIndex++;
		roomName.text = roomPicker.options[newRoomIndex].text;
	}

	public void PickRoom()
	{
		// First entry is hardcoded as None
		int index = roomPicker.value - 1;
		if ( index == -1 )
		{
			roomName.text = "";
			map.SetCurrentRoom ( null );
		}
		else
		{
			roomName.text = map.SetCurrentRoom(index).name;
		}
	}
}
