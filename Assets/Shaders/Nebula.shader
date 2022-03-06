Shader "Unlit/Nebula"
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
        Cull Front
        //Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        ZClip False
        //Lighting Off

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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _CameraOpaqueTexture;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.screenPos = ComputeScreenPos(o.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 col1 = tex2D(_MainTex, i.uv) * _Color;
                fixed4 col2 = tex2D(_CameraOpaqueTexture, uv);
                col1.rgb *= col1.a;
                fixed3 res = 1 - (1 - col1.rgb) * (1 - col2.rgb);
                //return fixed4(1, 1, 1, 0) - res;// fixed4(1, 1, 1, 1) - ñol;
                return fixed4(res.r, res.g, res.b, col2.a);// fixed4(1, 1, 1, 1) - ñol;
                //return fixed4(1, 1, 1, 1);// fixed4(1, 1, 1, 1) - ñol;
                //return fixed4(i.screenPos.x, i.screenPos.y, 0, 1);// fixed4(1, 1, 1, 1) - ñol;
            }
            ENDCG
        }
    }
}
