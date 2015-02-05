using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public class World {

    public void Create(uint size, uint height) {
      mHeight = height;
    }

    public void Update() {

    }

    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();
    private ArrayList mReadyChunks = new ArrayList();
    private uint mHeight;

  }

}