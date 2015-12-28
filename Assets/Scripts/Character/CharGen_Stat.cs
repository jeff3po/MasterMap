using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharGen_Stat : MonoBehaviour 
{
	public Text title;
	public CharGen_AbilitySlider value;
	public int abilityValue=10;

	public void Setup ( string nm, int val, int min, int max, int step )
	{
		abilityValue = val;
		title.text = nm;
		value.Setup ( val, min, max, step, null );
	}
}
