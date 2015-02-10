#version 440

// input
in vec3 inPosition;
in vec3 inNormal;
in vec3 inTexCoord;

// matrices
uniform mat4 uModelView;
uniform mat4 uModelViewProjection;
uniform mat3 uNormal;

// output
out vec3 TexCoord;
out vec3 EyeSun;
out vec3 EyeSun2;
out vec3 EyePosition;
out vec3 EyeNormal;

void main() {
  // sun directions
  vec3 sun = normalize(vec3(0.3, 1.0, 0.6));
  vec3 sun2 = normalize(vec3(-sun.x, sun.y, -sun.z));

  // calculate output
  gl_Position = uModelViewProjection * vec4(inPosition, 1.0);
  TexCoord = inTexCoord;
  EyeSun = normalize(uNormal * sun);
  EyeSun2 = normalize(uNormal * sun2);
  EyePosition = (uModelView * vec4(inPosition, 1.0)).xyz;
  EyeNormal = normalize(uNormal * inNormal);
}