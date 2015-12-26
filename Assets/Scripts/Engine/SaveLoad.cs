using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SaveLoad : MonoBehaviour 
{
	public RectTransform interfacePanel;
	public InputField inputField;

	public enum SaveLoadState
	{
		Undefined,
		Save,
		Load
	}

	SaveLoadState state;

	void Start()
	{
		interfacePanel.gameObject.SetActive ( false );
	}

	public void Save()
	{
		state = SaveLoadState.Save;
		ShowInterface();
	}

	public void Load()
	{
		state = SaveLoadState.Load;
		ShowInterface();
	}

	void ShowInterface()
	{
		interfacePanel.gameObject.SetActive ( true );
	}

	public void OK()
	{
		if ( state == SaveLoadState.Load )
		{
			LoadByName(inputField.text);
		}
		else
		if ( state == SaveLoadState.Save )
		{
			SaveByName(inputField.text);
		}
		interfacePanel.gameObject.SetActive ( false );
	}

	public void Cancel()
	{
		interfacePanel.gameObject.SetActive ( false );	
	}

	void SaveByName ( string nm )
	{
		RoomManager.Instance.SaveByName(nm);
	}

	void LoadByName ( string nm )
	{
		RoomManager.Instance.LoadByName(nm);
	}


}
