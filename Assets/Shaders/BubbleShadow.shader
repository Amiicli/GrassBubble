Shader "Milan/Bubbleshadow"{
	//show values to edit in inspector
	Properties
    {
		_Color ("Color", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
    {
		Tags{ "RenderType"="Transparent" "Queue"="Transparent-400" "DisableBatching"="True"}

		//Blend via alpha
		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite off

		Pass{
			CGPROGRAM

			//include useful shader functions
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;

			sampler2D_float _CameraDepthTexture;

			//the mesh data thats read by the vertex shader
			struct appdata
            {
				float4 vertex : POSITION;
			};

			//the data thats passed from the vertex to the fragment shader and interpolated by the rasterizer
			struct v2f
            {
				float4 position : SV_POSITION;
				float4 screenPos : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			float3 getProjectedObjectPos(float2 screenPos, float3 worldRay)
            {
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPos);
				depth = Linear01Depth (depth) * _ProjectionParams.z;
				worldRay = normalize(worldRay);
				worldRay /= dot(worldRay, -UNITY_MATRIX_V[2].xyz);
				float3 worldPos = _WorldSpaceCameraPos + worldRay * depth;
				float3 objectPos =  mul (unity_WorldToObject, float4(worldPos,1)).xyz;
				clip(0.5 - abs(objectPos));
				objectPos += 0.5;
				return objectPos;
			}

			v2f vert(appdata v)
            {
				v2f o;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                
				o.position = UnityWorldToClipPos(worldPos);
				o.ray = worldPos - _WorldSpaceCameraPos;
				o.screenPos = ComputeScreenPos (o.position);
				return o;
			}


			fixed4 frag(v2f i) : SV_TARGET
            {
				float2 screenUv = i.screenPos.xy / i.screenPos.w;
				float2 uv = getProjectedObjectPos(screenUv, i.ray).xz;
				fixed4 col = tex2D(_MainTex, uv);
				col *= _Color;
				return col;
			}

			ENDCG
		}
	}
}