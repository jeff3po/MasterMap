using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpinnerWedge : MonoBehaviour 
{
	public Text label;
	public Text labelShadow;
	public RectTransform pivot;
	public Image circle;
	public int value;

	public void SetPivot ( float angle, float piv )
	{
		// Rotate the whole thing that amount
		transform.localRotation = Quaternion.Euler ( new Vector3 ( 0,0, angle ));
		// And the label pivot halfway
		pivot.localRotation = Quaternion.Euler ( new Vector3 ( 0,0, piv ));
	}

	public void SetColor ( Color c )
	{
		circle.color = c;
	}

	public void SetLabel ( int rawVal )
	{
		value = rawVal + 1;
		label.text = value+"";
		labelShadow.text = value+"";
	}
}
