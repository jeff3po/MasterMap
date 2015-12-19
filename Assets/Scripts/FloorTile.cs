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

	public bool isDoor = false;

	WorldMap map;
	static bool touching = false;

	public void SetVisible ( bool vis )
	{
		gameObject.SetActive ( vis );
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
				DrawHere();
			}
		}
	}

	public void SetColor ( Color c )
	{
		color = c;
		floorImage.color = c;
	}

	void DrawHere()
	{
		// Can draw!
		map.DrawOnThisTile(this);
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if ( touching )
		{
			DrawHere();
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

		DrawHere();

		group.blocksRaycasts = false;
	}

	public void EndTouch()
	{
		touching = false;

		group.blocksRaycasts = true;
	}
}
