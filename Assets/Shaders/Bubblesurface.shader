Shader "Milan/Bubble"
{
    Properties
    {
        _MatCap ("Matcap", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        // LOD 200
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front 

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv : TEXCOORD0;
        };
        

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        sampler2D _MatCap;

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float4 p = float4( v.vertex );

            float3 e = normalize( mul( UNITY_MATRIX_MV , p) );
            float3 n = normalize( mul( UNITY_MATRIX_MV , float4(v.normal, 0.)) );

            float3 r = reflect( e, n );
                float m = 2. * sqrt( 
                pow( r.x, 2. ) + 
                pow( r.y, 2. ) + 
                pow( r.z + 1., 2. ) 
            );
            half2 capCoord;
            capCoord = r.xy / m + 0.5;
            o.uv = capCoord;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MatCap, IN.uv);
            float transparency = c.x;
            o.Albedo = c.rgb;
            o.Alpha = transparency;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
