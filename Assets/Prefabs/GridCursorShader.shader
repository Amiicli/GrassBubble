Shader "Unlit/GridCursorShader"
{
    Properties
    {
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Range(1,50)) = 10
        _Speed ("Rotation Speed", Range(-20,20)) = 10
        [KeywordEnum(SPIN, HOVER)] _MoveType("Move Type", Float) = 0
        _Scale ("Scale", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Stencil
        {
            Ref 1 // ReferenceValue = 1
            Comp Always // Comparison Function - Make the stencil test always pass.
            Pass Replace // Write the reference value into the buffer.
        }

        CGINCLUDE
            #pragma shader_feature_local  _MOVETYPE_SPIN _MOVETYPE_HOVER
            #include "UnityCG.cginc"
            #include "Milan.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
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
            float4 _Color; 
            float4 _SpecColor; 
            float _Shininess;
            float _Speed;
            float _Scale;
            sampler2D _RoughMask;
        ENDCG
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            struct structurePS //Thank you Victoria/Action Dawg for the help
            {
                fixed4 target00 : SV_Target0;
                float2 target01 : SV_Target1;
            };

            float2x2 GetRotMatrix(float angle,float4 vertex)
            {
                float c = cos(angle);
                float s = sin(angle);
                float4 worldPos = mul(unity_ObjectToWorld, vertex);
                return float2x2(c, -s, s, c);
            }

            v2f vert (appdata v)
            {
                v2f o;
                float speedMult = 0;
                speedMult = _Speed * 10;
                #ifdef _MOVETYPE_SPIN
                float angle = radians(_Time.y * speedMult);
                float2x2 rotationMatrix = GetRotMatrix(angle,v.vertex);
                v.vertex.xz = mul(rotationMatrix, v.vertex.xz);                
                #elif _MOVETYPE_HOVER
                speedMult = _Speed * 5;
                float angle = radians(_Time.y * speedMult);
                float2x2 rotationMatrix = GetRotMatrix(angle,v.vertex);
                v.vertex.xz = mul(rotationMatrix, v.vertex.xz);

                speedMult = _Speed * 0.3;
                v.vertex.y += sin(_Time.y * speedMult) * 0.1;
                #endif
                float3 scaledPosition = float3(v.vertex.x * _Scale, v.vertex.y * _Scale, v.vertex.z * _Scale);
                o.vertex = UnityObjectToClipPos(scaledPosition);
                o.light = GetGouraudLighting(v.normal,v.vertex, _Color, _SpecColor, _Shininess);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            half4 frag (v2f i) : SV_Target
            {
                return i.light;
            }
            ENDCG
        }
    }
}
