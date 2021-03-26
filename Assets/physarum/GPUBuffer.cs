using System.Runtime.InteropServices;
using UnityEngine;

public class GPUBuffer<T> : System.IDisposable
{
    public ComputeBuffer Buffer { get; private set; }
    public T[] CPUData { get; private set; }

    public GPUBuffer(int capacity)
    {
        CPUData = new T[capacity];
        Buffer = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(T)));
        Upload();
    }

    public void Upload() { Buffer.SetData(CPUData); }
    public T[] Download() { Buffer.GetData(CPUData); return CPUData; }

    #region IDisposable implementation
    public void Dispose()
    {
        if ( Buffer != null)
        {
            Buffer.Release();
            Buffer.Dispose();
        }
    }
    #endregion
}