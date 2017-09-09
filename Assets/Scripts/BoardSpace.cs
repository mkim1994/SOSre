﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpace : MonoBehaviour {

    public int color;
    public int colNum, rowNum;
    public bool isCenterSpace;

    public List<Tile> tileStack;
    public int provisionalTileCount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetBoardSpace(int c, int col, int row){
        color = c;
        colNum = col;
        rowNum = row;
    }

    public void AddTile(Tile tileToAdd, bool positionTile){
		if (!isCenterSpace)
		{
			if (tileStack.Count > 0)
			{
				tileStack[tileStack.Count - 1].gameObject.layer = LayerMask.NameToLayer("Default");

			}
			if (positionTile)
			{
				tileToAdd.transform.position = new Vector3(transform.position.x, provisionalTileCount * 0.2f + 0.1f, transform.position.z);
			}

			provisionalTileCount += 1;
			tileStack.Add(tileToAdd);
			tileToAdd.gameObject.layer = LayerMask.NameToLayer("TopTiles");
		}
		else
		{
		/*	//color = tileToAdd.color;
			GameObject.FindWithTag("TurnManager").GetComponent<TurnManager>().scoringMode = true;
			GetComponent<MeshRenderer>().material = boardManager.mats[tileToAdd.color];
			//Destroy (tileToAdd.gameObject);
			//juicy.TileSinkAnimation(tileToAdd.gameObject, transform.gameObject);
			boardManager.centerSpaceChanged = true;
            */
		}
    }

}