Shader "Unlit/Spritesheet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [MainColor]
        _Color ("Tint", Color) = (1,1,1,1)
        _Columns ("Columns", Int) = 1
        _Rows ("Rows", Int) = 1
        _AnimationSpeed("FPS", float) = 10
        _ManualIndex("Manual index", Int) = 1
        _Seed ("Seed", float) = 10
        _Wobble ("Wobble", float) = 10
        _WobbleSpeed ("Wobble Speed", float) = 10

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite On
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
                float4 color : COLOR; 
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR; 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float seed;
            float _Wobble;
            float _WobbleSpeed;
            int _Rows;
            float _AnimationSpeed;
            float _Columns;
            float _ManualIndex;

            v2f vert (appdata v)
            {
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float offsetInput = (_Time.y + frac(v.vertex.x)) * _WobbleSpeed * (3.14 * 2) + v.vertex.x / 2;
                v.vertex.y += sin(offsetInput  + (seed * 2000 + clipPos.x + clipPos.y) + v.vertex.x) * _Wobble;
                v.vertex.x += cos(offsetInput + v.vertex.x) * _Wobble;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;


				float2 size = float2(1.0f / _Columns, 1.0f / _Rows);
				uint totalFrames = _Columns * _Rows;

				// use timer to increment index
				uint index = (_Time.y*_AnimationSpeed) + _ManualIndex;

				// wrap x and y indexes
				uint indexX = index % _Columns;
				uint indexY = floor((index % totalFrames) / _Columns);

				// get offsets to our sprite index
				float2 offset = float2(size.x*indexX,-size.y*indexY);

				// get single sprite UV
				float2 newUV = v.uv*size;

				// flip Y (to start 0 from top)
				newUV.y = newUV.y + size.y*(_Rows - 1);

				o.uv = newUV + offset;
                return o;
            }
			float2 SpriteSheetAnimationUV(float2 uv, float size, float speed)
			{
				uv /= size;
				uv.y += floor(fmod(_Time * speed, 1.0) * size) / size;
				uv.x += 1/size;
				uv.x += (1 - floor(fmod(_Time * speed / size, 1.0) * size) / size);
				return uv;
			}
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // float2 uv = SpriteSheetAnimationUV(i.uv,4,4);
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                col *= i.color;
                return col;
            }
            ENDCG
        }
    }
}
