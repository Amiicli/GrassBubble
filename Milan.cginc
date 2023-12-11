#if !defined(MILAN_INCLUDED)
#define MILAN_INCLUDED

#include "UnityCG.cginc"

uniform float4 _LightColor0; 

float4 GetGouraudLighting(float3 normal, float4 vertex, float4 color, float4 specColor, float shininess)
{
    float4x4 modelMatrix = unity_ObjectToWorld;
    float3x3 modelMatrixInverse = unity_WorldToObject;
    float3 normalDirection = normalize(
    mul(normal, modelMatrixInverse));
    float3 viewDirection = normalize(_WorldSpaceCameraPos 
    - mul(modelMatrix, vertex).xyz);
    float3 lightDirection;
    float attenuation;

    if (0.0 == _WorldSpaceLightPos0.w) // directional light?
    {
        attenuation = 1.0; // no attenuation
        lightDirection = normalize(_WorldSpaceLightPos0.xyz);
    } 
    else // point or spot light
    {
        float3 vertexToLightSource = _WorldSpaceLightPos0.xyz
            - mul(modelMatrix, vertex).xyz;
        float distance = length(vertexToLightSource);
        attenuation = 1.0 / distance; // linear attenuation 
        lightDirection = normalize(vertexToLightSource);
    }

    float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * color.rgb;

    float3 diffuseReflection = 
    attenuation * _LightColor0.rgb * color.rgb
    * max(0.0, dot(normalDirection, lightDirection));

    float3 specularReflection;
    if (dot(normalDirection, lightDirection) < 0.0) 
    // light source on the wrong side?
    {
    specularReflection = float3(0.0, 0.0, 0.0); 
        // no specular reflection
    }
    else // light source on the right side
    { 
    specularReflection = attenuation * _LightColor0.rgb 
        * specColor.rgb * pow(max(0.0, dot(
        reflect(-lightDirection, normalDirection), 
        viewDirection)), shininess);
    }

    return float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
}

float4 GetAddLightPassShading(float3 normal, float4 vertex, float4 color, float4 specColor, float shininess)
{ 
     
    float4x4 modelMatrix = unity_ObjectToWorld;
    float3x3 modelMatrixInverse = unity_WorldToObject;
    float3 normalDirection = normalize(
        mul(normal, modelMatrixInverse));
    float3 viewDirection = normalize(_WorldSpaceCameraPos 
        - mul(modelMatrix, vertex).xyz);
    float3 lightDirection;
    float attenuation;

    if (0.0 == _WorldSpaceLightPos0.w) // directional light?
    {
        attenuation = 1.0; // no attenuation
        lightDirection = normalize(_WorldSpaceLightPos0.xyz);
    } 
    else // point or spot light
    {
        float3 vertexToLightSource = _WorldSpaceLightPos0.xyz
            - mul(modelMatrix, vertex).xyz;
        float distance = length(vertexToLightSource);
        attenuation = 1.0 / distance; // linear attenuation 
        lightDirection = normalize(vertexToLightSource);
    }

    float3 diffuseReflection = 
        attenuation * _LightColor0.rgb * color.rgb
        * max(0.0, dot(normalDirection, lightDirection));

    float3 specularReflection;
    if (dot(normalDirection, lightDirection) < 0.0) 
        // light source on the wrong side?
    {
        specularReflection = float3(0.0, 0.0, 0.0); 
            // no specular reflection
    }
    else // light source on the right side
    {
        specularReflection = attenuation * _LightColor0.rgb 
            * specColor.rgb * pow(max(0.0, dot(
            reflect(-lightDirection, normalDirection), 
            viewDirection)), shininess); 
    }
    return float4(diffuseReflection + specularReflection, 1.0);


    

}



#endif