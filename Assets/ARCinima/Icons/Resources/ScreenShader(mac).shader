﻿Shader "VideoPlayer/ScreenMac" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	}

		SubShader{
			Tags {"Queue" = "Transparent-100" "IgnoreProjector" = "True" "RenderType" = "Opaque"}
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_FOG_COORDS(1)
						UNITY_VERTEX_OUTPUT_STEREO
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;

					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						float2 uv;
						uv.x = i.texcoord.x;
						uv.y = 1 - i.texcoord.y;
						fixed4 col = tex2D(_MainTex, uv);
						return col;
					}
				ENDCG
			}
	}
}