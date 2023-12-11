Shader "Unlit/Texturerepeattest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollTime ("Scroll time", Range(0,1)) = 1
        _DivisionSection ("Division Section", Integer) = 1
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScrollTime;
            int _DivisionSection;

            v2f vert (appdata v)
            {
                v2f o;
                // v.uv.y = frac(v.uv.y * 8);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                _ScrollTime = _ScrollTime * 0.125;
                float section = _DivisionSection * 0.125;
                i.uv.y = 1 - (frac(i.uv.y * 0.125)  + section + _ScrollTime);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
