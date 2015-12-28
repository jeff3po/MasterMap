using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Holding the value field of the ability field brings up a drag interface to quickly set value
/// </summary>
public class CharGen_AbilitySlider : MonoBehaviour, IDragHandler
{
	public Image ring;
	public Image ringcore;

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
	}

	float dragDistance = 0;

	/// <summary>
	/// How many pixels to drag to change the value one unit
	/// </summary>
	public float dragInterval = 10;

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

		dragDistance += delta.x + delta.y;

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

		SetDisplayedValue();
	}
}
