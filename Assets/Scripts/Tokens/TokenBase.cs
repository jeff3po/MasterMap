using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary>
/// Base token type. Draggable and droppable from/to FloorTiles. 
/// Subclasses are players, monsters, furniture, etc
/// </summary>
public class TokenBase : Archivable, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
	public Image frame;
	public FloorTile homeTile = null;
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
		if ( WorldMap.Instance.editMode == WorldMap.EditMode.Room ) { return; }
			
		transform.SetParent ( tokenLayer );
		homeTile = null;
		Infopanel();
	}

	public void OnDrag ( PointerEventData data)
	{
		if ( WorldMap.Instance.editMode == WorldMap.EditMode.Room ) { return; }
		transform.Translate ( data.delta );
	}

	public void OnEndDrag ( PointerEventData data )
	{
		if ( WorldMap.Instance.editMode == WorldMap.EditMode.Room ) { return; }
		FindNewHome();
	}


	public void OnPointerClick ( PointerEventData data )
	{
		Infopanel();

		if ( WorldMap.Instance.editMode == WorldMap.EditMode.Play ) 
		{
			Interact();
			return; 
		}
		FindNewHome();
	}

	public void OnDrop ( PointerEventData data )
	{
		if ( WorldMap.Instance.editMode == WorldMap.EditMode.Room ) { return; }
		FindNewHome();
	}

	public void FindNewHome()
	{
		// Find closest floortile
		FloorTile closestTile = WorldMap.Instance.FindClosestTile ( transform.position );
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

	[HideInInspector]
	public string homeTileID = "";

	public override void PostInit()
	{
		base.PostInit();
		// Find hometile 
		homeTile = WorldMap.Instance.roomManager.FindTileByID(homeTileID);
	}

	public override void Init ( JSONNode data, int tokenIndex )
	{
		base.Init ( data, tokenIndex );
		homeTileID = data [ Category ] [ tokenIndex ] [ "hometile" ];
		_isMobile = data [ Category ] [ tokenIndex ] [ "isMobile" ].AsBool;

		int listCount = data [ Category ] [ tokenIndex ] [ "listCount" ].AsInt;
		for ( int listIndex=0;listIndex<listCount;listIndex++)
		{
			ActivityList newList = new ActivityList( data, tokenIndex, listIndex );
			allPossibleActivities.Add ( newList.title, newList );
		}
	}

	public override void Export(ref JSONNode data, int tokenIndex)
	{
		base.Export(ref data, tokenIndex);
		data [ Category ] [ tokenIndex ] [ "hometile" ] = homeTile.UniqueID();
		data [ Category ] [ tokenIndex ] [ "isMobile" ].AsBool = _isMobile;

		int listCount = 0;
		if ( allPossibleActivities != null )
		{
			foreach ( ActivityList list in allPossibleActivities.Values )
			{
				list.Export(ref data, tokenIndex, listCount );
				listCount ++;
			}
			data [ Category ] [ tokenIndex ] [ "listCount" ].AsInt = listCount;
		}
	}
}
