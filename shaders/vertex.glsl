#version 440

in vec3 inPosition;
in vec3 inNormal;
in vec3 inTexCoord;

uniform mat4 uModelView;
uniform mat4 uModelViewProjection;
uniform mat4 uNormal;

out vec3 Normal;
out vec3 TexCoord;
out float Depth;

void main() {
  gl_Position = uModelViewProjection * vec4(inPosition, 1.0);
  
  Normal = inNormal;
  TexCoord = inTexCoord;

  Depth = length((uModelView * vec4(inPosition, 1.0)).xyz);
}