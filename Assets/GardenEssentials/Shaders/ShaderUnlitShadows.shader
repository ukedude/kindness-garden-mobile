// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Garden/Unlit With Shadows"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		_ShadowStrength ("Shadow Strength", Range(0,1)) = 0.12
		_StencilRef("Stencil Ref",Int) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"  "Queue"="Geometry"}

		Pass
		{
			Stencil {
                Ref [_StencilRef]
                Comp NotEqual
                Pass Zero
            }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"
			#include "AutoLight.cginc"	

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				LIGHTING_COORDS(1,2)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _ShadowColor;
			fixed _ShadowStrength;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed attenuation = SHADOW_ATTENUATION(i);

				fixed4 col = tex2D(_MainTex, i.uv)*_Color;

				return lerp(_ShadowColor,col,clamp(_ShadowStrength+attenuation,0,1));
			}

			ENDCG
		}
	}

	Fallback "VertexLit"
}
