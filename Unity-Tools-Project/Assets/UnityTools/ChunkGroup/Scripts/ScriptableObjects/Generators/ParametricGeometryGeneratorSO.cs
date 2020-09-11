using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public abstract class ParametricGeometryGeneratorSO : GeometricGeneratorSO
    {
        [SerializeField]
        private bool m_ShellOnly = true;
        public bool ShellOnly
        {
            get
            {
                return m_ShellOnly;
            }
        }

        [Range(1, 360)]
        [SerializeField]
        private int m_Slices = 360;
        [Range(1, 360)]
        [SerializeField]
        private int m_Stacks = 360;

        public override void GenerateGroup(ChunkGroup cGroup)
        {
            for (int i = 0; i < m_Slices; i++)
            {
                float theta = i / (float)m_Slices * 360f * Mathf.Deg2Rad;

                for (int j = 0; j < m_Stacks; j++)
                {
                    float phi = j / (float)m_Stacks * 360f * Mathf.Deg2Rad;

                    GenerateChunkUV(cGroup, theta, phi);
                }
            }
        }

        public override void GenerateChunk(ChunkGroup cGroup, Chunk c)
        {
            for (int i = 0; i < m_Slices; i++)
            {
                float theta = i / (float)m_Slices * 360f * Mathf.Deg2Rad;

                for (int j = 0; j < m_Stacks; j++)
                {
                    float phi = j / (float)m_Stacks * 360f * Mathf.Deg2Rad;

                    GenerateChunkUV(cGroup, theta, phi);
                }
            }
        }

        protected abstract void GenerateChunkUV(ChunkGroup cGroup, float u, float v);
    }
}
