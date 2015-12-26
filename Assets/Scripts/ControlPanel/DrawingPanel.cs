using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DrawingPanel : ControlPanelSubpanel
{
	public Dropdown roomPicker;
	public InputField roomName;

	public void AddRoom()
	{
		WorldMap.Instance.AddRoom();

		ResetPicker();
	}

	public void ResetPicker(string overridename="")
	{
		// Reset picker
		roomPicker.ClearOptions();
		List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();
		data.Add ( new Dropdown.OptionData ( "None" ) );
		string current = "";
		foreach ( Room r in RoomManager.Instance.rooms )
		{
			data.Add ( new Dropdown.OptionData ( r.Name ) );
			current = r.Name;
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
		if ( WorldMap.Instance.SetCurrentRoomName ( roomName.text ) )
		{
			ResetPicker(roomName.text);
		}
	}

	public void RoomUp()
	{
		int newRoomIndex = WorldMap.Instance.RoomShift(1);
		newRoomIndex++;
		roomName.text = roomPicker.options[newRoomIndex].text;
	}

	public void RoomDown()
	{
		int newRoomIndex = WorldMap.Instance.RoomShift(-1);
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
			WorldMap.Instance.SetCurrentRoom ( null );
		}
		else
		{
			roomName.text = WorldMap.Instance.SetCurrentRoom(index).Name;
		}
	}
}
