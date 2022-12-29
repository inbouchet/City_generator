using UnityEngine;
using System.Collections;

namespace MD
{
public class MapDisplay {

	private Renderer textureRender;
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;

	public MapDisplay(Renderer textureRender, MeshFilter meshFilter, MeshRenderer meshRenderer)
	{
		this.textureRender = textureRender;
		this.meshFilter = meshFilter;
		this.meshRenderer = meshRenderer;
	}

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height);
	}

	public void DrawMesh(MeshData meshData){//, Texture2D texture) {
		meshFilter.sharedMesh = meshData.CreateMesh ();
		//meshRenderer.sharedMaterial.mainTexture = texture;
	}

}
}