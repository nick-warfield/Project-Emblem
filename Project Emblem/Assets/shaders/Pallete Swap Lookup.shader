﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprite/PaletteSwapLookup"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_PaletteTex("Texture", 2D) = "white" {}
	}
	
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _PaletteTex;

			fixed4 frag(v2f i) : SV_Target
			{
				float x = tex2D(_MainTex, i.uv).r;
				float y = tex2D(_MainTex, i.uv).a;
				return tex2D(_PaletteTex, float2(x, y));
			}

			ENDCG
		}
	}
}