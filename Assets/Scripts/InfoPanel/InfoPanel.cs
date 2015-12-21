using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bar at top of screen
/// </summary>
public class InfoPanel : MonoBehaviour 
{
	public InfoDrawer infoDrawerTemplate;
	Dictionary<string,InfoDrawer> infoDrawers = new Dictionary<string, InfoDrawer>();

	void Start()
	{
		infoDrawerTemplate.gameObject.SetActive ( false );
	}

	public void AddDrawer ( string ID, string label, string message )
	{
		InfoDrawer drawer = null;

		if ( infoDrawers.ContainsKey ( ID ) )
		{
			// Replace exisiting drawer's values
			infoDrawers.TryGetValue ( ID, out drawer );
			infoDrawers.Remove ( ID );
		}
		else
		{
			// Make a new one from scratch
			drawer = Instantiate ( infoDrawerTemplate );
			drawer.gameObject.SetActive( true );
			drawer.transform.SetParent ( infoDrawerTemplate.transform.parent );
			drawer.transform.localScale = Vector3.one;
		}
		drawer.Setup ( label, message );
		infoDrawers.Add ( ID, drawer );
	}
}
