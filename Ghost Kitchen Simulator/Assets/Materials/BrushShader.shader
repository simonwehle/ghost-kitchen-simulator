Shader "Unlit/BrushShader"
{
    Properties { _MainTex ("Texture", 2D) = "white" {} }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            // Hier lag der Fehler: 'Cull Off' statt 'Visual.Off'
            ZTest Always Cull Off ZWrite Off Blend One One 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float d = distance(i.uv, float2(0.5, 0.5));
                // Zeichnet einen weichen Kreis (Alpha-Verlauf)
                float mask = saturate(1.0 - (d * 2.0));
                return fixed4(mask, mask, mask, 1);
            }
            ENDCG
        }
    }
}