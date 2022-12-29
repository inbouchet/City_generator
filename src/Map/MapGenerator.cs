using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gen
{
	public class MapGenerator {

		public int mapWidth;
		public int mapHeight;
		public float noiseScale = 25;
		public float hauteurMultiplicateur = 10;
		public AnimationCurve meshHeightCurve;

		public int octaves;
		[Range(0,1)]
		public float persistance;
		public float lacunarity;

		public int seed;
		public Vector2 offset;

		private float[,] noiseMap;

		public MapGenerator(List<Vector2> pointsTowers) {
			//MapDisplay display = FindObjectOfType<MapDisplay> ();
			noiseMap = Noise.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
			//applatirTerrain(pointsTowers);
			//display.DrawMesh (MeshGenerator.GenerateTerrainMesh (noiseMap, hauteurMultiplicateur, meshHeightCurve)); 
		}

		public void applatirTerrain(List<Vector2> pointsTowers){
			pointsTowers = Geometry.MakePolygone(pointsTowers);
			int width = noiseMap.GetLength (0);
			int height = noiseMap.GetLength (1);
			for (int y = 0 ; y < height ; y++){
				for (int x = 0; x < width; x++){
					noiseMap[x,y] = 0;
					//Vector2 vec = new Vector2(x, y);
					//if(Geometry.IsInside(pointsTowers, vec))
				}
			}
		}

		void OnValidate() {
			if (mapWidth < 1) {
				mapWidth = 1;
			}
			if (mapHeight < 1) {
				mapHeight = 1;
			}
			if (lacunarity < 1) {
				lacunarity = 1;
			}
			if (octaves < 0) {
				octaves = 0;
			}
			if (hauteurMultiplicateur < 0)
				hauteurMultiplicateur = 0;
		}
	}

	[System.Serializable]
	public struct TerrainType {
		public string name;
		public float height;
		public Color colour;
	}
}