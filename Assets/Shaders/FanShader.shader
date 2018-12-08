Shader "Unlit/FanShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightDirection("LightDirection", Vector) = (0, 0, 0)
		_Ambient("Ambient", Vector) = (0, 0, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float3 _LightDirection;
			uniform float3 _Ambient;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = mul(unity_ObjectToWorld, v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float x = dot(normalize(i.normal), normalize(_LightDirection));
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * x + fixed4(_Ambient, 1);
			}
			ENDCG
		}
	}
}
