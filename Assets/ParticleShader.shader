Shader "Custom/Particle" {

	SubShader {
		Pass {
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		#include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		struct Particle{
			float3 position;
			float3 velocity;
			float life;
		};
		
		struct v2g{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};
		struct g2f{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;
		

		v2g vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			v2g o = (v2g)0;

			// Color
			float life = particleBuffer[instance_id].life;
			float lerpVal = life * 0.25f;
			o.color = fixed4(1.0f - lerpVal+0.1, lerpVal+0.1, 1.0f, lerpVal);

			// Position
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));

			return o;
		}

		[maxvertexcount(4)] // on génère un triangle strip de 4 points
		void geom(point v2g IN[1], inout TriangleStream<g2f> triStream) {
			g2f p;
			float s = 0.025;
			p.life = IN[0].life;
			p.color = IN[0].color;
			p.position = IN[0].position + float4(-s, -s *2, 0, 0);//top left
			triStream.Append(p);
			p.position = IN[0].position + float4(s, -s *2, 0, 0);//top right
			triStream.Append(p);
			p.position = IN[0].position + float4(-s, s *2, 0, 0);//bot left
			triStream.Append(p);
			p.position = IN[0].position + float4(s, s *2, 0, 0);//bot left
			triStream.Append(p);
			triStream.RestartStrip();
		// v2g est une structure (à définir) émise en sortie du Vertex Shader
		// IN[1] est un tableau de v2g, qui ne contient qu’une seule entrée (car le vertex shader traite un point ici)
		// triStream est un tableau de la structure g2f (à définir) stockant les Vertex à générer
			
		}
		
		float4 frag(g2f i) : COLOR
		{
			float4 c = i.color;
			c.z = i.life;
			return c;
		}


		ENDCG
		}
	}
	FallBack Off
}