using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base token type. Draggable and droppable from/to FloorTiles. 
/// Subclasses are players, monsters, etc
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
		if ( map.editMode == WorldMap.EditMode.Play ) { return; }
			
		transform.SetParent ( tokenLayer );
		homeTile = null;
		DebugOutput();
	}

	public void OnDrag ( PointerEventData data)
	{
		if ( map.editMode == WorldMap.EditMode.Play ) { return; }
		transform.Translate ( data.delta );
	}

	public void OnEndDrag ( PointerEventData data )
	{
		if ( map.editMode == WorldMap.EditMode.Play ) { return; }
		FindNewHome();
	}


	public void OnPointerClick ( PointerEventData data )
	{
		if ( map.editMode == WorldMap.EditMode.Play ) { return; }
		FindNewHome();
	}

	public void OnDrop ( PointerEventData data )
	{
		if ( map.editMode == WorldMap.EditMode.Play ) { return; }
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

	/// <summary>
	/// Clicked on in play mode
	/// </summary>
	public virtual void Interact()
	{
		// Each subclass handles clicks differently
	}

	void DebugOutput()
	{
		string homeName = "No home"; if ( homeTile != null ) { homeName = homeTile.name; }
		string parentName = "Not parented"; if ( transform.parent != null ) { parentName = transform.parent.name; }
		string message = string.Format ( "Home tile: {0}\nParent: {1}", homeName, parentName );
		map.infoPanel.AddInfoDrawer ( name, name, message );


		CharacterStats stats = new CharacterStats("Boris", "Jeff", 3, new int[]{10,16,10,12,10,10}, 20, 12, 30 );
		stats.AddAttack ( new Attack ( "Sword", "Dx", 5, Attack.AttackType.Melee, 1, 1, 6, 2, "slashing" ) );
		stats.AddAttack ( new Attack ( "Magic Missile", "In", 120, Attack.AttackType.Ranged, 1, 1, 4, 1, "radiant"));
		map.infoPanel.AddCharacterDrawer ( stats );
	}
}
