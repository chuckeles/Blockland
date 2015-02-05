using OpenTK;
using System;

namespace Blockland {

  public class Random {

    public Random() {
      mRandom = new System.Random();
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

    public static float Value {
      get {
        return (float)mStatic.mRandom.NextDouble();
      }
    }

    private System.Random mRandom;

    public static Random mStatic = new Random();

  }

}