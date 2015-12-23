using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatField : MonoBehaviour 
{
	public Text statName;
	public Text statValue;

	public void Setup ( string nm, string val )
	{
		statName.text = nm;
		statValue.text = val;
	}
}
