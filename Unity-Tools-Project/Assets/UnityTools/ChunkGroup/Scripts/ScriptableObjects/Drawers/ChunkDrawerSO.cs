using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public abstract class ChunkDrawerSO : BaseScriptable
    {
        [SerializeField]
        [Range(0f,1f)]
        private float m_RoutineDelay = 0.01f;

        [SerializeField]
        private Material m_Material;

        [SerializeField]
        private bool m_DrawEdgeChunkWalls = false;
        public bool DrawEdgeChunkWalls
        {
            get 
            { 
                return m_DrawEdgeChunkWalls; 
            }
            set
            {
                m_DrawEdgeChunkWalls = value;
            }
        }

        public Coroutine DrawChunk(Chunk chunk)
        {
            if (chunk.Parent.Generator is GeometricGeneratorSO)
                DrawEdgeChunkWalls = true;

            return chunk.StartCoroutine(GenerateMeshRoutine(chunk));
        }

        protected virtual IEnumerator GenerateMeshRoutine(Chunk chunk)
        {
            MeshDetails details = new MeshDetails();

            Mesh m = chunk.ChunkFilter.mesh;
            chunk.ChunkRenderer.material = m_Material;

            int[] vals = new int[3];

            for (int i = 0; i < s_ChunkNeighbourOffset.GetLength(0); i++)
            {
                if (!chunk.Parent)
                    continue;

                try
                {
                    Chunk c = chunk.Parent [chunk.GroupCoord + s_ChunkNeighbourOffset[i]];
                }
                catch (NoChunkException nce)
                {
                    vals[i] = -1;
                }
            }

            for (int x = vals[0]; x < chunk.ChunkSize; x++)
            {
                for (int y = vals[1]; y < chunk.ChunkSize; y++)
                {
                    for (int z = vals[2]; z < chunk.ChunkSize; z++)
                    {
                        MeshForCoord(ref details, x, y, z, chunk);

                        details.LoadIntoMesh(ref m);

                        if(m_RoutineDelay != 0)
                            yield return new WaitForSeconds(m_RoutineDelay);
                    }   
                } 
            }

            chunk.ChunkFilter.sharedMesh = chunk.ChunkCollider.sharedMesh = m;
        }

        protected abstract void MeshForCoord(ref MeshDetails details, int x, int y, int z, Chunk chunk);

        // s_CubeVertexOffset lists the positions, relative to vertex0, of each of the 8 vertices of a cube
        public static readonly Vector3Int[] s_ChunkNeighbourOffset = new Vector3Int[]
        {
            new Vector3Int(-1, 0, 0), 
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, -1), 
        };
    }
}
