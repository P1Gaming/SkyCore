

Shader "Custom/LineFor2DEditor"
{
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "RenderPipeline" = "UniversalPipeline"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Off



		Pass
		{
			//CGPROGRAM
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			//#include "UnityCG.cginc"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float4 _Color;
			float4 _ColorFullyTransparent;
			float _FractionWidthBeforeAntialias;
			CBUFFER_END

			// add antialiasing to the ends, because will use this for line segments

			/*struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};*/
			struct Attributes 
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
			};

			/*struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};*/
			struct Varyings
			{
				float4 positionHCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			/*v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.x = (-0.5 + v.uv.x) * 2;
				o.uv.y = (-0.5 + v.uv.y) * 2;
				return o;
			}*/
			Varyings vert(Attributes IN)
			{
				Varyings OUT;
				OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
				OUT.uv.x = (-0.5 + IN.uv.x) * 2;
				OUT.uv.y = (-0.5 + IN.uv.y) * 2;
				return OUT;
			}



			half4 frag(Varyings IN) : SV_Target
			{
				half tForLerp = smoothstep(_FractionWidthBeforeAntialias, 1.0, abs(IN.uv.y));
				return lerp(_Color, _ColorFullyTransparent, tForLerp);
			}

			//ENDCG
			ENDHLSL
		}

	}

}


