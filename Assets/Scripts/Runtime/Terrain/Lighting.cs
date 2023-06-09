﻿using System;
using System.Collections;
using JetBrains.Annotations;
using TerrariaClone.Runtime.Data;
using TerrariaClone.Runtime.Player;
using UnityEngine;

namespace TerrariaClone.Runtime.Terrain
{
    public class Lighting : MonoBehaviour
    {
        public LightingModes lighting;
        private TerrainGenerator _terrain;
        private float[,] _lightValues;
        public float sunlightBrightness = 15;

        public Texture2D lightMap;
        public Transform lightOverlay;
        public Material lightShader;
        
        [Header("Testing")]
        public bool smoothLighting;
        [Min(.1f)] public float groundAbsorption;
        [Min(.1f)] public float wallAbsorption;
        public int editRadius;

        public void Init()
        {
            _terrain = GetComponent<TerrainGenerator>();
            
            _lightValues = new float[TerrainConfig.Settings.worldSize.x, TerrainConfig.Settings.worldSize.y];
            lightMap = new Texture2D(TerrainConfig.Settings.worldSize.x, TerrainConfig.Settings.worldSize.y);
            
            lightOverlay.localScale =
                new Vector3(TerrainConfig.Settings.worldSize.x, TerrainConfig.Settings.worldSize.y, 1);
            lightOverlay.position = new Vector3(TerrainConfig.Settings.worldSize.x / 2,
                TerrainConfig.Settings.worldSize.y / 2, 10);

            lightMap.filterMode = FilterMode.Point;
            lightShader.SetTexture("_LightMap", lightMap);

        }

        /*private void Update()
        {
            if(smoothLighting)
            {
                switch(lighting)
                {
                    case LightingModes.Point:
                        {
                            lightMap.filterMode = FilterMode.Point;
                            RedrawLighting();
                            break;
                        }
                    case LightingModes.Bilinear:
                        {
                            lightMap.filterMode = FilterMode.Bilinear;
                            RedrawLighting();
                            break;
                        }
                    case LightingModes.Trilinear:
                        {
                            lightMap.filterMode = FilterMode.Trilinear;
                            RedrawLighting();
                            break;
                        }
                }
            }
        }*/

        private void UpdateLighting(int iterations, int rootX, int rootY, int stopX, int stopY)
        {
            //yield return new WaitForEndOfFrame();
            for (var i = 0; i < iterations; i++)
            {
                for (var x = rootX; x < stopX; x++)
                {
                    var lightLevel = sunlightBrightness;
                    for (var y = stopY - 2; y >= rootY; y--)
                    {
                        //check if this block is a torch OR exposes background
                        if (_terrain.IsIlluminate(x, y) ||
                            (WorldData.GetTile(x, y, 1) is null && WorldData.GetTile(x, y, 3) is null))
                            lightLevel = sunlightBrightness;
                        else
                        {
                            //else find the brightest neighbour
                            var nx1 = Mathf.Clamp(x - 1, 0, stopX - 1);
                            var nx2 = Mathf.Clamp(x - 1, 0, stopX - 1);
                            var ny1 = Mathf.Clamp(y - 1, 0, stopY - 1);
                            var ny2 = Mathf.Clamp(y - 1, 0, stopY - 1);

                            lightLevel = Mathf.Max(_lightValues[x, y], 
                                _lightValues[nx1, y], _lightValues[nx2, y], 
                                _lightValues[x, ny1], _lightValues[x, ny2]);

                            if (WorldData.GetTile(x, y, 1) is not null) lightLevel -= groundAbsorption;
                            else if (WorldData.GetTile(x, y, 3) is not null) lightLevel -= wallAbsorption;
                            else lightLevel -= 1;
                        }

                        _lightValues[x, y] = lightLevel;
                    }
                }
                
                //reverse calculation to remove artefacts
                for (var x = stopX - 1; x > rootX; x--)
                {
                    var lightLevel = sunlightBrightness;
                    for (var y = rootY; y < stopY; y++)
                    {
                        //check if this block is a torch OR exposes background
                        if (_terrain.IsIlluminate(x, y) ||
                            (WorldData.GetTile(x, y, 1) is null && WorldData.GetTile(x, y, 3) is null))
                            lightLevel = sunlightBrightness;
                        else
                        {
                            //else find the brightest neighbour
                            var nx1 = Mathf.Clamp(x + 1, 0, stopX - 1);
                            var nx2 = Mathf.Clamp(x + 1, 0, stopX - 1);
                            var ny1 = Mathf.Clamp(y + 1, 0, stopY - 1);
                            var ny2 = Mathf.Clamp(y + 1, 0, stopY - 1);

                            lightLevel = Mathf.Max(_lightValues[x, y], 
                                _lightValues[nx1, y], _lightValues[nx2, y], 
                                _lightValues[x, ny1], _lightValues[x, ny2]);

                            if (WorldData.GetTile(x, y, 1) is not null) lightLevel -= groundAbsorption; //ground
                            else if (WorldData.GetTile(x, y, 3) is not null) lightLevel -= wallAbsorption; //wall
                            //else if (!Mathf.Approximately(lightLevel, sunlightBrightness)) lightLevel -= airAbsorption; //air
                            else lightLevel -= 1;
                        }

                        _lightValues[x, y] = lightLevel;
                    }
                }
                
                //add data from array to the lightmap
                for (var x = rootX; x < stopX; x++)
                    for (var y = rootY; y < stopY; y++)
                        lightMap.SetPixel(x, y, new Color(0,0,0, _lightValues[x, y] / 15));
            }
            
            //apply and set texture
            lightMap.Apply();
            lightShader.SetTexture("_LightMap", lightMap);
        }

        /// <summary>
        /// Updates lighting in a certain radius, fixes lag somehow lol
        /// </summary>
        /// <param name="x">Mouse position X</param>
        /// <param name="y">Mouse position Y</param>
        public void RedrawLighting(int x = -1, int y = -1)
        {
            if(x is -1 && y is -1)
                UpdateLighting(50, 0, 0, TerrainConfig.Settings.worldSize.x, TerrainConfig.Settings.worldSize.y);
            else 
                UpdateLighting(25, 
                    Mathf.Clamp(x - editRadius - 2, 0, TerrainConfig.Settings.worldSize.x),
                    Mathf.Clamp(y - editRadius - 2, 0, TerrainConfig.Settings.worldSize.y),
                    Mathf.Clamp(x + editRadius + 2, 0, TerrainConfig.Settings.worldSize.x),
                    Mathf.Clamp(y + editRadius + 2, 0, TerrainConfig.Settings.worldSize.y));
        }
    }

    public enum LightingModes
    {
        Point,
        Bilinear,
        Trilinear
    }
}