Shader "Unlit/VAT Test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Range(1,50)) = 10
        _Position ("Position texture", 2D) = "" {}
        _Normal ("Normal texture", 2D)  = "" {}
        _vatScale ("Scale", Range(0,100)) = 1
        _Frame ("Frame", Range(0,100)) = 1
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
            float4 _MainTex_ST;
            float4 _Color; 
            float4 _SpecColor; 
            float _Shininess;
            float _Speed;
            float _vatScale;
            float _Frame;
            sampler2D _RoughMask;
            sampler2D _Position;
            float4 _Position_TexelSize;
            sampler2D _Normal;
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

                // float texture_width = _Position_TexelSize.z;
                float texture_width = 128;
                int num_frames = 40;
                // float frame = 0.0 + sin(_Time.y) * 10;
                float frame = _Frame;

                float pixel = 1.0 / texture_width;
                float halfPixel = pixel * 0.5;
                float framePixelSize = 1.0 / num_frames;

                o.uv = v.uv;
                float3 pos = tex2Dlod(_Position, float4(v.uv2 + float2(halfPixel, -((frame + 0.5) * framePixelSize)), 0.0, 0.0)).xyz;
                float3 nrm = tex2Dlod(_Normal, float4(v.uv2 + float2(halfPixel, -((frame + 0.5) * framePixelSize)), 0.0, 0.0)).xyz;

                float new_x = ((pos.x * 2.0) - 1.0) * 1.0;
                float new_y = (pos.z * 2.0) - 1.0;
                float new_z = ((pos.y * 2.0) - 1.0) * -1.0;
                
                float nrm_x = (nrm.x * 2.0) - 1.0;
                float nrm_y = (nrm.z * 2.0) - 1.0;
                float nrm_z = ((nrm.y * 2.0) - 1.0) * -1.0; 

                nrm = float3(nrm_x, nrm_y, nrm_z);
                pos = float3(new_x, new_y, new_z) * _vatScale;
                
                v.vertex.xyz = pos;
                v.normal.xyz = nrm;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.light = GetGouraudLighting(v.normal,v.vertex, _Color, _SpecColor, _Shininess);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 grassgrid = tex2D(_MainTex, i.uv).rgb;
                return float4(grassgrid,1);
            }
            
            ENDCG
        }
    }
}
