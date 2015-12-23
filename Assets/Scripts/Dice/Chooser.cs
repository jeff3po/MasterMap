using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Chooser : MonoBehaviour 
{
	List<ChooserButton> choices = new List<ChooserButton>();
	System.Action <string> actionCallback;

	public ChooserButton buttonTemplate;
	public void AddButton ( string label, System.Action <string>callback )
	{
		gameObject.SetActive ( true );

		ChooserButton newButton = Instantiate ( buttonTemplate );
		newButton.gameObject.SetActive ( true );
		newButton.transform.SetParent ( buttonTemplate.transform.parent );
		newButton.transform.localScale = Vector3.one;
		newButton.label.text = label;
		choices.Add ( newButton );

		actionCallback = callback;
	}

	public void MakeChoice ( string choice )
	{
		actionCallback ( choice );
		foreach ( ChooserButton but in choices ) { Destroy(but.gameObject); }
		choices.Clear();
		gameObject.SetActive ( false );
	}
}
