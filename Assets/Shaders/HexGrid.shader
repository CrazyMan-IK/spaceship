Shader "Unlit/HexGrid"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Color("Color", Color) = (1, 1, 1, 1)
        [HDR] _GlowColor("Glow Color", Color) = (1, 1, 1, 1)
        _Thickness("Thickness", float) = 0.01
        _Offset("Offset", Range(0, 1)) = 0
        _HexSizeMultiplier("Hex Size Multiplier", float) = 1
        [Toggle(USE_RANDOM_COLOR)] _UseRandomColor("Use Random Color", int) = 1
    }
    
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        ZClip False

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local USE_RANDOM_COLOR

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

            //sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                //o.vertex = float4(UnityObjectToViewPos(v.vertex), 1);
                o.vertex = v.vertex;
                o.vertex.xy *= 2;
                o.vertex.y = -o.vertex.y;
                /*o.vertex.z = 1;*/
                o.uv = v.uv;
                return o;
            }

            #define PI 3.141592653589793

            // Helper vector. If you're doing anything that involves regular triangles or hexagons, the
            // 30-60-90 triangle will be involved in some way, which has sides of 1, sqrt(3) and 2.
            const static float2 s = float2(1, 1.7320508);

            float4 _Color;
            float4 _GlowColor;
            float _Thickness;
            float _Offset;
            float _HexSizeMultiplier;

            //float2 hash2(float2 p)
            //{
            //    float2 o = float2(1, 1); //texture2D(noise, (p + 0.5) / 256.0, -100.0).xy;
            //    return o;
            //}

#if USE_RANDOM_COLOR
            float2 hash2(float2 p)
            {
                //p = (p + 0.5) / 256.0, -100.0;

                return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
            }
#endif

            /*float rand(float2 c) {
                return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p, float freq) {
                float unit = _ScreenParams.x / freq;
                float2 ij = floor(p / unit);
                float2 xy = (p % unit) / unit;
                //xy = 3.*xy*xy-2.*xy*xy*xy;
                xy = .5 * (1. - cos(PI * xy));
                float a = rand((ij + float2(0., 0.)));
                float b = rand((ij + float2(1., 0.)));
                float c = rand((ij + float2(0., 1.)));
                float d = rand((ij + float2(1., 1.)));
                float x1 = lerp(a, b, xy.x);
                float x2 = lerp(c, d, xy.x);
                return lerp(x1, x2, xy.y);
            }

            float pNoise(float2 p, int res) {
                float persistance = .5;
                float n = 0.;
                float normK = 0.;
                float f = 4.;
                float amp = 1.;
                int iCount = 0;
                for (int i = 0; i < 50; i++) {
                    n += amp * noise(p, f);
                    f *= 2.;
                    normK += amp;
                    amp *= persistance;
                    if (iCount == res) break;
                    iCount++;
                }
                float nf = n / normK;
                return nf * nf * nf * nf;
            }*/

            // hex(uv)
            // getHex(uv)
            // -------------
            // These very very helpful functions are by Gary Warne. I hope to understand it one day, but for now it's a black box :)
            //
            // -------------
            float hex(in float2 p)
            {
                p = abs(p);

                return max(dot(p, s * .5), p.x); // Hexagon.
            }

            float4 getHex(float2 p)
            {
                float4 hC = floor(float4(p, p - float2(0.5, 1)) / s.xyxy) + 0.5;
                //return float4(p, p - float2(0.5, 1)) / s.xyxy;

                float4 h = float4(p - hC.xy * s, p - (hC.zw + 0.5) * s);

                return (dot(h.xy, h.xy) < dot(h.zw, h.zw) ? float4(h.xy, hC.xy) : float4(h.zw, hC.zw + float2(0.5, 0.5))) * float4(1, 1, s);
            }

            const static float divisor = 8.0;

#if USE_RANDOM_COLOR
            float4 getColour(float col_id)
            {
                //return _Color; //float4(0.0, 0.325, 0.75, 1.0);
                //return float4(col_id, col_id, col_id, 1.0);

                col_id = col_id % divisor;
                float3 col = _Color.rgb; //float3(.16, .15, .18);
                if (col_id <= 0.) {
                    //col = float3(0.78, 0.78, 0.29);
                    col = float3(1, 1, 0.37);
                }
                else if (col_id <= 1.) {
                    //col = float3(0.29, 0.62, 0.64);
                    col = float3(0.37, 0.79, 0.82);
                }
                else if (col_id <= 2.) {
                    //col = float3(0.18, 0.46, 0.4);
                    col = float3(0.23, 0.59, 0.51);
                }
                else if (col_id >= 3.) {
                    //col = float3(0.18, 0.22, 0.24);
                    col = float3(0.23, 0.28, 0.31);
                }

                return float4(col, 1.0);
            }
#endif

            // float3 easeOutQuad(in float current, in float3 begin_value, in float3 change_value, in float duration) {
            //  return -change_value *(current/=duration)*(current-2.) + begin_value;
            // }

            float3 easeOutQuad(float t, float3 b, float3 c, float d)
            {
                t /= d;
                return -c * (t / d) * (t - 2.) + b;
            }
            float easeOutQuad(float t, float b, float c, float d)
            {
                t /= d;
                return -c * (t / d) * (t - 2.) + b;
            }
            float easeInQuad(float t, float b, float c, float d)
            {
                t /= d * 0.5;
                return lerp(b, c, t * t);
            }
            float easeInCubic(float t, float b, float c, float d)
            {
                t /= d * 0.5;
                return lerp(b, c, t * t * t);
            }
            float easeInExpo(float t, float b, float c, float d)
            {
                t /= d * 0.5;
                return lerp(b, c, t == 0 ? 0 : pow(2, 10 * t - 10));
            }
            float3 easeOutSine(float t, float3 b, float3 c, float d)
            {
                return c * sin(t / d * (PI / 2.)) + b;
            }

            float4 hexColour(float2 id)
            {
#if USE_RANDOM_COLOR
                float ani_length = 2.;
                float pause_length = 1.;

                float2 h = hash2(id);
                float time_offset = h.y;

                float t = (_Time.z / ani_length + time_offset);
                // t = u_time / ani_length;

                float time_id_offset = floor(t);
                float time_id_offset_next = floor(t + 1.);
                float col_id = floor(h.x * divisor + time_id_offset);
                //return (t % 1.0);
                float col_id_next = floor(h.x * divisor + time_id_offset_next);

                float4 colour = getColour(col_id);
                float4 next_colour = getColour(col_id_next);

                // t = mod(t, 1.);
                // return easeOutSine(t, colour, next_colour, 1.);

                float intermix = t % 1.;
                return lerp(colour, next_colour, intermix);
#else
                return _Color;
#endif
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv; //(gl_FragCoord.xy - 0.5 * resolution.xy) / min(resolution.y, resolution.x);

                float2 ar = float2(_ScreenParams.x / _ScreenParams.y, 1.0);

                //float xn = (pNoise((uv.xy + 1000) * 5000, 1));
                //float yn = (pNoise((uv.yx + 1000) * 5000, 1));

                //uv *= 5.;
                //uv *= 0.25;
                //uv -= 0.75;
                uv -= 0.5;
                uv *= _HexSizeMultiplier;
                uv.x *= ar.x;

                float4 hex_uv = getHex(uv);// -float2(xn, yn));

                //gl_FragColor = float4(smoothstep(.49, .5, 0.99 - hex(hex_uv.xy)));
                //return;

                float iso = hex(hex_uv.xy) * 0.9; //max(hex_uv.zw * hex_uv.zw * normalize(abs(uv)), 1.0)) * 0.9;//(1.0 / length(hex_uv) - 0.5);

                //gl_FragColor = float4(hex_uv.xy / normalize((uv.xy * ar)), 0, 0); //float4(iso);
                //return;

                float2 id = hex_uv.zw / (_HexSizeMultiplier / 6);

                float4 colour = hexColour(id);

                //float offset = 1 - (_Time.y * 1.5 % 1);
                //float offset = 0.75 + sin(_Time.x * 1.5) * 0.75;
                float offset = _Offset;
                //offset *= 1.0 / length(id) * 4.0;
                //offset = easeInQuad(min(offset, 1.0), 0.0, 1.0 / length(id), 0.7);
                //3 = 1.7
                //6 = 1.9
                //8 = 1.6
                //10 = 1.45
                //12 = 1.3
                //24 = 0.6
                //48 = 0.4
                //96 = 0.3

                //return float4(length(id).xxx / 10, 1.0);

                //float target = _Temp; //1.414;
                //if (abs(length(id) - target) <= 0.05)
                //{
                    //return float4(1.0 - min(abs(length(id) - target), 1.0).xxx, 1.0);
                //    return float4(min(abs(length(id) - target), 1.0).xxx, 1.0);
                //}
                //return float4(0.0, 0.0, 0.0, 1.0);

                offset = max(easeInQuad(max(offset, 0), 0.0, 1.0 / max(length(id), 0.5) * 3, 1.82) - 0.1, 0.0);
                //offset = easeInQuad(max(offset, 0), 0.0, 1.0 / length(id), _Temp);
                //return saturate(offset);
                //offset = 0.15;

                //gl_FragColor = float4(1.0 / length(id) * 0.25);
                //return;

                float outer = smoothstep(.49, .5, (offset - _Thickness) + iso);
                float inner = smoothstep(.49, .5, (1.0 - offset - _Thickness) - iso);

                //return (1.0 - max(outer, inner));
                //float glow = max(easeInQuad(max(_Offset, 0), 0.0, 2 / length(id), 0.6) - 0.5, 0) * (1.0 - max(outer, inner));
                float glow = max(easeInQuad(max(_Offset, 0), 0.0, 1 / length(uv / _HexSizeMultiplier), 0.6) - 3.0, 0) * (1.0 - max(outer, inner));
                //glow = saturate(glow);
                //float glow = max(easeInQuad(max(_Offset, 0), 0.0, 1 / length(hex_uv.xy), 0.6), 0) * (1.0 - max(outer, inner));
                //return glow;
                //return 0.1 / length(hex_uv.xy);

                //return (1.0 * _Offset - length(uv / _HexSizeMultiplier) * 2);
                //return length(uv / 6);
                //return (1.0 - max(outer, inner)) * offset;
                //return offset * 2.0 * inner + (1.0 - max(outer, inner)) * offset;

                //colour.rgb = saturate(colour.rgb - (1.0 - max(outer, inner)) * 10);
                colour.rgb = lerp(0.0, colour.rgb, max(outer, inner));
                colour.rgb = lerp(colour.rgb, _GlowColor, glow);//(1.0 - max(outer, inner)) * offset);
                colour.a = offset * 2.0 * inner + 1.0 - max(outer, inner);

                colour = lerp(colour, 0.0, outer);

                //return float4(uv.xyx, 1);
                //return float4(hex_uv.xyz, 1);
                return colour;
            }
            ENDCG
        }
    }
}