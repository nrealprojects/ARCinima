using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[AddComponentMenu ("PengLu/ImageEffect/BrokenScreen")]
public class ImageEffect_BrokenScreen : MonoBehaviour {
	#region Variables
	public Shader BrokenScreenShader = null;
	private Material BrokenScreenMaterial = null;

	public Texture2D BumpMap;
	// private RenderTextureFormat rtFormat = RenderTextureFormat.Default;

	[Range(0.0f, 1.0f)]
	public float satCount = 0.17f;



	#endregion
	

void Start () {
		FindShaders ();
		CheckSupport ();
		CreateMaterials ();
	}

	void FindShaders () {
		if (!BrokenScreenShader) {
			BrokenScreenShader = Shader.Find("PengLu/ImageEffect/Unlit/BrokenScreen");
		}
	}

	void CreateMaterials() {
		if(!BrokenScreenMaterial){
			BrokenScreenMaterial = new Material(BrokenScreenShader);
			BrokenScreenMaterial.hideFlags = HideFlags.HideAndDontSave;	
		}
	}

	bool Supported(){
		return (SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && BrokenScreenShader.isSupported);
	}


	bool CheckSupport() {
		if(!Supported()) {
			enabled = false;
			return false;
		}
		return true;
	}



	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{	
		#if UNITY_EDITOR
			FindShaders ();
			CheckSupport ();
			CreateMaterials ();	
		#endif

		float scaleX , scaleY ;

		if(sourceTexture.width > sourceTexture.height)
		{	
			scaleX = 1.0f;
			scaleY = (float) sourceTexture.height / (float) sourceTexture.width;			
		}		
		else
		{
			scaleX = (float) sourceTexture.width / (float) sourceTexture.height;
			scaleY = 1.0f;	
		}
		

		print("scaleX:-------" + scaleX + " 	" + "scaleY:-------" + scaleY);

		if(BumpMap != null){
			
	        BrokenScreenMaterial.SetFloat ("_satCount", satCount);
	        BrokenScreenMaterial.SetFloat ("_scaleX", scaleX);
	        BrokenScreenMaterial.SetFloat ("_scaleY", scaleY);
	 		BrokenScreenMaterial.SetTexture ("_BumpTex", BumpMap);
	   		Graphics.Blit (sourceTexture, destTexture, BrokenScreenMaterial,0);

		}
		else {

			Graphics.Blit (sourceTexture, destTexture);

		}



		
		
	}
	
	 public void OnDisable () {
        if (BrokenScreenMaterial)
            DestroyImmediate (BrokenScreenMaterial);
            // BrokenScreenMaterial = null;
    }
}
