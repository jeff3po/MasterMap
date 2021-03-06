﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FloorTile : MonoBehaviour, IDragHandler, IPointerEnterHandler, IScrollHandler, IPointerUpHandler, IPointerDownHandler
{
	/// <summary>
	/// Once any floor tile is being touched, they all know about it
	/// </summary>
	static public bool touching = false;

	/// <summary>
	/// Default image. Overridden by other factors
	/// </summary>
	public Image floorImage;

	/// <summary>
	/// Coordinates
	/// </summary>
	public int xPos;

	/// <summary>
	/// Coordinates
	/// </summary>
	public int yPos;

	/// <summary>
	/// Color override
	/// </summary>
	public Color color;

	/// <summary>
	/// Can attach a door
	/// </summary>
	public Door attachedDoor = null;

	/// <summary>
	/// Is true if this tile is shared between exactly two rooms
	/// </summary>
	public bool IsDoorway
	{
		get { return attachedDoor != null; }
	}

	/// <summary>
	/// Unique because there can be only one tile at each coordinate
	/// </summary>
	public string UniqueID()
	{
		return xPos+"-"+yPos;
	}

	/// <summary>
	/// Convenience list of the one or two rooms in which this tile lives
	/// </summary>
	List<Room> owningRooms = new List<Room>();

	public void AddToRoom ( Room r )
	{
		if ( owningRooms.Contains ( r ) == false)
		{
			owningRooms.Add ( r );
		}
		SetColor ( r.roomColor );
	}

	public void RemoveFromRoom ( Room r )
	{
		owningRooms.Remove ( r );
		if ( owningRooms.Count > 0 )
		{
			SetColor ( owningRooms[0].roomColor );
		}
		else
		{
			SetColor ( Color.white );
		}
	}

	public void SetVisible ( bool vis )
	{
		gameObject.SetActive ( vis );
	}

	public void SetAlpha ( float a )
	{
		Color c = floorImage.color;
		c.a = a;
		floorImage.color = c;
	}

	public void Setup ( int x, int y )
	{
		xPos = x;
		yPos = y;
	}

	public void SetColor ( Color c )
	{
		color = c;
		floorImage.color = c;
	}

	void ClickThisTile()
	{
		// Go thorugh map first. It filters out the current state of everything
		WorldMap.Instance.ClickThisTile(this);
	}

	public void OnPointerEnter(PointerEventData data)
	{
		// If already dragging, entering a new tile counts as clicking it 
		if ( touching )
		{
			ClickThisTile();
		}
	}

	public void OnScroll(PointerEventData data)
	{
		WorldMap.Instance.OnScroll(data);
	}

	/// <summary>
	/// Report how much player drags any tile and slide the entire map that amount
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnDrag(PointerEventData data)
	{
		//Debug.Log ( "Drag "+data.delta );
		WorldMap.Instance.Scroll(data.delta);
	}

//	public void OnEndDrag(PointerEventData data)
//	{
//		EndTouch();
//	}

	public void OnPointerUp(PointerEventData data)
	{
		EndTouch();
	}

	public void OnPointerDown(PointerEventData data)
	{
		StartTouch();
	}

	public void StartTouch()
	{
		if ( touching == false ) 
		{ 
			touching = true;
		}
		ClickThisTile();
	}

	public void EndTouch()
	{
		touching = false;
		RoomManager.Instance.ResetAddFloorState();
	}

	public void AttachDoor ( Door door )
	{
		attachedDoor = door;
	}

	public void RemoveDoor()
	{
		Destroy ( attachedDoor.gameObject );
		attachedDoor = null;
	}


}
