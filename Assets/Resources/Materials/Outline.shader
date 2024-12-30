Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Float) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // Первый проход: рендерим обводку
        Pass
        {
            Name "OUTLINE"
            Cull Front // Отключаем фронтальные стороны для рендера только обводки
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                fixed4 color : COLOR;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;

                // Увеличиваем размер объекта
                float3 scaledVertex = v.vertex.xyz * (1.0 + _OutlineWidth);
                o.pos = UnityObjectToClipPos(float4(scaledVertex, 1.0));
                o.color = _OutlineColor;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color; // Рендерим обводку выбранным цветом
            }
            ENDCG
        }

        // Второй проход: рендерим сам объект
        Pass
        {
            Name "BASE"
            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
