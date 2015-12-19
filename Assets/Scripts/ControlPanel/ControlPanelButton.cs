using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlPanelButton : MonoBehaviour 
{
	public Button button;
	public Text label;
	System.Action callback;

	public void Setup ( string nm, System.Action cb )
	{
		label.text = nm;
		callback = cb;
	}

	public void Click()
	{
		if ( callback != null )
		{
			callback();
		}
	}
}
