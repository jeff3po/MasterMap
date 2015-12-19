using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ControlPanel : MonoBehaviour 
{
	public enum DrawingPanelMode
	{
		Floor,
		Door
	}

	public DrawingPanelMode drawingPanelMode = DrawingPanelMode.Floor;

	public WorldMap map;

	public RoomVisibilityPanel visPanel;

	public Toggle toggleLockDrag;

	public Dropdown editModeDropdown;

	public Image frame;

	bool _canDrag = false;

	public bool _isOpen = true;

	public Button OpenButton_ToClose;
	public Button OpenButton_ToOpen;

	public Vector2 _controlPanelClosed;

	public Color currentColor = Color.white;

	public DrawingPanel drawingPanel;

	void Start()
	{
		List<string>optionNames = new List<string> ( System.Enum.GetNames ( typeof(WorldMap.EditMode)) );
		editModeDropdown.AddOptions( optionNames );
		Open ( true );
		_controlPanelClosed = Vector2.zero;
		_controlPanelClosed.x -= frame.rectTransform.sizeDelta.x - OpenButton_ToOpen.image.rectTransform.sizeDelta.x;
		drawingPanel.AddButton ( "Draw Floors", DrawRooms );
		drawingPanel.AddButton ( "Set Doors", SetDoors );
	}

	void DrawRooms()
	{
		Debug.Log ( "Draw rooms");
		drawingPanelMode = DrawingPanelMode.Floor;
	}

	void SetDoors()
	{
		Debug.Log ( "Set Doors");
		drawingPanelMode = DrawingPanelMode.Door;
	}

	public void Open ( bool open )
	{
		_isOpen = open;
		OpenButton_ToOpen.gameObject.SetActive ( !open );
		OpenButton_ToClose.gameObject.SetActive ( open );

		Vector2 dest = Vector2.zero;
		if ( !open ) { dest = _controlPanelClosed; }
		frame.rectTransform.localPosition = dest;
	}

	public bool CanDrag
	{
		get { return _canDrag; }
	}

	public void ToggleLockDrag()
	{
		_canDrag = toggleLockDrag.isOn;
	}

	public void SetCurrentColor ( Color c )
	{
		currentColor = c;
	}
}
