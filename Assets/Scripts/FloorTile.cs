using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FloorTile : MonoBehaviour, IDragHandler, IPointerEnterHandler, IScrollHandler, IEndDragHandler, IPointerUpHandler
{
	public Image floorImage;
	public CanvasGroup group;
	public int xPos;
	public int yPos;
	public Color color;
	public Door attachedDoor = null;
	public bool isDoor = false;

	List<Room> owningRooms = new List<Room>();

	WorldMap map;
	static bool touching = false;

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

	public void Setup ( WorldMap m, int x, int y )
	{
		map = m;
		xPos = x;
		yPos = y;
	}

	void Update()
	{
		if ( EventSystem.current.currentSelectedGameObject == this )
		{
			if ( touching )
			{
				ClickThisTile();
			}
		}
	}

	public void SetColor ( Color c )
	{
		color = c;
		floorImage.color = c;
	}

	void ClickThisTile()
	{
		map.ClickThisTile(this);
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if ( touching )
		{
			ClickThisTile();
		}
	}

	public void OnScroll(PointerEventData data)
	{
		map.OnScroll(data);
	}

	/// <summary>
	/// Report how much player drags any tile and slide the entire map that amount
	/// </summary>
	/// <param name="data">Data.</param>
	public void OnDrag(PointerEventData data)
	{
		//Debug.Log ( "Drag "+data.delta );
		map.Scroll(data.delta);
	}

	public void OnEndDrag(PointerEventData data)
	{
		map.addState = WorldMap.AddFloorState.undefined;
	}

	public void OnPointerUp(PointerEventData data)
	{
		map.addState = WorldMap.AddFloorState.undefined;
	}

	public void StartTouch()
	{
		touching = true;

		ClickThisTile();

		group.blocksRaycasts = false;
	}

	public void EndTouch()
	{
		touching = false;

		group.blocksRaycasts = true;
	}
}
