Shader "Custom/Level" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
    _ShadowColor ("Main Color", Color) = (1,1,1,1)
    _DiffuseVal ("Diffuse Value",Range(0,10)) = 5
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf CSLambert

sampler2D _MainTex;
fixed4 _ShadowColor;
fixed4 _Color;
float _DiffuseVal;

struct Input {
	float2 uv_MainTex;
};
half4 LightingCSLambert (SurfaceOutput s, half3 lightDir, half atten) 
{
	fixed diff = max (0, dot (s.Normal, lightDir));

	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
    
    c.rgb += _ShadowColor.xyz * max(0.0,(1.0-(diff*atten*2))) * _DiffuseVal;
	c.a = s.Alpha;
	return c;
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}