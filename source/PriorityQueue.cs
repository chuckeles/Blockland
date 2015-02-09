using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// A queue that is always sorted by priority. (http://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx?Page=1)
  /// </summary>
  public class PriorityQueue<T> {

    #region Methods

    /// <summary>
    /// Get the item with the highest priority.
    /// </summary>
    /// <returns>Top priority item</returns>
    public T Dequeue() {
      int li = mList.Count - 1;
      KeyValuePair<int, T> frontItem = mList[0];
      mList[0] = mList[li];
      mList.RemoveAt(li);

      --li;
      int pi = 0;
      while (true) {
        int ci = pi * 2 + 1;
        if (ci > li) break;
        int rc = ci + 1;
        if (rc <= li && mList[rc].Key < mList[ci].Key)
          ci = rc;
        if (mList[pi].Key <= mList[ci].Key) break;
        KeyValuePair<int, T> tmp = mList[pi]; mList[pi] = mList[ci]; mList[ci] = tmp;
        pi = ci;
      }
      return frontItem.Value;
    }

    /// <summary>
    /// Add new item to the queue.
    /// </summary>
    /// <param name="priority">Item's priority (lower number means higher priority)</param>
    /// <param name="item">Item to add</param>
    public void Enqueue(int priority, T item) {
      mList.Add(new KeyValuePair<int, T>(priority, item));

      int ci = mList.Count - 1;
      while (ci > 0) {
        int pi = (ci - 1) / 2;

        if (mList[ci].Key >= mList[pi].Key)
          break;

        KeyValuePair<int, T> tmp = mList[ci]; mList[ci] = mList[pi]; mList[pi] = tmp;
        ci = pi;
      }
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the number of items in the queue.
    /// </summary>
    public int Count {
      get {
        return mList.Count;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Underlying list.
    /// </summary>
    private List<KeyValuePair<int, T>> mList = new List<KeyValuePair<int, T>>();

    #endregion Fields

  }

}