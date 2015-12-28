using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Holding the value field of the ability field brings up a drag interface to quickly set value
/// </summary>
public class CharGen_AbilitySlider : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Image ring;
	public Image ringcore;

	public Image trailBG;
	public Text trailMinus2;
	public Text trailMinus1;
	public Text trailCurrent;
	public Text trailPlus1;
	public Text trailPlus2;

	Text modifierTextField;

	public Text value;
	string[] strings;
	[HideInInspector]
	public bool usesStrings = false;

	public bool horizontal = true;

	float valueRange = 1;
	int minValue = 0;
	int maxValue = 0;
	int step = 1;

	[HideInInspector]
	public int currentValue = 10;

	bool showTrail = false;

	public void OnPointerDown(PointerEventData data)
	{
		ShowTrails ( true );
	}

	public void OnPointerUp(PointerEventData data)
	{
		ShowTrails ( false );
	}

	void ShowTrails ( bool show )
	{
		if ( trailBG == null ) { return; }
		showTrail = show;
		trailBG.gameObject.SetActive ( show );
	}

	public void SetDisplayedValue()
	{
		string textToDisplay = currentValue+"";

		if ( usesStrings == false )
		{
			if ( minValue < 0 )
			{
				// Since we care about negative values, add + to 0 so there's always a sign
				if ( currentValue >= 0 )
				{
					textToDisplay = "+"+textToDisplay;
				}
			}
		}
		else
		{
			textToDisplay = strings[ Mathf.Min ( currentValue, strings.Length-1 ) ];
		}

		value.text = textToDisplay;

		if ( ring != null )
		{
			float pct = (float)currentValue/valueRange;
			ring.fillAmount = pct;
			ringcore.fillAmount = pct;
			Color c = ring.color;
			c.a = pct;
			ring.color = c;
		}

		if ( modifierTextField != null )
		{
			int delta = currentValue - 10;
			delta /= 2;

			string disp = delta+"";
			if ( delta >= 0 ) { disp = "+"+disp; }
			modifierTextField.text = disp;
		}

		int prev2 = currentValue-2;
		int prev1 = currentValue-1;
		int next1 = currentValue+1;
		int next2 = currentValue+2;

		if ( trailMinus2 != null )
		{
			SetTrail ( trailMinus2, prev2 );
			SetTrail ( trailMinus1, prev1 );
			SetTrail ( trailCurrent, currentValue );
			SetTrail ( trailPlus1, next1 );
			SetTrail ( trailPlus2, next2 );
		}
	}

	void SetTrail ( Text trail, int index )
	{
		if ( index < 0 || index >= strings.Length )
		{
			trail.text = "";
		}
		else
		{
			trail.text = strings [ index ];
		}
	}

	public void Setup ( int initValue, int min, int max, int stp, string[] str=null, Text mod=null ) 
	{
		modifierTextField = mod;
		step = stp;
		minValue = min;
		maxValue = max;
		currentValue = initValue;
		valueRange = max - min;

		if ( str != null )
		{
			usesStrings = true;
			strings = str;
		}

		SetDisplayedValue();

		// Full range left or right from center
		dragInterval = 0.5f/valueRange;

		ShowTrails ( false );
	}

	float dragDistance = 0;

	/// <summary>
	/// How many pixels to drag to change the value one unit
	/// </summary>
	float dragInterval = 1;

	public void OnDrag ( PointerEventData data)
	{
		Vector2 delta = data.delta;
		if ( horizontal )
		{
			delta.y = 0;
		}
		else
		{
			delta.x = 0;
		}

		float scrn = Screen.width;
		if ( horizontal )
		{
			scrn = Screen.height;
		}

		dragDistance += (delta.x + delta.y) / scrn;

		while ( dragDistance > dragInterval )
		{
			dragDistance -= dragInterval;
			currentValue += step;
		}

		while ( dragDistance < -dragInterval )
		{
			dragDistance += dragInterval;
			currentValue -= step;
		}

		if ( currentValue < minValue ) { currentValue = minValue; }
		if ( currentValue > maxValue) { currentValue = maxValue; }

		if ( usesStrings )
		{
			if ( currentValue > maxValue-1) { currentValue = maxValue-1; }
		}

		SetDisplayedValue();
	}
}
