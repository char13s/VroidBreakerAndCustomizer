Shader "Custom/CustomVertexPositionMask"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MaskThreshold("Mask Threshold", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert

            struct Input
            {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float _MaskThreshold;

            void vert(inout appdata_full v)
            {
                // Calculate the mask based on vertex position
                float mask = clamp(v.vertex.y, 0.0, 1.0); // You can replace this with any masking logic you want

                // Apply the mask to the UV coordinates
                v.texcoord.xy *= mask > _MaskThreshold ? 1.0 : 0.0;
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                // Sample the main texture
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

                // Output the final color
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
