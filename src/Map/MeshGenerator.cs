using UnityEngine;
using System.Collections;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] longueurMap, float hauteurMultiplicateur, int simple) {
		int largeur = longueurMap.GetLength (0);
		int longueur = longueurMap.GetLength (1);
		float topLeftX = (largeur - 1) / -2f;
		float topLeftZ = (longueur - 1) / 2f;

		MeshData meshData = new MeshData (largeur, longueur);
		int vertexIndex = 0;

		int Simplificateur = (simple == 0)?1:simple*2;
		int verticesLigne = (largeur - 1) / Simplificateur + 1; 

		for (int y = 0; y < longueur; y+=Simplificateur) {
			for (int x = 0; x < largeur; x+=Simplificateur ) {

				meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, longueurMap[x,y] * hauteurMultiplicateur, topLeftZ - y);
				meshData.uvs [vertexIndex] = new Vector2 (x / (float)largeur, y / (float)longueur);

				if (x < largeur - 1 && y < longueur - 1) {
					meshData.AddTriangle (vertexIndex, vertexIndex + verticesLigne + 1, vertexIndex + verticesLigne);
					meshData.AddTriangle (vertexIndex + verticesLigne + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		return meshData;

	}
}

public class MeshData {
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;

	int triangleIndex;

	public MeshData(int meshlargeur, int meshlongueur) {
		vertices = new Vector3[meshlargeur * meshlongueur];
		uvs = new Vector2[meshlargeur * meshlongueur];
		triangles = new int[(meshlargeur-1)*(meshlongueur-1)*6];
	}

	public void AddTriangle(int a, int b, int c) {
		triangles [triangleIndex] = a;
		triangles [triangleIndex + 1] = b;
		triangles [triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateNormals ();
		return mesh;
	}

}