using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SpinnerPanel : SingletonMonoBehaviour<SpinnerPanel> 
{
	public Spinner spinnerTemplate;
	public Text summaryText;

	List<Spinner> spinners = new List<Spinner>();
	System.Action<int,bool> finalCallback;

	int remainingSpins = 0;

	List<bool> successResults = new List<bool>();
	int total = 0;

	public void SpinTheWheel ( Dice dice, System.Action<int,bool> callback, int target, string action )
	{
		gameObject.SetActive( true );
		foreach ( Spinner s in spinners ) { Destroy(s.gameObject); } spinners.Clear();
		successResults.Clear();
		total = 0;

		finalCallback = callback;
		for ( int i=0;i<dice.numberToRoll;i++ )
		{
			Spinner newSpin = Instantiate ( spinnerTemplate );
			newSpin.gameObject.SetActive ( true );
			spinners.Add (newSpin);
			newSpin.transform.SetParent ( spinnerTemplate.transform.parent );
			newSpin.transform.localScale = Vector3.one;
			newSpin.SpinTheWheel ( dice, Result, target, action );
		}

		// Count down the returned spin results
		remainingSpins = dice.numberToRoll;

		summaryText.text = "Spin "+action;
	}

	void Result ( int i, bool succ )
	{
		remainingSpins --;
		successResults.Add ( succ );
		total += i;

		if ( remainingSpins <= 0 )
		{
			summaryText.text = "Final result: "+total;

			bool realSuccess = true;
			foreach ( bool b in successResults )
			{
				if ( b == false ) { realSuccess = false; }
			}

			finalCallback ( total, realSuccess );
			foreach ( Spinner s in spinners ) { Destroy(s.gameObject); } spinners.Clear();
			successResults.Clear();
			gameObject.SetActive ( false );
		}
		else
		{
			summaryText.text = "Result so far: "+total;
		}
	}

	public void Cancel()
	{
		gameObject.SetActive ( false );
	}

}
