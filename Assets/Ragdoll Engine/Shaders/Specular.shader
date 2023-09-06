Shader "Ragdoll Engine/Specular"
{
    Properties
    {
        _MainTex ("Color Texture", 2D) = "white" {}

        _SpecularTexture ("Specular Texture", 2D) = "white" {}

        _NormalTexture ("Normal Texture", 2D) = "white" {}

        _NormalStrength ("Normal Strength", Range(0, 1)) = 0

        _FalloffTexture ("Falloff Texture", 2D) = "white" {}

        _FresnelPower ("Fresnel Power", Float) = 5

        _FresnelStrength ("Fresnel Strength", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        LOD 200

        CGPROGRAM

        #pragma surface surf StandardSpecular fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;

        sampler2D _SpecularTexture;

        sampler2D _NormalTexture;

        sampler2D _FalloffTexture;

        struct Input
        {
            float3 worldPos;

            float3 worldNormal;

            float2 uv_MainTex; INTERNAL_DATA
        };

        half _NormalStrength;

        half _FresnelPower;

        half _FresnelStrength;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Base Color
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);

            // Specular
            fixed4 specular = tex2D(_SpecularTexture, IN.uv_MainTex);

            specular.rgb = specular;

            // Smoothness
            float smoothness = specular.a;

            // Normal
            float3 normal = UnpackNormal(tex2D(_NormalTexture, IN.uv_MainTex));

            normal.xy *= _NormalStrength;

            normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));

            // Fresnel
            float fresnel = tex2D(_FalloffTexture, IN.uv_MainTex)
                * pow(1 - abs(dot(WorldNormalVector(IN, o.Normal), normalize(_WorldSpaceCameraPos - IN.worldPos))), _FresnelPower)
                * _FresnelStrength;

            albedo += fixed4(fresnel, fresnel, fresnel, 0);
            
            o.Albedo = albedo;

            o.Specular = specular * albedo;

            o.Smoothness = smoothness;

            o.Normal = normal;

            o.Alpha = albedo.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
