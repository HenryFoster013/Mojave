Shader "Custom/TileUpdate"
{
    Properties
    {
        _MainTex ("Current State Texture", 2D) = "white" {}
        _ColourMap ("Unique Color ID Map", 2D) = "white" {}
        _TargetID ("ID Color to Change", Vector) = (0,0,0,1)
        _NewColour ("New Color to Paint", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _ColourMap;
            float4 _TargetID;
            float4 _NewColour;

            fixed4 frag (v2f_img i) : SV_Target
            {
                float3 idSample = tex2D(_ColourMap, i.uv).rgb;
                float4 currentState = tex2D(_MainTex, i.uv);

                float3 diff = abs(idSample - _TargetID.rgb);
                
                if (max(max(diff.r, diff.g), diff.b) < 0.001)
                {
                    return _NewColour;
                }

                return currentState;
            }
            ENDCG
        }
    }
}