﻿using UnityEngine;
using System.Collections;

public class BackgroundGenerator : MonoBehaviour
{
	// Extra width to take up in the X and Y directions
	public float tileOffsetX;
	public float tileOffsetY;
	// List of tiles and their weighted random chances
	public Sprite[] tiles;
	public int[] tileWeights;

	float tileWidth;	// Unit width of each tile
	float tileHeight;	// Unit height of each tile
	float startX = 0f;	// x-coord to start making tiles per row
	float endX = 0f;	// x-coord to end making tiles per row

	// Current highest y level of tiles
	float screenTopMax;

	int lowestTileRow;
	int numRows;

	// Creates a new row of random tiles at position y
	void MakeNewRow(float height)
	{
		GameObject rowContainer = new GameObject("Row" + numRows);
		numRows++;
		rowContainer.transform.parent = this.transform;

		// Make a tile at each X point from left to right
		for (float i = startX; i < endX; i += tileWidth) {
			Sprite spriteToUse = tiles[0];
			int rand = Random.Range(0, 100);
			// Compare our random number to the weighted chance of each tile
			for (int j = 0; j < tiles.Length; j++)
			{
				if (rand < tileWeights[j])
				{
					spriteToUse = tiles[j];
					break;
				}
				// If we didnt find the right tile, subtract that tile's
				// weighted random change from our random number
				rand -= tileWeights[j];
			}

			GameObject tileToMake = new GameObject();
			tileToMake.AddComponent<SpriteRenderer>().sprite = spriteToUse;

			GameObject newTile = Instantiate(tileToMake, new Vector3(i, height), Quaternion.identity) as GameObject;
			newTile.transform.parent = rowContainer.transform;
		}
	}

	// Use this for initialization
	void Start ()
	{
		Vector2 botLeftScreen = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
		Vector2 topRightScreen = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));

		tileWidth = tiles[0].bounds.size.x;
		tileHeight = tiles[0].bounds.size.y;
		lowestTileRow = 0;
		numRows = 0;

		screenTopMax = topRightScreen.y + tileOffsetY;
		float screenBot = botLeftScreen.y - tileOffsetY;
		startX = botLeftScreen.x - tileOffsetX;
		endX = topRightScreen.x + tileOffsetX;

		for (float i = screenBot; i <= screenTopMax; i += tileHeight)
		{
			MakeNewRow(i);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		float screenTopCurrent = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f)).y + tileOffsetY;
		if (screenTopCurrent > screenTopMax)
		{
			MakeNewRow(screenTopMax);
			screenTopMax += tileHeight;
			Destroy(GameObject.Find("Row" + lowestTileRow));
			lowestTileRow++;
		}
	}
}
