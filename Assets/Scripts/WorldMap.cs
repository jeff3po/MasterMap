using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class WorldMap : MonoBehaviour, IScrollHandler
{
	public Transform mapScroller;
	public FloorTile floorTileTemplate;
	public Scrollbar zoomBar;
	public ControlPanel controlPanel;

	FloorTile[,] worldGrid;

	public int columnCount;
	public int rowCount;

	public float maxZoom = 5;

	void Start()
	{
		floorTileTemplate.gameObject.SetActive ( false );

		worldGrid = new FloorTile[columnCount,rowCount];

		float tileWidth = floorTileTemplate.floorImage.rectTransform.sizeDelta.x;
		float tileHeight = floorTileTemplate.floorImage.rectTransform.sizeDelta.y;

		// Floor tile template starts centered in world. Build out then center
		for ( int x=0;x<columnCount;x++ )
		{
			for ( int y=0;y<rowCount;y++)
			{
				FloorTile newTile = Instantiate ( floorTileTemplate );
				newTile.gameObject.SetActive ( true );

				newTile.transform.SetParent ( floorTileTemplate.transform.parent );
				newTile.transform.localScale = Vector3.one;
				newTile.transform.localPosition = new Vector3 ( tileWidth * x, tileHeight * y, 0 );
				newTile.Setup ( this );
				newTile.name = x+"_"+y;
				worldGrid[x,y] = newTile;
			}
		}

		Vector2 offset = new Vector2 ( -columnCount/4 * tileWidth, -rowCount/4 * tileHeight );

		foreach ( FloorTile t in worldGrid )
		{
			t.transform.Translate ( offset );
		}
	}

	public void Scroll ( Vector2 scroll )
	{
		if ( controlPanel.CanDrag )
		{
			mapScroller.transform.Translate ( scroll );
		}
	}


	float zoomFactor = 1.0f;

	public void OnScroll(PointerEventData data)
	{
		zoomFactor += data.scrollDelta.y * Time.deltaTime;
		if ( zoomFactor > maxZoom ) { zoomFactor = maxZoom;}
		if ( zoomFactor < 1 ) { zoomFactor = 1;}
		Zoom();
	}

	void Zoom ( bool updateZoomBar=true )
	{
		mapScroller.localScale = Vector3.one * zoomFactor;

		if ( updateZoomBar )
		{
			zoomBar.value = Helpers.RemapToRange ( 1,5,0,1, zoomFactor);
		}
	}

	public void ManualZoom()
	{
		zoomFactor = Helpers.RemapToRange ( 0, 1, 1, 5, zoomBar.value );
		Zoom(false);
	}
}
