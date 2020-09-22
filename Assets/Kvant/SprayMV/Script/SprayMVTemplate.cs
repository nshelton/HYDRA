//
// Kvant/SprayMV - Particle system with motion vectors support
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Kvant
{
    public class SprayMVTemplate : ScriptableObject
    {
        #region Public properties

        /// Source shapes (editable)
        public Mesh[] shapes {
            get { return _shapes; }
        }

        [SerializeField] Mesh[] _shapes = new Mesh[1];

        /// Maximum number of instances (editable)
        public int maxInstanceCount {
            get { return Mathf.Clamp(_maxInstanceCount, 1, 8192); }
        }

        [SerializeField] int _maxInstanceCount = 8192;

        /// Instance count (read only)
        public int instanceCount {
            get { return _instanceCount; }
        }

        [SerializeField] int _instanceCount;

        /// Tmplate mesh (read only)
        public Mesh mesh {
            get { return _mesh; }
        }

        [SerializeField] Mesh _mesh;

        #endregion

        #region Private members

        [SerializeField] Mesh _defaultShape;

        Mesh GetShape(int index)
        {
            if (_shapes == null || _shapes.Length == 0) return _defaultShape;
            var mesh = _shapes[index % _shapes.Length];
            return mesh == null ? _defaultShape : mesh;
        }

        #endregion

        #region Public methods

        #if UNITY_EDITOR

        // Template mesh rebuilding method
        public void RebuildMesh()
        {
            // Working buffers
            var vtx_out = new List<Vector3>();
            var nrm_out = new List<Vector3>();
            var tan_out = new List<Vector4>();
            var uv0_out = new List<Vector2>();
            var uv1_out = new List<Vector2>();
            var idx_out = new List<int>();

            // Vertex/instance count
            var vcount = 0;
            _instanceCount = 0;

            while (_instanceCount < maxInstanceCount)
            {
                // Get the Nth Source mesh.
                var mesh = GetShape(_instanceCount);
                var vtx_in = mesh.vertices;

                // Keep the vertex count under 64k.
                if (vcount + vtx_in.Length > 65535) break;

                // Copy the vertices.
                vtx_out.AddRange(vtx_in);
                nrm_out.AddRange(mesh.normals);
                tan_out.AddRange(mesh.tangents);
                uv0_out.AddRange(mesh.uv);

                // Set UV1 temporarily.
                var uv1 = new Vector2(_instanceCount + 0.5f, 0);
                uv1_out.AddRange(Enumerable.Repeat(uv1, vtx_in.Length));

                // Copy the indices.
                foreach (var i in mesh.triangles) idx_out.Add(i + vcount);

                // Increment the vertex/instance count.
                vcount += vtx_in.Length;
                _instanceCount++;
            }

            // Rescale the UV1.
            for (var i = 0; i < vcount; i++)
                uv1_out[i] /= _instanceCount;

            // Reset the mesh asset.
            _mesh.Clear();
            _mesh.SetVertices(vtx_out);
            _mesh.SetNormals(nrm_out);
            _mesh.SetUVs(0, uv0_out);
            _mesh.SetUVs(1, uv1_out);
            _mesh.SetIndices(idx_out.ToArray(), MeshTopology.Triangles, 0);
            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            _mesh.Optimize();
            _mesh.UploadMeshData(true);
        }

        #endif

        #endregion

        #region ScriptableObject functions

        void OnEnable()
        {
            if (_mesh == null) {
                _mesh = new Mesh();
                _mesh.name = "SprayMV Template";
            }
        }

        #endregion
    }
}
