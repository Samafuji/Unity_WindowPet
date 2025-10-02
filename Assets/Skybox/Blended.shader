Shader "Skybox/Blended"
{
    Properties
    {
        _Blend ("Blend", Range (0,1)) = 0.5
        _Tex1 ("Skybox 1", Cube) = "" {}
        _Tex2 ("Skybox 2", Cube) = "" {}
    }
    SubShader
    {
        Tags {"Queue"="Background" "RenderType"="Background"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : POSITION;
                float3 texcoord : TEXCOORD0;
            };

            samplerCUBE _Tex1;
            samplerCUBE _Tex2;
            float _Blend;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col1 = texCUBE(_Tex1, i.texcoord);
                half4 col2 = texCUBE(_Tex2, i.texcoord);
                return lerp(col1, col2, _Blend);
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
