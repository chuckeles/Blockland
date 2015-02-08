#version 440

in vec3 inPosition;
in vec3 inNormal;
in vec3 inTexCoord;

uniform mat4 Model;
uniform mat4 View;
uniform mat4 Projection;

out vec3 Normal;
out vec3 TexCoord;
out float Depth;

void main() {
  gl_Position = Projection * View * Model * vec4(inPosition, 1.0);
  
  Normal = inNormal;
  TexCoord = inTexCoord;

  Depth = length((View * Model * vec4(inPosition, 1.0)).xyz);
}