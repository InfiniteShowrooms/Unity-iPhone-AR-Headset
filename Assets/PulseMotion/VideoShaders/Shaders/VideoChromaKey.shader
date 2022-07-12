Shader "Video/Chroma Key"
{
	Properties
	{
        _Color("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
        _ChromaKey("Chroma Key", Color) = (0,1,0,1)
        _ChromaThreshold("Chroma Threshold", Range(0.0, 1.0)) = 0.8
        _ChromaSlope("Chroma Slope", Range(0.0, 1.0)) = 0.2
	}
	SubShader
	{
		Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
		LOD 100
        Cull Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_nicest
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

            uniform fixed4 _Color;
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform float4 _ChromaKey;
            uniform fixed _ChromaThreshold;
            uniform fixed _ChromaSlope;
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
                float4 c = tex2D(_MainTex, i.uv);

                /*
                float dist = length(_ChromaKey.rgb - c.rgb);
                float edge = _ChromaThreshold * (1.0 - _ChromaSlope);
                c.a = smoothstep(edge, _ChromaThreshold, dist);
                */

                fixed edge = _ChromaThreshold * (1.0 - _ChromaSlope);
                fixed3 u = _ChromaKey 
                    + dot(_ChromaKey.rgb, c.rgb) * fixed3(1, 1, 1)
                    - (_ChromaKey.r + _ChromaKey.g + _ChromaKey.b) * c;

                fixed3 v = smoothstep(edge, _ChromaThreshold, u);
                c.a = 1.0f - v.r * v.g * v.b;
                return c;
			}
			ENDCG
		}
	}
}
