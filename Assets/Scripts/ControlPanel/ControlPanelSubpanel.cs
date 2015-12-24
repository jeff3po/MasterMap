using UnityEngine;
using System.Collections;

public class ControlPanelSubpanel : MonoBehaviour 
{
	[HideInInspector]
	public WorldMap map;

	public ControlPanelButton buttonTemplate;

	public virtual void Setup ( WorldMap m )
	{
		map = m;
	}

	public void AddButton ( string nm, System.Action callback )
	{
		buttonTemplate.gameObject.SetActive ( true );
		ControlPanelButton newButton = Instantiate ( buttonTemplate );
		newButton.transform.SetParent ( buttonTemplate.transform.parent);
		newButton.transform.localScale = Vector3.one;
		newButton.Setup ( nm, callback );

		buttonTemplate.gameObject.SetActive ( false );
	}
}
