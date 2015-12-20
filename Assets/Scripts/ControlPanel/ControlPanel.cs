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
	public DecorationPanel decorationPanel;

	public InsertDecoration decorationMode = InsertDecoration.None;
	void Start()
	{
		Application.targetFrameRate = 30;
		QualitySettings.vSyncCount = -1;

		toggleLockDrag.isOn = _canDrag;
		List<string>optionNames = new List<string> ( System.Enum.GetNames ( typeof(WorldMap.EditMode)) );
		editModeDropdown.AddOptions( optionNames );
		Open ( true );
		_controlPanelClosed = Vector2.zero;
		_controlPanelClosed.x -= frame.rectTransform.sizeDelta.x - OpenButton_ToOpen.image.rectTransform.sizeDelta.x;

		drawingPanel.AddButton ( "Floors", DrawRooms );
		drawingPanel.AddButton ( "Doors", SetDoors );

		decorationPanel.AddButton ( "No deco", AddDeco_None );
		decorationPanel.AddButton ( "Add pillar", AddDeco_Pillar );

		map.SetEditMode ( WorldMap.EditMode.Room );
	}

	public void ChangePanel()
	{
		string option = editModeDropdown.captionText.text;
		if ( option == "Decoration")
		{
			map.SetEditMode ( WorldMap.EditMode.Decoration );
			ShowPanel ( WorldMap.EditMode.Decoration );
		}
		else
		if ( option == "Room")
		{
			map.SetEditMode ( WorldMap.EditMode.Room );
			ShowPanel ( WorldMap.EditMode.Room );
		}
	}

	public void ShowPanel ( WorldMap.EditMode mode )
	{
		drawingPanel.gameObject.SetActive ( false );
		decorationPanel.gameObject.SetActive ( false );
		// TODO: Hide all other panels

		if ( mode == WorldMap.EditMode.Room )
		{
			drawingPanel.gameObject.SetActive ( true );
		}
		else
		if ( mode == WorldMap.EditMode.Decoration )
		{
			decorationPanel.gameObject.SetActive ( true );
		}
	}

	public enum InsertDecoration
	{
		None,
		Pillar
	}

	void AddDeco_Pillar()
	{
		decorationMode = InsertDecoration.Pillar;
	}

	void AddDeco_None()
	{
		decorationMode = InsertDecoration.None;
	}

	void DrawRooms()
	{
		drawingPanelMode = DrawingPanelMode.Floor;
	}

	void SetDoors()
	{
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
