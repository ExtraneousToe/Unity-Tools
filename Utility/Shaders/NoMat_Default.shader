// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UnityTools Shaders/NoMat_Default"
{
    Properties
    {
        _Density ("Density", Range(2,50)) = 30
        _Color ("Colour 1", color) = (1,1,1,1)
        _Color2 ("Colour 2", color) = (0,0,0,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Density;
            fixed4 _Color;
            fixed4 _Color2;

            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv * _Density;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 c = i.uv;
                c = floor(c) / 2;
                float checker = frac(c.x + c.y) * 2;

                if(checker == 1)
                    return _Color;
                else
                    return _Color2;
            }
            ENDCG
        }
    }
}