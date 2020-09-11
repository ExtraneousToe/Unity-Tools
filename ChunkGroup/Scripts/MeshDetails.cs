using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class MeshDetails
    {
        private List<Vector3> _points;

        public List<Vector3> Points
        {
            get
            {
                if (_points == null)
                    _points = new List<Vector3>();

                return _points;
            }
        }

        public void AddPoint(Vector3 p)
        {
            Points.Add(p);
        }

        private List<Vector3> _normals;

        public List<Vector3> Normals
        {
            get
            {
                if (_normals == null)
                    _normals = new List<Vector3>();

                return _normals;
            }
        }

        public void AddNormal(Vector3 n)
        {
            Normals.Add(n);
        }

        private List<int> _tris;

        public List<int> Tris
        {
            get
            {
                if (_tris == null)
                    _tris = new List<int>();

                return _tris;
            }
        }

        public void AddTri(int t)
        {
            Tris.Add(t);
        }

        private List<Vector2> _uvs;

        public List<Vector2> UVs
        {
            get
            {
                if (_uvs == null)
                    _uvs = new List<Vector2>();

                return _uvs;
            }
        }

        public void AddUV(Vector2 uv)
        {
            UVs.Add(uv);
        }

        private List<Color> _cols;

        public List<Color> Cols
        {
            get
            {
                if (_cols == null)
                    _cols = new List<Color>();

                return _cols;
            }
        }

        public void AddColour(Color c)
        {
            Cols.Add(c);
        }

        public void LoadIntoMesh(ref Mesh m)
        {
            m.Clear();

            m.SetVertices(Points);
            m.SetTriangles(Tris.ToArray(), 0);

            m.SetUVs(0, UVs);
            m.SetColors(Cols);

            if (Normals.Count != 0)
                m.SetNormals(Normals);
            else
                m.RecalculateNormals();
        }
    }
}
