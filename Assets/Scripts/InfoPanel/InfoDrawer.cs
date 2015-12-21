using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Base class for info panel that folds out of top of screen.
/// </summary>
public class InfoDrawer : MonoBehaviour 
{
	/// <summary>
	/// Main title at bottom of drawer, always visible (even when rolled up)
	/// </summary>
	public Text label;

	/// <summary>
	/// Basic text field making up the bulk of the base drawer
	/// </summary>
	public Text message;

	public void Setup ( string lbl, string msg )
	{
		label.text = lbl;
		message.text = msg;
	}
}
