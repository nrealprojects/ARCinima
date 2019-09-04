#include "UnityCG.cginc" 
//#define DistanceToProjectionWindow 5.671281819617709             //1.0 / tan(0.5 * radians(20));
//#define DPTimes300 1701.384545885313                             //DistanceToProjectionWindow * 300

#define SamplerSteps 25
#define SSSS_FOVY 20

uniform sampler2D _CameraDepthTexture;
float4 _CameraDepthTexture_TexelSize;

uniform sampler2D _MainTex;
uniform float4 _MainTex_ST;
uniform float _SSSScale;
uniform float4 _Kernel[SamplerSteps];
            
struct VertexInput {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};
struct VertexOutput {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
};
VertexOutput vert (VertexInput v) {
    VertexOutput o;
    o.pos = v.vertex;
    o.uv = v.uv;
    return o;
}

//float4 SSS(float4 SceneColor, float2 UV, float2 SSSIntencity) {
//    float SceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UV));                                   
//    float BlurLength = DistanceToProjectionWindow / SceneDepth;                                   
//    float2 UVOffset = SSSIntencity * BlurLength;                      
//        float4 BlurSceneColor = SceneColor;
//    BlurSceneColor.rgb *=  _Kernel[0].rgb;  

//    [loop]
//    for (int i = 1; i < SamplerSteps; i++) {
//        float2 SSSUV = UV +  _Kernel[i].a * UVOffset;
//        float4 SSSSceneColor = tex2D(_MainTex, SSSUV);
//        float SSSDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, SSSUV)).r;         
//        float SSSScale = saturate(DPTimes300 * SSSIntencity * abs(SceneDepth - SSSDepth));
//        SSSSceneColor.rgb = lerp(SSSSceneColor.rgb, SceneColor.rgb, SSSScale);
//        BlurSceneColor.rgb +=  _Kernel[i].rgb * SSSSceneColor.rgb;
//    }
//    return BlurSceneColor;
//}


float4 SSS(float4 SceneColor, float2 UV, float2 sssWidth, float2 dir) {
    //float SceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UV));   
	                              
    //float BlurLength = DistanceToProjectionWindow / SceneDepth;                                   
    //float2 UVOffset = SSSIntencity * BlurLength;      

	//float depthM = SSSSSamplePoint(depthTex, texcoord).r;
	//float4 colorM = tex2D(_MainTex, UV);

	float depthM =  Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UV)).r;
	float DistanceToProjectionWindow = 1.0 / tan(0.5 * radians(SSSS_FOVY));  
    float scale = DistanceToProjectionWindow / depthM;

	float2 finalStep = sssWidth * scale * dir; 
	//float2 finalStep = SceneColor.a * 20/depthM; 

	finalStep *= SceneColor.a;
	finalStep *= 1.0 / 3.0;
	               
    float4 BlurSceneColor = SceneColor;
    BlurSceneColor.rgb *=  _Kernel[0].rgb;  

    [loop]
    for (int i = 1; i < SamplerSteps; i++) {
        float2 offset = UV +  _Kernel[i].a * finalStep;
        float4 SSSSceneColor = tex2D(_MainTex, offset);

      //float SSSDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset)).r;         
        float SSSDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset)).r;         

        float SSSScale = saturate(300.0 * DistanceToProjectionWindow * sssWidth * abs(depthM - SSSDepth));
        SSSSceneColor.rgb = lerp(SSSSceneColor.rgb, SceneColor.rgb, SSSScale);
        BlurSceneColor.rgb += _Kernel[i].rgb * SSSSceneColor.rgb;
    }
    return BlurSceneColor;
}

// Separable SSS Transmittance Function
//float4 SSSSTransmittance(float translucency, float sssWidth, float3 worldPosition, float3 worldNormal, float3 light, float4x4 lightViewProjection, float lightFarPlane) 
//{
//	float scale = 8.25 * (1.0 - translucency) / sssWidth;
//	float4 shrinkedPos = float4(worldPosition - 0.005 * worldNormal, 1.0);

//	float4 

//	float4 shadowPosition = mul(shrinkedPos, lightViewProjection);
//    //float d1 = tex2D(shadowMap, shadowPosition.xy / shadowPosition.w).r; // 'd1' has a range of 0..1
//    float d1 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, shadowPosition.xy / shadowPosition.w).r;
//    float d2 = shadowPosition.z; // 'd2' has a range of 0..'lightFarPlane'
//    d1 *= lightFarPlane; // So we scale 'd1' accordingly:
//    float d = scale * abs(d1 - d2);

//	float dd = -d * d;
//    float3 profile = float3(0.233, 0.455, 0.649) * exp(dd / 0.0064) +
//                     float3(0.1,   0.336, 0.344) * exp(dd / 0.0484) +
//                     float3(0.118, 0.198, 0.0)   * exp(dd / 0.187)  +
//                     float3(0.113, 0.007, 0.007) * exp(dd / 0.567)  +
//                     float3(0.358, 0.004, 0.0)   * exp(dd / 1.99)   +
//                     float3(0.078, 0.0,   0.0)   * exp(dd / 7.41);

//	return profile * saturate(0.3 + dot(light, -worldNormal));
//}