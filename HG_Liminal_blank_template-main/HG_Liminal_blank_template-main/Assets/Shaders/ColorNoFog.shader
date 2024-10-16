﻿Shader "Unlit/Color NoFog" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					fixed4 _Color;

					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						return o;
					}

					fixed4 frag(v2f i) : COLOR
					{
						fixed4 col = _Color;
						UNITY_OPAQUE_ALPHA(col.a);
						return col;
					}
				ENDCG
			}
	}

}
