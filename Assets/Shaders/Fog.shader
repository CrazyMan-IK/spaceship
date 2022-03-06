Shader "Unlit/Fog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Fog {Mode Off}
        //Color[_Color]
        //LOD 100
        Cull Off
        ZWrite Off
        ZTest Always
        Lighting Off

        Pass
        {
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _CameraOpaqueTexture;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed depth = tex2D(_CameraDepthTexture, i.uv).r;
                
                //if (depth <= 0.1 && depth >= 0.01)
                if (depth > 0.0)
                {
                    return depth;
                    return lerp(fixed4(0, 0, 0, 0), col, depth);
                }

                return fixed4(col.r, col.g, col.b, col.a);
                //return fixed4(col.r, col.g, col.b, 1);
                //return fixed4(0, 0, 1, 1);
            }
            ENDCG
        }
    }
}
