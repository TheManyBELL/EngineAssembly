Shader "Custom/3dDiscardSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        //_ShowPercent("ShowPercent", range(0.00, 1.00)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Cull off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        //float _Factor;
        float3 _Position;
        float3 _Direction;
        float _Max;
        float _ShowPercent;
        float4 _LightColor;
        float _LightWidth;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_base v, out Input i)
        {
            UNITY_INITIALIZE_OUTPUT(Input, i);
            i.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            //v.uv = v.texcoord;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float factor = dot(IN.worldPos - _Position, _Direction) / _Max - (_ShowPercent * 2 - 1);
            if (factor > 0)
            {
                discard;
            }
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
