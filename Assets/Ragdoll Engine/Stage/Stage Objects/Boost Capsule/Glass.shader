Shader "Ragdoll Engine/Glass"
{
    Properties
    {
        _MainTex ("Color Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MatCap ("MatCap", 2D) = "white" {}
        _FresnelPower("Fresnel Power", Float) = 5
        _FresnelStrength("Fresnel Strength", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma target 3.0

        sampler2D _MainTex;
        
        sampler2D _MatCap;

        struct Input
        {
            float3 worldPos;

            float3 worldNormal;

            float2 uv_MainTex; INTERNAL_DATA
        };

        half4 _Color;

        half _FresnelPower;

        half _FresnelStrength;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base Color
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // MatCap
            albedo *= tex2D(_MatCap, mul(UNITY_MATRIX_V, IN.worldNormal).xy * 0.5 + 0.5);

            // Fresnel
            float fresnel = pow(1 - abs(dot(WorldNormalVector(IN, o.Normal), normalize(_WorldSpaceCameraPos - IN.worldPos))), _FresnelPower) * _FresnelStrength;

            albedo += fixed4(fresnel, fresnel, fresnel, fresnel);
            
            o.Albedo = albedo;

            o.Alpha = clamp(albedo.a, 0, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
