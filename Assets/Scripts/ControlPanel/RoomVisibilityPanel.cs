using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomVisibilityPanel : MonoBehaviour 
{
	public Image frame;
	public GridLayoutGroup grid;
	public RoomVisToggle toggleTemplate;
	List<RoomVisToggle> toggles = new List<RoomVisToggle>();

	void Start()
	{
		toggleTemplate.gameObject.SetActive ( false );
	}

	public void Reset()
	{
		foreach ( RoomVisToggle t in toggles )
		{
			Destroy ( t.gameObject );
		}
		toggles.Clear();
//		Debug.Log ( "Reset toggles");
	}

	/// <summary>
	/// Grid layout group handles positioning
	/// </summary>
	/// <param name="index">Index.</param>
	public void AddRoomToggle ( int index, Color c )
	{
		// Prevent adding more than once
		foreach ( RoomVisToggle t in toggles )
		{
			if ( t.isIndex ( index ) ) { return; }
		}

		RoomVisToggle tog = Instantiate ( toggleTemplate );
		tog.gameObject.SetActive( true );
		tog.transform.SetParent ( toggleTemplate.transform.parent);
		tog.transform.localScale = Vector3.one;
		toggles.Add ( tog );
		tog.Setup ( index, c);

		int offset = (int) ((float)toggles.Count / (float)8);
		offset +=2;
		float ht = 150;//frame.rectTransform.sizeDelta.y;// / offset;
		ht /= offset;
		Vector2 newSz = new Vector2 ( 50, ht );
		grid.cellSize = newSz;
		Debug.Log ( "Offset "+offset+"  cellsize "+newSz);
	}
}
