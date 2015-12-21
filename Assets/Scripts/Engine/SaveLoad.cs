using UnityEngine;
using System.Collections;

public class SaveLoad : MonoBehaviour 
{
	public RoomManager roomManager;

	public void Save()
	{
		roomManager.SaveAll();
	}

	public void Load()
	{
		roomManager.LoadAll();
	}
}
