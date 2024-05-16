Shader "Flocking/Instanced" {

    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" { }
        _BumpMap ("Bumpmap", 2D) = "bump" { }
        _MetallicGlossMap ("Metallic", 2D) = "white" { }
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Glossiness ("Smoothness", Range(0, 1)) = 1.0
    }

    SubShader {
        
        CGPROGRAM

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        #pragma surface surf Standard vertex:vert addshadow nolightmap
        #pragma instancing_options procedural:setup

        float4x4 _Matrix;
        float3 _BoidPosition;

        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
            struct Boid {
                float3 position;
                float3 direction;
                float noise_offset;
            };

            StructuredBuffer<Boid> boidsBuffer;
        #endif
        
        void vert(inout appdata_full v, out Input data) {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                v.vertex = mul(_Matrix, v.vertex);
                //v.vertex.xyz += _BoidPosition;
            #endif
        }

        float4x4 create_matrix(float3 position, float3 direction, float3 up) {
            //这里旋转的做法是在旋转坐标系，将现有的位置变换到根据旋转方向（forward方向 即z轴的朝向）所构建的新坐标系中。
            float3 zAxis = normalize(direction);
            float3 xAxis = normalize(cross(up, zAxis));
            float3 yAxis = cross(zAxis, xAxis);

            return float4x4(
                xAxis.x, yAxis.x, zAxis.x, position.x,
                xAxis.y, yAxis.y, zAxis.y, position.y,
                xAxis.z, yAxis.z, zAxis.z, position.z,
                0, 0, 0, 1
            );
        }

        void setup() {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                Boid boid = boidsBuffer[unity_InstanceID];
                _Matrix = create_matrix(boid.position, boid.direction, float3(0, 1, 0));
            #endif
        }
        
        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Metallic = m.r;
            o.Smoothness = _Glossiness * m.a;
        }
        
        ENDCG
    }
}