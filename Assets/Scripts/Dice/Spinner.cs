using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Spinner : MonoBehaviour 
{
	// TODO: Support multiple spinners at once

	public Text rollValue;
	public Text bonusText;
	public Text resultText;

	public RectTransform hub;

	public SpinnerWedge wedgeTemplate;

	public Image marker;

	int finalValue = 0;
	int targetValue = -999;

	SpinnerWedge prevClosest = null;

	int modifier = 0;

	public enum SpinState
	{
		Unspun,
		Windup, // TODO Allow dragging with finger to make it feel like a tangible spin in addition to single tap spin
		Spinning,
		Spun,
		Delivered // Value sent back to spin manager
	}

	SpinState spinState = SpinState.Unspun;

	public int fullSpins;
	float targetSpin = 0;
	float currentSpin = 0;
	public float spinRate;
	List<SpinnerWedge> wedges = new List<SpinnerWedge>();

	System.Action<int,bool> rollCallback;

	void Start()
	{
		wedgeTemplate.gameObject.SetActive ( false );
	}

	public void SpinTheWheel ( Dice dice, System.Action<int,bool> callback, int target, string action )
	{
		Setup ( dice, callback, target, action );
	}
		
	public void Setup ( Dice dice, System.Action<int,bool> callback, int target, string act )
	{
		foreach (SpinnerWedge w in wedges ) { Destroy ( w.gameObject ); }
		wedges.Clear();
		finalValue = 0;
		targetValue = -999;
		prevClosest = null;
		modifier = 0;
		bonusText.text = "";
		resultText.text = "";
		rollValue.text = "Spin";
		targetSpin = 0;
		currentSpin = 0;
		spinState = SpinState.Unspun;

		gameObject.SetActive ( true );
		targetValue = target;

		int faceCount = dice.numberOfFaces;
		modifier = dice.modifier;

		if ( dice.modifier != 0 )
		{
			string bonus = "";
			if ( dice.modifier > 0 )
			{
				bonus += "+";
			}

			bonus += dice.modifier;
			bonusText.text = bonus+" "+act;
		}

		wedgeTemplate.gameObject.SetActive ( false );
		rollCallback = callback;

		// Distribute face list in non-linear fashion
		List<int> facelist = new List<int>();

		for ( int i=0;i<faceCount;i++ )
		{
			facelist.Add ( i );
		}

		// Do it twice for halving then quartering
		List<int> distributedFaces = _distributeFaces ( _distributeFaces ( facelist ) );

		// Arc for each wedge
		float interval = 1.0f / (float)faceCount;

		for ( int i=0;i<distributedFaces.Count;i++)
		{
			int val = distributedFaces[i];

			SpinnerWedge wedge = Instantiate ( wedgeTemplate );
			wedge.gameObject.SetActive ( true );
			wedge.transform.SetParent ( hub );
			wedge.transform.localPosition = Vector3.zero;
			wedge.transform.localScale = Vector3.one;
			wedge.circle.fillAmount = interval;
			float angle = i*interval * -360.0f;
			float pivot = interval/2 * -360.0F;
			wedge.SetPivot ( angle, pivot );
//			Color c = Color.white * (float)val/(float)faceCount;
//			c.a = 1.0f;
			Color c = Color.blue;
			if ( i%2==0) { c = Color.gray; }
			wedge.SetColor ( c );
			wedge.SetLabel ( val );
			wedge.name = val+"";
			wedges.Add ( wedge );
		}
	}

	List<int> _distributeFaces ( List<int> facelist )
	{
		// Shuffle to interspere high/low values
		List<int> distributedFaces = new List<int>();

		while ( facelist.Count > 0 )
		{
			int index = 0;
			if ( (facelist.Count % 2) == 0 )
			{
				// Take from end of list
				index = 0;
			}
			else
			{
				// Take from end of list
				index = facelist.Count-1;
			}

			int val = facelist[index];

			distributedFaces.Add ( val );
			facelist.RemoveAt ( index );
		}

		return distributedFaces;
	}

	public void SpinIt()
	{
		if ( spinState == SpinState.Delivered ) { return; }

		if ( spinState == SpinState.Spun )
		{
			rollCallback ( finalValue, finalValue>=targetValue );
			// Hide self
			gameObject.SetActive ( false );
			// Callback should close when all rolling is done
			return;
		}

		if ( spinState != SpinState.Unspun ) { return; }

		spinState = SpinState.Spinning;

		targetSpin = (fullSpins * 360) + Random.Range ( 0,360 );
		currentSpin = 0;
	}

	void Update()
	{
		if ( spinState == SpinState.Spinning )
		{
			float delta = targetSpin - currentSpin;
			delta *= Time.deltaTime * spinRate;

			if ( Mathf.Abs ( delta ) < 0.1f )
			{
				spinState = SpinState.Spun;
				if ( targetValue != -999 )
				{
					if ( finalValue >= targetValue )
					{
						resultText.text = "Success!";
					}
					else
					{
						resultText.text = "Fail!";
					}
				}
				else
				{
					// No target, so display total result if modified
					if ( modifier != 0 )
					{
						resultText.text = "Total: "+finalValue;
					}
				}
			}
			else
			{
				currentSpin += delta;
			}

//			Debug.Log ( "currentSpin: "+currentSpin+"    targetSpin: "+targetSpin );
			float newAngle = currentSpin;
			while (newAngle < -360 ) { newAngle += 360; }

			hub.transform.localRotation = Quaternion.Euler ( new Vector3 ( 0,0,newAngle ) );

			// Update value
			float closestMark = 9999999;
			SpinnerWedge closestWedge = null;
			foreach ( SpinnerWedge w in wedges )
			{
				float dist = (w.label.transform.position - marker.transform.position).magnitude;
				if ( dist < closestMark )
				{
					closestMark = dist;
					closestWedge = w;
				}
			}

			if ( prevClosest != closestWedge )
			{
				finalValue = closestWedge.value + modifier;
				prevClosest = closestWedge;
				rollValue.text = finalValue+"";
				rollValue.rectTransform.DOKill();
				rollValue.rectTransform.localScale = Vector3.one;
				rollValue.rectTransform.DOPunchScale ( Vector3.one * 0.8f, 0.4f, 2, 0.8f );
			}
		}
	}
}
