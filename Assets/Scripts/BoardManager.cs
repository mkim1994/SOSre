﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class BoardManager {

    private FSM<BoardManager> fsm;

    public int numRows, numCols;

    public int currentNumRows, currentNumCols;

    private int currentLowestColIndex, currentHighestColIndex, currentLowestRowIndex, currentHighestRowIndex;

    public BoardSpace[,] board;
    public List<BoardSpace> centerSpaces;
    public List<Tile> tileBag;

    public GameObject pivotPoint;

    public Tile spawnedTile;
    public Tile selectedTile;

    public bool tileInPosition;
    public int sideAboutToCollapse;
    /*public LayerMask spawnedTileLayer;
    public LayerMask topTileLayer;
    public LayerMask invisPlane;*/



    public void Update(){
        fsm.Update();
    }

	public void InitializeBoard()
    {
      /*  spawnedTileLayer = LayerMask.NameToLayer("DrawnTile");
        topTileLayer = LayerMask.NameToLayer("TopTiles");
        invisPlane = LayerMask.NameToLayer("InvisBoardPlane");*/
        CreateBoard();
        CreateTileBag();

        pivotPoint = GameObject.FindGameObjectWithTag("PivotPoint");

        currentNumRows = numRows;
        currentNumCols = numCols;
        currentHighestRowIndex = numRows - 1;
        currentHighestColIndex = numCols - 1;

        if(Services.BoardData.randomTiles){
			for (int i = 0; i < numCols; i++)
			{
				for (int j = 0; j < numRows; j++)
				{
					if ((!IsCentered(i, numCols) && IsEdge(j, numRows)) || (!IsCentered(j, numRows) && IsEdge(i, numCols)))
					{
						Tile firstTileToPlace;
						Tile secondTileToPlace;
						firstTileToPlace = DrawTile();
						secondTileToPlace = DrawTile();
						board[i, j].AddTile(firstTileToPlace, true);
						board[i, j].AddTile(secondTileToPlace, true);
					}
				}
			}

        } else{


        }



		fsm = new FSM<BoardManager>(this);
		fsm.TransitionTo<SpawnTile>();

	}

    private void CreateBoard(){
        centerSpaces = new List<BoardSpace>();

        numCols = Services.BoardData.numCols;
        numRows = Services.BoardData.numRows;
		board = new BoardSpace[numCols, numRows];
		for (int i = 0; i < numCols; i++)
		{
			for (int j = 0; j < numRows; j++)
			{
				int spaceColor;
				if (IsCentered(i, numCols) && IsCentered(j, numRows))
				{
					spaceColor = 0;
				}
				else if ((i + j) % 2 == 0)
				{
					spaceColor = 1;
				}
				else
				{
					spaceColor = 2;
				}
				CreateBoardSpace(i, j, spaceColor);
			}
		}



    }

	public bool IsCentered(int index, int sideLength)
	{
		bool centered = (index == sideLength / 2 - 1) || (index == sideLength / 2);
		return centered;
	}

	private bool IsEdge(int index, int sideLength)
	{
		bool edge = (index == 0) || (index == sideLength - 1);
		return edge;
	}


	private void CreateBoardSpace(int colNum, int rowNum, int color){
        Vector3 location = new Vector3(colNum - numCols / 2 + 0.5f, 0, rowNum - numRows / 2 + 0.5f);
        GameObject boardSpace = Object.Instantiate(Services.Prefabs.BoardSpace, location, Quaternion.LookRotation(Vector3.down)) as GameObject;
        boardSpace.GetComponent<MeshRenderer>().material = Services.Materials.BoardMats[color];
        boardSpace.GetComponent<BoardSpace>().SetBoardSpace(color, colNum, rowNum);
		if (IsCentered(colNum, numCols) && IsCentered(rowNum, numRows))
		{
			boardSpace.GetComponent<BoardSpace>().isCenterSpace = true;
			centerSpaces.Add(boardSpace.GetComponent<BoardSpace>());
		}
		else
		{
			boardSpace.GetComponent<BoardSpace>().isCenterSpace = false;
		}
		board[colNum, rowNum] = boardSpace.GetComponent<BoardSpace>();
    }

    private void CreateTile(int materialIndex){
		//GameObject tile;
		Vector3 offscreen = new Vector3(-1000, -1000, -1000);
        GameObject tile = Object.Instantiate(Services.Prefabs.Tile, offscreen, Quaternion.identity) as GameObject;
        tile.GetComponent<MeshRenderer>().material = Services.Materials.TileMats[materialIndex];
        tile.GetComponent<Tile>().SetTile(materialIndex);
		tileBag.Add(tile.GetComponent<Tile>());
    }

	private void CreateTileBag()
	{
		tileBag = new List<Tile>();
        for (int i = 0; i < 4; ++i){
            CreateTilesOfAColor(i);
        }
	}

    private void CreateTilesOfAColor(int materialIndex){
        for (int i = 0; i < Services.BoardData.initialNumberOfEachTileColor[materialIndex]; ++i)
		{
			CreateTile(materialIndex);
		}
    }

	public Tile DrawTile()
	{
		//prevent out of range exception
		if (tileBag.Count > 0)
		{
			int numTilesInBag = tileBag.Count;
			Tile drawnTile;
			int tileIndexToDraw;
            if (Services.BoardData.randomTiles)
			{
				tileIndexToDraw = Random.Range(0, numTilesInBag);
			}
			else
			{
				tileIndexToDraw = 0;
			}
			drawnTile = tileBag[tileIndexToDraw];
			tileBag.Remove(drawnTile);
			return drawnTile;
		}
		return null;
	}


	public void DrawTileToPlace()
	{
		Tile tileToPlace;
		tileToPlace = DrawTile();
		if (tileToPlace == null)
		{
            //yield return new WaitForSeconds (1f);
            //mode = "Game Over";
		}
		else
		{
			SetupSpawnedTile(tileToPlace);
			spawnedTile = tileToPlace;
			spawnedTile.GetComponent<MeshRenderer>().sortingOrder = 2;
		}
	}

	void SetupSpawnedTile(Tile tileToPlace)
	{
		tileToPlace.transform.SetParent(pivotPoint.transform);
		tileToPlace.transform.localPosition = new Vector3(-5, 0, 0);
        tileToPlace.gameObject.layer = LayerMask.NameToLayer("DrawnTile");
		//juicyManager.spawnTileAnimation(tileToPlace.gameObject);
	}

	BoardSpace CalculateSpaceFromLocation(Vector3 location)
	{
		int col = Mathf.RoundToInt(location.x - 0.5f + numCols / 2);
		int row = Mathf.RoundToInt(location.z - 0.5f + numRows / 2);
        return board[col, row];
	}

	public List<BoardSpace> GetSpaceListFromSideNum()
	{
		List<BoardSpace> spaceList = new List<BoardSpace>();
		int indexToCollapse = 0;
		if (sideAboutToCollapse == 0)
		{
			indexToCollapse = currentLowestColIndex;
		}
		else if (sideAboutToCollapse == 1)
		{
			indexToCollapse = currentHighestRowIndex;
		}
		else if (sideAboutToCollapse == 2)
		{
			indexToCollapse = currentHighestColIndex;
		}
		else
		{
			indexToCollapse = currentLowestRowIndex;
		}
		if ((sideAboutToCollapse % 2) == 0)
		{
			for (int i = currentLowestRowIndex; i < currentHighestRowIndex + 1; i++)
			{
				spaceList.Add(board[indexToCollapse, i]);
			}
		}
		else
		{
			for (int i = currentLowestColIndex; i < currentHighestColIndex + 1; i++)
			{
				spaceList.Add(board[i, indexToCollapse]);
			}
		}
		return spaceList;
	}


    public void SpawnTileAction(){

        DrawTileToPlace();

    }

    public void SelectTileAction(){

        Ray ray = Services.GameManager.currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Services.Main.spawnedTileLayer))
            {
                //ToggleGlow(spawnedTile, "bright");
                // SetSpaceGlow("dark");
                selectedTile = spawnedTile;
                spawnedTile = null;
            }
        }
    }

    public void PlaceTileAction(){
        Ray ray = Services.GameManager.currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Services.Main.topTileLayer))
        {
            //if (!CalculateSpaceFromLocation(hit.collider.transform.position).isCenterTile)
            //{
            Vector3 pointOnBoard = hit.transform.position;
            selectedTile.transform.position = new Vector3(pointOnBoard.x, pointOnBoard.y + 0.2f, pointOnBoard.z);
            selectedTile.transform.parent = null;
            tileInPosition = true;
            //BoardSpace space = CalculateSpaceFromLocation(pointOnBoard);
            //ToggleGlow(space, "bright");
            /*if (highlightedSpace != null)
            {
                if (highlightedSpace != space)
                {
                    ToggleGlow(highlightedSpace, "normal");
                }
            }
            highlightedSpace = space;*/
            //}
        }
        else
		{
            tileInPosition = false;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Services.Main.invisPlane))
			{
				selectedTile.transform.position = hit.point;
			}
            //selectedTile.transform.position = Services.GameManager.currentCamera.ScreenToWorldPoint(Input.mousePosition);
  
        }


        //finalize tile placement
        if(Input.GetMouseButtonDown(0) && tileInPosition){
            tileInPosition = false;

			BoardSpace space = CalculateSpaceFromLocation(selectedTile.transform.position);
			space.AddTile(selectedTile, false);
			selectedTile.GetComponent<MeshRenderer>().sortingOrder = 0;
            //ToggleGlow(selectedTile, "normal");
            //SetSpaceGlow("normal");
            /*if (highlightedSpace != null)
			{
				ToggleGlow(highlightedSpace, "normal");
			}*/
            //CheckForScore();

            if (currentNumCols == numCols && currentNumRows == numRows) //!firstTileFinalized)
			{
                
				//firstTileFinalized = true;
				if ((space.colNum == 0) && (space.rowNum != 0))
				{
					sideAboutToCollapse = 0;
				}
                else if (space.rowNum == numRows - 1)
				{
					sideAboutToCollapse = 1;
				}
				else if (space.colNum == numCols - 1)
				{
					sideAboutToCollapse = 2;
				}
				else
				{
					sideAboutToCollapse = 3;
				}

				List<BoardSpace> boardspaces = GetSpaceListFromSideNum();
				foreach (BoardSpace bs in boardspaces)
				{
                    bs.gameObject.GetComponent<MeshRenderer>().material = Services.Materials.BoardMats[3];
					bs.aboutToCollapse = true;
				}
			}
            //selectedTile.GetComponent<AudioSource>().Play();

            selectedTile = null;

        }

    }




	private class Turn : FSM<BoardManager>.State { }

	private class SpawnTile : Turn
	{
		public override void OnEnter()
		{
            Context.SpawnTileAction();
		}
		public override void Update()
		{
            TransitionTo<SelectTile>();
            return;
		}
	}

	private class SelectTile : Turn
	{
		public override void OnEnter()
		{
		}
		public override void Update()
		{
            if (Context.spawnedTile != null)
            {
                Context.SelectTileAction();
            } else{
                TransitionTo<PlaceTile>();
                return;
            }



		}
	}

	private class PlaceTile : Turn
	{
		public override void OnEnter()
		{
		}
		public override void Update()
		{
            if (Context.selectedTile != null)
            {
                Context.PlaceTileAction();
            } else{
                TransitionTo<BoardFall>();
                return;
            }
		}
	}

    private class BoardFall : Turn{
        public override void OnEnter(){
            
        }
        public override void Update(){
            
        }

    }

	private class SelectStack : Turn
	{
		public override void OnEnter()
		{
			//Context. ___
		}
		public override void Update()
		{

		}
	}

	private class QueueSpill : Turn
	{
		public override void OnEnter()
		{
			//Context. ___
		}
		public override void Update()
		{

		}
	}

	private class Interim : Turn
	{
		public override void OnEnter()
		{
			//Context. ___
		}
		public override void Update()
		{

		}
	}

	private class FinalizeSpill : Turn
	{
		public override void OnEnter()
		{
			//Context. ___
		}
		public override void Update()
		{

		}
	}

	private class GameOver : Turn
	{
		public override void OnEnter()
		{
			//Context. ___
		}
		public override void Update()
		{

		}
	}
}
