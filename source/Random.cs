using OpenTK;
using SimplexNoise;
using System;

namespace Blockland {

  public class Random {

    public Random() {
      mRandom = new System.Random();

      mRandom.NextBytes(Noise.perm);
    }

    public static float Range(float from, float to) {
      return Value * (to - from) + from;
    }

    public static int Range(int from, int to) {
      return (int)(Value * (to - from)) + from;
    }

    public static Vector2 Vector(float from, float to) {
      return new Vector2(Range(from, to), Range(from, to));
    }

    public static float Simplex(float x, float y, int octaves, float min = -1, float max = 1, float persistance = 0.5f) {
      float maxAmp = 0;
      float noise = 0;

      for (int i = 0; i < octaves; ++i) {
        float freq = (float)Math.Pow(2, i);
        float amp = (float)Math.Pow(persistance, i);

        noise += Noise.Generate(x * freq, y * freq, i) * amp;
        maxAmp += amp;
      }

      noise /= maxAmp;
      noise = noise * (max - min) / 2 + (max + min) / 2;

      return noise;
    }

    public static float Value {
      get {
        return (float)mStatic.mRandom.NextDouble();
      }
    }

    private System.Random mRandom;

    public static Random mStatic = new Random();

  }

}