﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : Scene<TransitionData> {

	void Start()
	{

       // Time.timeScale = 1;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void StartGame()
	{
		Services.SceneStackManager.Swap<Main>();
	}

    public void StartLevelEditor(){
        Services.SceneStackManager.Swap<LevelEditor>();
    }

    public void StartTutorial(){
        Services.SceneStackManager.Swap<Tutorial>();
    }
}
