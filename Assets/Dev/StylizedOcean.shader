Shader "Custom/StylizedOceanSimple"
{
    Properties
    {
        _ColorShallow ("Shallow Color", Color) = (0.2, 0.6, 1, 1)
        _ColorDeep ("Deep Color", Color) = (0.0, 0.1, 0.4, 1)
        _WaveStrength ("Wave Strength", Float) = 0.2
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveScale ("Wave Scale", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        fixed4 _ColorShallow;
        fixed4 _ColorDeep;
        float _WaveStrength;
        float _WaveSpeed;
        float _WaveScale;

        struct Input
        {
            float3 worldPos;
        };

        void vert(inout appdata_full v)
        {
            float wave = sin((v.vertex.x + _Time.y * _WaveSpeed) * _WaveScale)
                       * cos((v.vertex.z + _Time.y * _WaveSpeed) * _WaveScale);
            v.vertex.y += wave * _WaveStrength;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            float depth = saturate((IN.worldPos.y + 5.0) / 10.0);
            fixed4 col = lerp(_ColorDeep, _ColorShallow, depth);
            o.Albedo = col.rgb;
            o.Alpha = 1.0;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
