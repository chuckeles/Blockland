#version 440

in vec3 inPosition;
in vec3 inNormal;
in vec3 inTexCoord;

uniform mat4 ModelView;
uniform mat4 ModelViewProjection;

out vec3 Normal;
out vec3 TexCoord;
out float Depth;

void main() {
  gl_Position = ModelViewProjection * vec4(inPosition, 1.0);
  
  Normal = inNormal;
  TexCoord = inTexCoord;

  Depth = length((ModelView * vec4(inPosition, 1.0)).xyz);
}