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
out float Depth;
out float Light;

void main() {
  // eye position
  vec3 eyePosition = (uModelView * vec4(inPosition, 1.0)).xyz;
  
  // sun direction
  vec3 sun = normalize(vec3(0.3, 1.0, 0.6));

  // calculate output
  gl_Position = uModelViewProjection * vec4(inPosition, 1.0);
  TexCoord = inTexCoord;
  Depth = length(eyePosition);
  
  // calculate light
  vec3 eyeSun = normalize(uNormal * sun);
  vec3 eyeNormal = normalize(uNormal * inNormal);
  
  vec3 v = normalize(-eyePosition);
  vec3 r = reflect(-eyeSun, eyeNormal);
  float sunDotNormal = max(dot(eyeSun, eyeNormal), 0.0);
  
  float ambient = 0.2;
  float diffuse = 0.8 * sunDotNormal;
  float specular = pow(max(dot(v, r), 0.0), 100.0) * 0.6;
  
  Light = ambient + diffuse + specular;
}