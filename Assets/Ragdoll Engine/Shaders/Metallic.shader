Shader "Ragdoll Engine/Metallic"
{
    Properties
    {
        _MainTex ("Color Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MetallicTexture ("Metallic Texture", 2D) = "white" {}
        [KeywordEnum(R, G, B, A)] _MetallicChannel ("Metallic Channel", Float) = 4
        [Toggle] _InvertMetallic ("Invert Metallic", Float) = 0
        _Metallic ("Metallic", Range(0, 1)) = 0
        _SmoothnessTexture ("Smoothness Texture", 2D) = "white" {}
        [KeywordEnum(R, G, B, A)] _SmoothnessChannel("Smoothness Channel", Float) = 4
        [Toggle] _InvertSmoothness ("Invert Smoothness", Float) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0
        _NormalTexture ("Normal Texture", 2D) = "white" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 0
        _EmissionTexture ("Emission Texture", 2D) = "black"
        _FalloffTexture ("Falloff Texture", 2D) = "white" {}
        _FresnelPower ("Fresnel Power", Float) = 5
        _FresnelStrength ("Fresnel Strength", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;

        sampler2D _MetallicTexture;

        sampler2D _SmoothnessTexture;

        sampler2D _NormalTexture;

        sampler2D _EmissionTexture;

        sampler2D _FalloffTexture;

        struct Input
        {
            float3 worldPos;

            float3 worldNormal;

            float2 uv_MainTex; INTERNAL_DATA
        };

        half4 _Color;

        half _MetallicChannel;

        half _InvertMetallic;

        half _Metallic;

        half _SmoothnessChannel;

        half _InvertSmoothness;

        half _Smoothness;

        half _NormalStrength;

        half _FresnelPower;

        half _FresnelStrength;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base Color
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Metallic
            float metallic = _Metallic;

            switch (_MetallicChannel)
            {
                case 1:
                    metallic *= tex2D(_MetallicTexture, IN.uv_MainTex).r;

                    break;
                case 2:
                    metallic *= tex2D(_MetallicTexture, IN.uv_MainTex).g;

                    break;
                case 3:
                    metallic *= tex2D(_MetallicTexture, IN.uv_MainTex).b;

                    break;
                default:
                    metallic *= tex2D(_MetallicTexture, IN.uv_MainTex).a;

                    break;
            }

            if (_InvertMetallic > 0)
                metallic = 1 - metallic;

            // Smoothness
            float smoothness = _Smoothness;

            switch (_SmoothnessChannel)
            {
                case 1:
                    smoothness *= tex2D(_SmoothnessTexture, IN.uv_MainTex).r;

                        break;
                case 2:
                    smoothness *= tex2D(_SmoothnessTexture, IN.uv_MainTex).g;

                        break;
                case 3:
                    smoothness *= tex2D(_SmoothnessTexture, IN.uv_MainTex).b;

                        break;
                default:
                    smoothness *= tex2D(_SmoothnessTexture, IN.uv_MainTex).a;

                        break;
            }

            if (_InvertSmoothness > 0)
                smoothness = 1 - smoothness;

            // Normal
            float3 normal = UnpackNormal(tex2D(_NormalTexture, IN.uv_MainTex));

            normal.xy *= _NormalStrength;

            normal.z = sqrt(1 - saturate(dot(normal.xy, normal.xy)));

            // Emission
            fixed4 emission = tex2D(_EmissionTexture, IN.uv_MainTex);

            // Fresnel
            float fresnel = tex2D(_FalloffTexture, IN.uv_MainTex)
                * pow(1 - abs(dot(WorldNormalVector(IN, o.Normal), normalize(_WorldSpaceCameraPos - IN.worldPos))), _FresnelPower)
                * _FresnelStrength;

            albedo += fixed4(fresnel, fresnel, fresnel, 0);
            
            o.Albedo = albedo;

            o.Metallic = metallic;

            o.Smoothness = smoothness;

            o.Normal = normal;

            o.Emission = emission;

            o.Alpha = albedo.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
