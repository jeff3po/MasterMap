using UnityEngine;
using System.Collections;

public class ControlPanelSubpanel : MonoBehaviour 
{
	public ControlPanelButton buttonTemplate;

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
