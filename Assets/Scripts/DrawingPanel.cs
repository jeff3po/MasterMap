using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DrawingPanel : MonoBehaviour 
{
	public Image swatchTemplate;

	List<Color> colors = new List<Color>();

	void Start()
	{
		for ( int r=0;r<3;r++)
		{
			for ( int g=0;g<3;g++)
			{
				for ( int b=0;b<3;b++)
				{
					Color c = new Color ( r*0.33f, g*0.33f, b*0.33f);
					colors.Add ( c );
				}
			}
		}

		colors.Add ( new Color(1,1,1));
		
		foreach ( Color c in colors )
		{
			Image swatch = Instantiate(swatchTemplate);
			swatch.color = c;
			swatch.transform.SetParent ( swatchTemplate.transform.parent);
			swatch.transform.localScale = Vector3.one;
		}
		swatchTemplate.gameObject.SetActive ( false );
	}
}
