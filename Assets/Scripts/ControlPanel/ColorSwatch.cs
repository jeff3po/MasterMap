using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ColorSwatch : MonoBehaviour, IPointerClickHandler
{
	public Image image;

	public void OnPointerClick(PointerEventData data)
	{
		ControlPanel.Instance.SetCurrentColor ( image.color );
//		Debug.Log ( "Color "+image.color );
	}
}
