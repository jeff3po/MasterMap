using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base token type. Draggable and droppable from/to FloorTiles. 
/// Subclasses are players, monsters, furniture, etc
/// </summary>
public class TokenBase : Archivable, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
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
		if ( map.editMode == WorldMap.EditMode.Room ) { return; }
			
		transform.SetParent ( tokenLayer );
		homeTile = null;
		Infopanel();
	}

	public void OnDrag ( PointerEventData data)
	{
		if ( map.editMode == WorldMap.EditMode.Room ) { return; }
		transform.Translate ( data.delta );
	}

	public void OnEndDrag ( PointerEventData data )
	{
		if ( map.editMode == WorldMap.EditMode.Room ) { return; }
		FindNewHome();
	}


	public void OnPointerClick ( PointerEventData data )
	{
		Infopanel();

		if ( map.editMode == WorldMap.EditMode.Play ) 
		{
			Interact();
			return; 
		}
		FindNewHome();
	}

	public void OnDrop ( PointerEventData data )
	{
		if ( map.editMode == WorldMap.EditMode.Room ) { return; }
		FindNewHome();
	}

	public void FindNewHome()
	{
		// Find closest floortile
		FloorTile closestTile = map.FindClosestTile ( transform.position );
		homeTile = closestTile;
		transform.SetParent ( homeTile.transform );
		transform.localPosition = Vector3.zero;
		transform.localScale = Vector3.one;
	}

	/// <summary>
	/// Clicked on in play mode
	/// </summary>
	public virtual void Interact()
	{
		// Each subclass handles clicks differently
	}

	public virtual void Infopanel()
	{
	}
}
