using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ColorSwatch : MonoBehaviour, IPointerClickHandler
{
	public Image image;
	public ControlPanel controlPanel;

	public void OnPointerClick(PointerEventData data)
	{
		controlPanel.SetCurrentColor ( image.color );
		Debug.Log ( "Color "+image.color );
	}
}
