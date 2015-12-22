using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base token type. Draggable and droppable from/to FloorTiles. 
/// Subclasses are players, monsters, etc
/// </summary>
public class TokenBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
	public Image frame;
	public FloorTile homeTile = null;
	public WorldMap map;
	public RectTransform tokenLayer;
	bool _isMobile = true;

	/// <summary>
	/// All possible activity lists
	/// </summary>
	public Dictionary<string,ActivityList> allPossibleActivities = new Dictionary<string, ActivityList>();

	public virtual bool isMobile()
	{
		// TODO: Subclasses like Characters are always mobile while decorations are mobile only during Deco edit mode
		return _isMobile;
	}

	public void OnBeginDrag ( PointerEventData data )
	{
		transform.SetParent ( tokenLayer );
		homeTile = null;
		DebugOutput();
	}

	public void OnDrag ( PointerEventData data)
	{
		transform.Translate ( data.delta );
	}

	public void OnEndDrag ( PointerEventData data )
	{
		FindNewHome();
	}

	public void OnPointerClick ( PointerEventData data )
	{
		FindNewHome();
	}

	public void OnDrop ( PointerEventData data )
	{
		FindNewHome();
	}

	void FindNewHome()
	{
		// Find closest floortile
		FloorTile closestTile = map.FindClosestTile ( transform.position );
		homeTile = closestTile;
		transform.SetParent ( homeTile.transform );
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
//		Debug.Log ( string.Format ("Dropped {0} onto {1}, mounted to {2}", name, homeTile.name, transform.parent ) );

		DebugOutput();
	}

	void DebugOutput()
	{
		string homeName = "No home"; if ( homeTile != null ) { homeName = homeTile.name; }
		string parentName = "Not parented"; if ( transform.parent != null ) { parentName = transform.parent.name; }
		string message = string.Format ( "Home tile: {0}\nParent: {1}", homeName, parentName );
		map.infoPanel.AddDrawer ( name, name, message );
	}
}
