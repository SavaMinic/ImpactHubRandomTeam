Shader "Demo/FanShader"
{
	Properties
	{
		_LightDirection("LightDirection", Vector) = (0, 0, 0)
		_LightIntensity("LightIntensity", Range(0, 5.0)) = 2
		_LightColor("LightColor", Color) = (1, 1, 1, 0)
		_InputTexture("InputTexture", 2D) = "white" {}
		_AmbientColor("AmbientColor", Color) = (0, 0, 0, 0)
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
			
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 world : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};
			
			uniform float3 _LightDirection;
			uniform float _LightIntensity;
			uniform sampler2D _InputTexture;
			uniform float3 _AmbientColor;
			uniform float3 _LightColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, v.normal);
				o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 color = tex2D(_InputTexture, i.uv).rgb;
				color = pow(color, 2.2);
				float3 diffusePart = dot(normalize(i.normal), normalize(_LightDirection)) * _LightColor * _LightIntensity * color;
				float3 ambientPart = _AmbientColor * color;
				return fixed4(pow(ambientPart + diffusePart, 0.4545), 1);
			}
			ENDCG
		}
	}
}
