Shader "Milan/Bubble"
{
    Properties
    {
        _MatCap ("Matcap", 2D) = "white" {}
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Noise ("Noise", 2D) = "white" {}
        _WobbleStrength ("Wobble Strength", Range(0,5)) = 0.5
        _OffSet ("Wobble Strength", Range(0,5)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        Cull back 

        GrabPass 
        {
            "_GrabTexture"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog  

            #include "UnityCG.cginc"
            // #include "Milan.cginc" 
            #define REFRACTION_AMOUNT 300

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
                float2 uvCap : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _MatCap;
            sampler2D _Noise;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            
            float4 _MainTex_ST;
            float4 _Noise_ST;

            float _OffSet;

            float4 _Color; 
            float4 _SpecColor; 
            float _WobbleStrength;
            

            #define SampleGrabTexture(posX, posY) tex2Dproj( _GrabTexture, float4( \
                i.screenPos.x + _GrabTexture_TexelSize.x * posX + tex.x, \
                i.screenPos.y + _GrabTexture_TexelSize.y * posY - tex.y, \
                i.screenPos.z + tex.y,  \
                i.screenPos.w + tex.x  )) \


            v2f vert (appdata v)
            {
                v2f o;
                // o.light = GetGouraudLighting(v.normal,v.vertex, _Color, _SpecColor, _Shininess);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float wobble = o.uv * 0.05 * _Time * 0.02;
                fixed4 noise = tex2Dlod(_Noise, float4(wobble,1,wobble,1));

                float4 p = float4( v.vertex );

                float3 e = normalize( mul( UNITY_MATRIX_MV , p) );
                float3 n = normalize( mul( UNITY_MATRIX_MV , float4(v.normal, 0.1)) );

                v.vertex.xyz += (((v.normal * sin(_Time * noise + _OffSet)) ))  * _WobbleStrength * 0.3;


                float3 r = reflect(e, n);
                float m = 2. * sqrt( pow( r.x, 2. ) + pow( r.y, 2. ) + pow( r.z + 1., 2. ) );
                half2 capCoord; 
                capCoord = r.xy / m + 0.5;
                o.uvCap = capCoord;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }
            float2 FlowUV (float2 uv, float2 flowVector, float time) 
            {
                return uv - flowVector * time;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 base = tex2D(_MatCap, i.uvCap).rgb;
                float3 tex = tex2D(_MainTex , i.uv + _Time.y * 0.2).rgb;
                float2 flowVector = tex2D(_MainTex, i.uv).rg * 2 - 1;
                float2 uv = FlowUV(base.xy, flowVector, _Time.y);
                half4 result = SampleGrabTexture(0, 0);
                // result.xy = FlowUV(i.uvCap.xy,result.xy,_Time.y);

                for (int range = 1; range <= REFRACTION_AMOUNT; range++)
                {
                    result += SampleGrabTexture(0, range);
                    result += SampleGrabTexture(0, -range);
                }
                result /= REFRACTION_AMOUNT * 2 + 1;
                

                float3 final = (base + 0.2) + (float3(result.xyz) );

                float transparency = base.x;
                return float4(base,transparency + 0.4);

            } 
            ENDCG
        }
    }
}
