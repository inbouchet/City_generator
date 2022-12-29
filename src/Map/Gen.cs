using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MG
{
public class MapGenerator {

	float persistance;

	int TailleTerrain, octaves, seed;

	const int simpleMesh = 6;
	const int lacunarity = 1;
	const float noiseScale =  250.0f;


	public float[,] noiseMap;

	private Renderer textureRender;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	private int hauteurMultiplicateur;


	public MapGenerator(int TailleTerrain, Renderer textureRender, MeshFilter meshFilter, MeshRenderer meshRenderer) {
		this.TailleTerrain = TailleTerrain;
		this.seed = Random.Range(0, 100);
		this.persistance = Random.Range(0f, 0.9f);
		this.hauteurMultiplicateur = Random.Range(30, 40);
		this.octaves = Random.Range(80, 100);
		this.textureRender = textureRender;
		this.meshFilter = meshFilter;
		this.meshRenderer = meshRenderer;
		
		noiseMap = Noise.GenerateNoiseMap (TailleTerrain, TailleTerrain, seed, noiseScale, octaves, persistance, lacunarity);
		//randomRivers();
	}

	public void drawTerrain(){
		MD.MapDisplay display = new MD.MapDisplay(textureRender,meshFilter,meshRenderer);
		display.DrawMesh (MeshGenerator.GenerateTerrainMesh (noiseMap, hauteurMultiplicateur, simpleMesh));
	}

	public float getHeight(int x, int y){
		return noiseMap[x,y];
	}

	public Vector2 randomPoints(){
		int x = Random.Range(1, TailleTerrain-1);
		int y = Random.Range(1, TailleTerrain-1);
		Vector2 point = new Vector2(x,y);
		return point;
	}

	public void setHeight(int x, int y, float height){
		noiseMap[x,y] = height;
	}

	public Vector2 getHigherPoint(){
		float higher = float.MinValue;
		int x1 = 0;
		int y1 = 0;
		for (int y = 0; y < TailleTerrain; y++){
			for (int x = 0; x < TailleTerrain; x++){
				if (noiseMap[x,y] > higher){
					higher = noiseMap[x,y];
					x1 = x;
					y1 = y;
				}
			}
		}
		Vector2 point = new Vector2(x1,y1);
		return point;
	}

	public float[,] copyNoise(){
		float[,] noiseCopy = new float[TailleTerrain,TailleTerrain];
		for (int y = 0; y < TailleTerrain; y++){
			for (int x = 0; x < TailleTerrain; x++){
				noiseCopy[x,y] = noiseMap[x,y];
			}
		}
		return noiseCopy;
	}

	public void flatTerrainVillage(int distance){
		distance += 30; //laisser de la place pour les portes
		int cx = TailleTerrain/2; 
		int cy = TailleTerrain/2;
		for (int y = 0; y < TailleTerrain; y++){
			for (int x = 0; x < TailleTerrain; x++){
				Vector2 point = new Vector2(x,y);
				float dis_point = Mathf.Sqrt((Mathf.Pow(cy-y, 2) + Mathf.Pow(cx -x, 2)));
				if (dis_point < distance)
					noiseMap[x,y]=noiseMap[x,y]/3;
				if (dis_point> distance && dis_point < (distance+100))
					noiseMap[x,y]=noiseMap[x,y]/2; 
			}
		}
	}

	public void flatTerraindoors(List<Vector2> pointDoors){
		foreach (Vector2 door in pointDoors){
			int x = (int)door.x;
			int y = (int)door.y;
			for (int i = -5; i <= 5; i++){
				for (int l = -5; l <= 5; l++){
					//if (x+l > 0 && x+l < TailleTerrain && y+i > 0 && y+i < TailleTerrain)
					noiseMap[x+l,y+i] = 0;
				}
			}
		}
	}

	public void flatNoise(){
		for (int v = 0; v<TailleTerrain; v++){
			for (int s = 0; s<TailleTerrain; s++){
				noiseMap[s,v] = 0;
			}
		}
	}	

	public void pathRiver(Vector2 debut){
		float[,] modifNoise = copyNoise();

		int x = (int) debut.x;
		int y = (int) debut.y;
		
		int tempX = 0;
		int tempY = 0;

		bool minFound = true;
		int arret = 180;

		float minimumPr = float.MaxValue;
		float minimumFound = float.MaxValue;
		int border = 3;

		while (minFound || arret !=0){
			if (x-1 > border && noiseMap[x-1, y] < minimumPr){
				minimumPr = noiseMap[x-1, y];
				tempX = x-1;
				tempY = y;
			}
			if (y-1 > border && noiseMap[x, y-1] < minimumPr){
				minimumPr = noiseMap[x, y-1];
				tempX = x;
				tempY = y-1;
			}
			if (x+1 < TailleTerrain-border && noiseMap[x+1, y] < minimumPr){
				minimumPr = noiseMap[x+1, y];
				tempX = x+1;
				tempY = y;
			}
			if (y+1 < TailleTerrain-border && noiseMap[x, y+1] < minimumPr){
				minimumPr = noiseMap[x, y+1];
				tempX = x;
				tempY = y+1;
			}
			if (x-1 > 1 && y-1 > border && noiseMap[x-1, y-1] < minimumPr){
				minimumPr = noiseMap[x-1, y-1];
				tempX = x-1;
				tempY = y-1;
			}
			if (x-1 > 1 && y+1 < TailleTerrain-border && noiseMap[x-1, y+1] < minimumPr){
				minimumPr = noiseMap[x-1, y-1];
				tempX = x-1;
				tempY = y+1;
			}
			if (x+1 > 1 && y-1 > border && noiseMap[x+1, y-1] < minimumPr){
				minimumPr = noiseMap[x+1, y-1];
				tempX = x-1;
				tempY = y+1;
			}
			if (x+1 < TailleTerrain-border && y+1 < TailleTerrain - border  && noiseMap[x+1, y+1] < minimumPr){
				minimumPr = noiseMap[x+1, y+1];
				tempX = x+1;
				tempY = y+1;
			}


			for (int s = -border; s <= border; s++){
				for (int v = -border; v <= border; v++){
					modifNoise[tempX-v, tempY-s] = 0;

				}
			}

			x = tempX;
			y = tempY; 

			if (minimumPr >= minimumFound)
				minFound = false;
			
			minimumFound = minimumPr;
			arret-=1;

		}

		for (int v = 0; v<TailleTerrain; v++){
			for (int s = 0; s<TailleTerrain; s++){
				noiseMap[s,v] = modifNoise[s,v];
			}
		}
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}
}