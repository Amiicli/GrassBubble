Shader "Unlit/vat_instancing"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _PositionTexture ("Position texture", 2D) = "" {}
        _minBounds ("Minimum Bounds", Range(-10,0)) = 1
        _maxBounds ("Maximum Bounds", Range(0,10)) = 1
        _Frame ("Frame", Range(0,1)) = 1
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
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural  
            #pragma target 2.5
            #pragma require samplelod

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2: TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };



            struct Grasshopper
            {
                float3 position;
                float3 color;
                float scale;
                float direction;
                float spinDirection;
                float radians;          // reuse variables to save space
                float forwardVelocity; // init x 
                float upwardVelocity; // init y
                float jumpWaitTime; // init z
                float temp;
                
                int state;
                float timer;
                int bubbleParent; 
                int seed;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
                UNITY_DEFINE_INSTANCED_PROP(float, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float, _Rotation)
            UNITY_INSTANCING_BUFFER_END(Props)

            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                StructuredBuffer<Grasshopper> grasshoppers;
            #endif
            void ConfigureProcedural ()
            {
                #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)

                #endif
            }
            v2f vert (appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
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
