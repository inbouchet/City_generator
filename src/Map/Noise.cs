using UnityEngine;
using System.Collections;

public static class Noise {

	public static float[,] GenerateNoiseMap(int largeur, int longueur, int graine, float scale, int octaves, float persistance, float lacunarite) {
		float[,] noiseMap = new float[largeur,longueur];

		System.Random prng = new System.Random (graine);
		Vector2[] octaveOffsets = new Vector2[octaves];


		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseLongueur = float.MinValue;
		float minNoiseLongueur = float.MaxValue;

		float halfLargeur = largeur / 2f;
		float halfLongueur = longueur / 2f;


		for (int y = 0; y < longueur; y++) {
			for (int x = 0; x < largeur; x++) {
		
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++) {
					float sampleX = (x-halfLargeur) / scale * frequency ;
					float sampleY = (y-halfLongueur) / scale * frequency ;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarite;
				}

				if (noiseHeight > maxNoiseLongueur) {
					maxNoiseLongueur = noiseHeight;
				} else if (noiseHeight < minNoiseLongueur) {
					minNoiseLongueur = noiseHeight;
				}
				noiseMap [x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < longueur; y++) {
			for (int x = 0; x < largeur; x++) {
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseLongueur, maxNoiseLongueur, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}

}