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
    //public LayerMask defaultAndTopTiles;

    public GameObject Previews;
    public GameObject PauseScreen;

    public GameObject HighlightCenter;

    public SpriteRenderer gradient;

    public AudioController audioController;

    public int currentTileColor;

	// Use this for initialization
	void Start () {

       // Time.timeScale = 1;

	}
	
	// Update is called once per frame
	void Update () {
        Services.LevelManager.Update();
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

	}

    public void ConfirmButton(){
       // Services.BoardManager.ConfirmSpill();
    }

    public void UndoButton(){
       // Services.BoardManager.UndoSpill();
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
}
