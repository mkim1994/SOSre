using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.ImageEffects;

public class LevelEditor : Scene<TransitionData> {

	public LayerMask spawnedTileLayer;
	public LayerMask topTileLayer;
	public LayerMask invisPlane;
    public LayerMask spillUILayer;
    public LayerMask topTilesAndSpillUI;

    public LayerMask tileLayer;
    //public LayerMask defaultAndTopTiles;

    public GameObject Previews;
    public GameObject PauseScreen;

    public GameObject HighlightCenter;

    public SpriteRenderer gradient;

    public AudioController audioController;

    public int currentTileColor;

    public int[] previewColors;
    public Dropdown[] dropdowns;

    public string levelStr = "";

    public GameObject selectedTileMenu;

    public InputField input;
    private string inputString;


	// Use this for initialization
	void Start () {

       // Time.timeScale = 1;

	}
	
	// Update is called once per frame
	void Update () {
        Services.LevelManager.Update();
        input.onEndEdit.AddListener(delegate { inputString = input.text; });
	}

	void InitializeServices()
	{
		Services.LevelEditor = this;
        Services.BoardData.InitializeBoardData();
        Services.LevelManager.InitializeBoard();
	}

	internal override void OnEnter(TransitionData data)
	{
		InitializeServices();
        currentTileColor = -1;
		Services.GameManager.currentCamera = GetComponentInChildren<Camera>();
        previewColors = new int[8];
	}

    public void ConfirmButton(){
       // Services.BoardManager.ConfirmSpill();
    }

    public void UndoButton(){
       // Services.BoardManager.UndoSpill();
    }

    public void TrashButton()
    {
        Services.LevelManager.TrashTile();
    }

    public void Pause(){
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
        Services.GameManager.currentCamera.GetComponent<BlurOptimized>().enabled = true;

    }

    public void Resume(){
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
        Services.GameManager.currentCamera.GetComponent<BlurOptimized>().enabled = false;
    }

    public void Restart(){
        // Services.SceneStackManager.Swap<Main>();
        Time.timeScale = 1;
        Services.SceneStackManager.PopScene();
        Services.SceneStackManager.PushScene<LevelEditor>();
    }

    public void MainMenu(){

        Services.SceneStackManager.Swap<TitleScreen>();

    }

    public void SpawnTile(int color){
        Debug.Log(color);
        currentTileColor = color;
    }

    public void PreviewChange(int index){
        previewColors[index] = dropdowns[index].value;
    }

    public void ImportLevel(){
        Services.LevelManager.CleanBoard();
        Services.LevelManager.InitializeBoard();
        Services.LevelManager.ParseLevel(inputString);

    }

    public void ExportLevel(){

        for (int i = 0; i < previewColors.Length; ++i){
            levelStr += previewColors[i] +"-";
        }

        string boardStr = "";
        for (int c = 0; c < Services.LevelManager.numCols; ++c){
            for (int r = 0; r < Services.LevelManager.numRows; ++r){
                boardStr += "[-" + c + "-,-" + r + "-]-";
                foreach(Tile tile in Services.LevelManager.board[c, r].tileStack){
                    boardStr += tile.color + "-";
                }
            }
        }

        levelStr += boardStr;
        levelStr += ".";

        HandleTextFile.WriteString(inputString, levelStr);
    }
}
