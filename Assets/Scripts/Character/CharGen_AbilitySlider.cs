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

	int currentValue = 10;

	List<Image> valueTabs = new List<Image>();

	public void Setup ( int initValue, int min, int max, int step ) 
	{
		currentValue = initValue;

		valueTemplate.gameObject.SetActive ( false );
		slider.gameObject.SetActive ( false );
		currentValue = int.Parse ( value.text );

		if ( valueTabs.Count == 0 )
		{
			// Array the numbers
			for ( int i=min;i<=max;i+=step)
			{
				Image val = Instantiate ( valueTemplate );
				val.gameObject.SetActive ( true );
				val.transform.SetParent ( valueTemplate.transform.parent );
				val.transform.localScale = Vector3.one;
				val.GetComponentInChildren<Text>().text = i+"";
				valueTabs.Add ( val );
			}
		}
	}

	float updateDistance = 0;

	public void OnDrag ( PointerEventData data)
	{
		if ( updateDistance == 0 )
		{
			updateDistance = value.rectTransform.sizeDelta.x *0.25f;
		}

		Vector2 delta = data.delta;
		delta.y = 0;
		slider.transform.Translate ( delta );

		updatedDrag += delta;

		if ( Mathf.Abs ( updatedDrag.x ) > updateDistance )
		{
			updatedDrag = Vector3.zero;
			// Each time it moves, find the closest number on the strip to the unerlying value field. 
			float closestDist = 99999;
			foreach ( Image tab in valueTabs )
			{
				float dist = (tab.transform.position - value.transform.position + (Vector3.left * tab.rectTransform.sizeDelta.x*0.25f) ).magnitude;

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
				currentValue = int.Parse ( tab.GetComponentInChildren<Text>().text );
				value.text = currentValue+"";
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
