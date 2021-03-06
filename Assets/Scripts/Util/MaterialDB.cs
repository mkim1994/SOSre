﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Material DB")]
public class MaterialDB : ScriptableObject {
    [SerializeField]
    private Material[] tileMats;
    public Material[] TileMats { get { return tileMats; } }

    [SerializeField]
    private Shader[] highlightShaders;
    public Shader[] HighlightShaders { get { return highlightShaders; }}

    [SerializeField]
    private Shader[] arrowShaders;
    public Shader[] ArrowShaders { get { return arrowShaders; }}


    [SerializeField]
    private Material[] boardMats;
    public Material[] BoardMats { get { return boardMats; }}

    [SerializeField]
    private Sprite[] previewSprites;
    public Sprite[] PreviewSprites{ get { return previewSprites; }}
}
