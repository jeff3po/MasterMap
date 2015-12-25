using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Holding the value field of the ability field brings up a drag interface to quickly set value
/// </summary>
public class CharGen_AbilitySlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public Text value;
	public Image slider;
	public Image valueTemplate;
	public RectTransform centerMark;
	string[] strings;
	[HideInInspector]
	public bool usesStrings = false;

	public bool horizontal = true;

	int minValue = 0;
	int currentValue = 10;
	string currentString = "";

	List<Image> valueTabs = new List<Image>();

	void SetDisplayedValue()
	{
		string textToDisplay = currentString;

		if ( usesStrings == false )
		{
			currentValue = int.Parse ( textToDisplay );

			if ( minValue < 0 )
			{
				// Since we care about negative values, add + to 0 so there's always a sign
				if ( currentValue == 0 )
				{
					textToDisplay = "+"+textToDisplay;
				}
			}
		}
		value.text = textToDisplay;
	}

	public void Setup ( int initValue, int min, int max, int step, string[] str=null ) 
	{
		if ( valueTabs.Count > 0 ) { return; }
		minValue = min;
		currentValue = initValue;
		currentString = initValue+"";

		if ( str != null )
		{
			usesStrings = true;
			strings = str;
			currentString = strings [ initValue ];
		}

		valueTemplate.gameObject.SetActive ( false );
		slider.gameObject.SetActive ( false );

		SetDisplayedValue();

		if ( valueTabs.Count == 0 )
		{
			// Array the numbers. Inclusive of max value except when using a string list 
			int count = max+1;
			if ( usesStrings ) { count = strings.Length; }
			for ( int i=min;i<count;i+=step)
			{
				Image val = Instantiate ( valueTemplate );
				val.gameObject.SetActive ( true );
				val.transform.SetParent ( valueTemplate.transform.parent );
				val.transform.localScale = Vector3.one;
				string numberToDisplay = ""+i;
				if ( usesStrings ) { numberToDisplay = strings[i]; }
				if ( min < 0 )
				{
					// We care about negative values, so add + 
					if ( i > 0 )
					{
						numberToDisplay = "+"+i;
					}
				}
				val.GetComponentInChildren<Text>().text = numberToDisplay;
				valueTabs.Add ( val );
			}
		}
	}

	float updateDistance = 0;

	public void OnDrag ( PointerEventData data)
	{
		if ( updateDistance == 0 )
		{
			updateDistance = value.rectTransform.sizeDelta.x *0.1f;
		}

		Vector2 delta = data.delta;
		if ( horizontal )
		{
			delta.y = 0;
		}
		else
		{
			delta.x = 0;
		}
		slider.transform.Translate ( delta );

		updatedDrag += delta;

		float updateDist = updatedDrag.x;
		if ( !horizontal) { updateDist = updatedDrag.y; }

		if ( Mathf.Abs ( updateDist ) > updateDistance )
		{
			updatedDrag = Vector3.zero;
			// Each time it moves, find the closest number on the strip to the unerlying value field. 
			float closestDist = 99999;
			foreach ( Image tab in valueTabs )
			{
				float dist = (tab.transform.position - centerMark.position).magnitude;

				if ( dist < closestDist )
				{
					closestDist = dist;
					closestTab = tab;
				}
			}
			RefreshTabs();
		}
	}

	Vector2 updatedDrag = Vector2.zero;

	Image closestTab = null;

	void RefreshTabs()
	{
		foreach ( Image tab in valueTabs )
		{
			if ( tab == closestTab )
			{
				tab.DOKill();
				tab.DOFade ( 1.0f, 0.1f );

				currentString = tab.GetComponentInChildren<Text>().text;

				SetDisplayedValue();
			}
			else
			{
				tab.DOFade ( 0.1f, 0.4f );
			}
		}
	}

	public void OnPointerDown ( PointerEventData data )
	{
		if ( slider.gameObject.activeSelf ) { return; }
		slider.gameObject.SetActive ( true );
		RefreshTabs();
	}

	public void OnPointerUp ( PointerEventData data )
	{
		slider.gameObject.SetActive ( false );
	}
}
