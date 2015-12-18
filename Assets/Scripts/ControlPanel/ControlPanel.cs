using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ControlPanel : MonoBehaviour 
{
	public Toggle toggleLockDrag;

	public Image frame;

	bool _canDrag = true;

	public bool _isOpen = true;

	public Button OpenButton_ToClose;
	public Button OpenButton_ToOpen;

	public Vector2 _controlPanelClosed;

	public Color currentColor = Color.white;

	void Start()
	{
		Open ( true );
		_controlPanelClosed = Vector2.zero;
		_controlPanelClosed.x -= frame.rectTransform.sizeDelta.x - OpenButton_ToOpen.image.rectTransform.sizeDelta.x;
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
