Shader "Unlit/BasicInstancing"
{
	Properties
	{
		_Color ("Color", 2D) = "white"
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags { "LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			// Enable gpu instancing variants.
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv_Color  : TEXCOORD0;

				// Need this for basic functionality.
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv_Color : TEXCOORD00;
				float3 world : TEXCOORD01;
			};

			uniform sampler2D _Color;
            uniform float4 _Color_ST;

			v2f vert (appdata v)
			{
				v2f o;

				// Need this for basic functionality.
				UNITY_SETUP_INSTANCE_ID(v);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_Color = TRANSFORM_TEX(v.uv_Color, _Color);
				o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float factor = 0.55 + i.world.y / 130;
				return tex2D(_Color, i.uv_Color) * factor;
			}
			ENDCG
		}
	}
}
