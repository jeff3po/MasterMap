using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharGen_Ability : CharGen_Stat 
{
	public Text modifier;

	public void SetupAbility ( string nm, int val, int min, int max, int step )
	{
		abilityValue = val;
		title.text = nm;
		value.Setup ( val, min, max, step, null, modifier );
	}
}
