﻿using UnityEngine;

namespace Leap.Unity {

  public class DeltaQuaternionBuffer {

    protected struct ValueTimePair {
      public Quaternion value;
      public float time;
    }

    protected RingBuffer<ValueTimePair> _buffer;

    public DeltaQuaternionBuffer(int bufferSize) {
      _buffer = new RingBuffer<ValueTimePair>(bufferSize);
    }

    public int length { get { return _buffer.Length; } }
    public bool isFull { get { return _buffer.IsFull; } }
    
    public void Clear() { _buffer.Clear(); }

    private float _previousSampleTime = 0F;
    public void Add(Quaternion sample, float sampleTime) {
      sample = sample.ToNormalized();

      if (sampleTime == _previousSampleTime) {
        SetLatest(sample, sampleTime);
        return;
      }

      _buffer.Add(new ValueTimePair { value = sample, time = sampleTime });
    }

    public Quaternion Get(int idx) {
      return _buffer.Get(idx).value;
    }

    public Quaternion GetLatest() {
      return Get(length - 1);
    }

    public void Set(int idx, Quaternion sample, float sampleTime) {
      sample = sample.ToNormalized();

      _buffer.Set(idx, new ValueTimePair { value = sample, time = sampleTime });
    }

    public void SetLatest(Quaternion sample, float sampleTime) {
      if (length == 0) Set(0, sample, sampleTime);
      else Set(length - 1, sample, sampleTime);
    }

    public float GetTime(int idx) {
      return _buffer.Get(idx).time;
    }

    /// <summary>
    /// Returns the average change between each sample rotation per unit time as an
    /// angle-axis vector, or zero if the buffer is empty.
    /// </summary>
    public Vector3 Delta() {
      if (length == 0) return Vector3.zero;

      // TODO: FINISH
    }

    /// <summary> Returns the average change between each sample per unit time, or zero if the buffer is not full. </summary>
    //public override Vector3 Delta() {
    //  if (!IsFull) {
    //    return Vector3.zero;
    //  }
    //  Vector3 deltaPerTimeSum = Vector3.zero;
    //  int length = Length;
    //  for (int i = 0; i < length - 1; i++) {
    //    deltaPerTimeSum += (Get(i + 1) - Get(i)) / (GetTime(i + 1) - GetTime(i));
    //  }
    //  return deltaPerTimeSum / (length - 1);
    //}

  }

}