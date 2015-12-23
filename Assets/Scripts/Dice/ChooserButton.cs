using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooserButton : MonoBehaviour
{
	public Chooser chooser;
	public Text label;

	public void OnClick()
	{
		chooser.MakeChoice ( label.text );
	}
}
