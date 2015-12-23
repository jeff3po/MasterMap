using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Panel in the bar at top of screen
/// </summary>
public class InfoPanel : MonoBehaviour 
{
	public InfoDrawer infoDrawerTemplate;
	public CharacterDrawer characterDrawerTemplate;

	Dictionary<string,InfoDrawer> infoDrawers = new Dictionary<string, InfoDrawer>();
	Dictionary<string,CharacterDrawer> characterDrawers = new Dictionary<string, CharacterDrawer>();

	void Start()
	{
		infoDrawerTemplate.gameObject.SetActive ( false );
		characterDrawerTemplate.gameObject.SetActive ( false );
	}

	public void AddInfoDrawer ( string ID, string label, string message )
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

	public void AddCharacterDrawer ( CharacterStats stats )
	{
		CharacterDrawer drawer = null;

		string nm = stats.characterName;

		if ( characterDrawers.ContainsKey ( nm ) )
		{
			// Replace exisiting drawer's values
			// TODO: Smarter non-setup version
			characterDrawers.TryGetValue ( nm, out drawer );
			Destroy ( drawer.gameObject );
			characterDrawers.Remove ( nm );
		}

		// Make a new one from scratch
		drawer = Instantiate ( characterDrawerTemplate );
		drawer.gameObject.SetActive( true );
		drawer.transform.SetParent ( characterDrawerTemplate.transform.parent );
		drawer.transform.localScale = Vector3.one;
		drawer.Setup ( stats, characterDrawers );
		characterDrawers.Add ( nm, drawer );
	}
}
