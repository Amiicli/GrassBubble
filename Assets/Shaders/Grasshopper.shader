Shader "Custom/Grasshopper"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _VertPosition ("Vertex Position", 2D) = "white" {}
        _minBounds ("Minimum Bounds", Range(-10,0)) = 1
        _maxBounds ("Maximum Bounds", Range(0,10)) = 1
        _Frame ("Frame", Range(0,1)) = 1
        _DivisionSection ("Division Section", Integer) = 1
    }
    SubShader
    {
        // Tags { "RenderType"="Opaque" }

        Stencil 
        {
            Ref 1 // ReferenceValue = 1
            Comp NotEqual
        }
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert
        #pragma instancing_options assumeuniformscaling procedural:ConfigureProcedural 
        #pragma editor_sync_compilation
        #pragma target 4.5

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
            float3 normal : NORMAL;
        };

        struct Input
        {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;
        sampler2D _VertPosition;
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
            float frame;
        };

        float Rand(float st)
        {
            return frac(sin(dot(st, float2(12.9898,78.233))) * 43758.5453123);
        }

        float RandomRange(float min, float max, float seed)
        {
            return lerp(min, max, Rand(seed));
        }
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _minBounds;
        float _maxBounds;

        int _DivisionSection;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            StructuredBuffer<Grasshopper> grasshoppers;
        #endif


        float2x2 GetRotMatrix(float angle,float4 vertex)
        {
            float c = cos(angle);
            float s = sin(angle);
            float4 worldPos = mul(unity_ObjectToWorld, vertex);
            return float2x2(c, -s, s, c);
        }
        float _Scale;
        float _Rotation;
        float3 _cachedPos;
        int _ID;
        float _Frame;
        void ConfigureProcedural ()
        {
            #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
                Grasshopper grasshopper = grasshoppers[unity_InstanceID];
                _ID = unity_InstanceID;
                unity_ObjectToWorld = 0.0;
                unity_ObjectToWorld._m03_m13_m23_m33 = float4(grasshopper.position, 1.0);
                _cachedPos = grasshopper.position;
                float3x3 rotationMatrix = (float3x3)unity_ObjectToWorld;
                _Scale = 1;
                _DivisionSection = grasshopper.state;
                _Frame = grasshopper.frame;
                // _Scale = cos(_Time.y * unity_InstanceID);
                unity_ObjectToWorld._m00_m11_m22 = _Scale;
                _Rotation = grasshopper.radians;
                _Color.rgb = grasshopper.color;

            #endif
        }

        void vert (inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float speed = 1;
            float vertexID = v.texcoord1.x;
            float textureTime = _Frame / 32;
            float4 animUV = float4(v.texcoord1.x,v.texcoord1.y,0,0);
            // float4 uv = float4(vertexID, _Frame, 0, 0);
            _Frame = _Frame * 0.125;
            float section = _DivisionSection * 0.125;
            animUV.x = vertexID;
            animUV.y = (frac(animUV.y * 0.125)  + section + _Frame);
            float4 currentAnimationTexturePos = tex2Dlod(_VertPosition, animUV);
            
            float3 finalPosition = lerp(_minBounds, _maxBounds, currentAnimationTexturePos.xyz);
            float3 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
            // // Grasshopper grasshopper = grasshoppers[unity_InstanceID];
            // worldPos.xyz = finalPosition;
            v.vertex.xyz = finalPosition;
            // v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
            // v.vertex.xyz += sin(_Time.y * 20) * 0.1;
            // o.customColor = abs(v.normal);'
            // float2x2 rotationMatrix = GetRotMatrix(grasshopper.radians,v.vertex);
            float2x2 rotationMatrix = GetRotMatrix(_Rotation - 90,v.vertex);
            // o.position = finalPosition;
            
            v.vertex.xz = mul(rotationMatrix, v.vertex.xz); //Might need to give a Vector3 to em, maybe try if statement?
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // o.Metallic = _Metallic;
            // o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
