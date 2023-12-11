Shader "Unlit/VAT Test 2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Range(1,50)) = 10
        _minBounds ("Minimum Bounds", Range(-10,0)) = 1
        _maxBounds ("Maximum Bounds", Range(0,10)) = 1
        _PositionTexture ("Position texture", 2D) = "" {}
        _Frame ("Frame", Range(0,1)) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        CGINCLUDE

            #include "UnityCG.cginc"
            #include "Milan.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float4 light : COLOR; 
            };
            sampler2D _MainTex;
            sampler2D _PositionTexture;
            float4 _MainTex_ST;
            float4 _PositionTexture_ST;
            float4 _Color; 
            float4 _SpecColor; 
            float _Shininess;
            float _Speed;
            float _minBounds;
            float _maxBounds;
            float _Frame;
        ENDCG
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                float speed = 1;
                float vertexID = v.uv2.x;
                float textureTime = _Frame;

                float4 uv = float4(vertexID, textureTime, 0, 0);
                float4 currentAnimationTexturePos = tex2Dlod(_PositionTexture, uv);
                
                float3 finalPosition = lerp(_minBounds, _maxBounds, currentAnimationTexturePos.xyz);
 
                v.vertex.xyz = finalPosition;
                

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.light = GetGouraudLighting(v.normal,v.vertex, _Color, _SpecColor, _Shininess);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 grassgrid = tex2D(_MainTex, i.uv).rgb;
                grassgrid *= i.light;
                return float4(grassgrid,1);
            }
            
            ENDCG
        }
    }
}
