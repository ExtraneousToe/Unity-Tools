using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTools
{
    [CreateAssetMenu(menuName = "Group/Sphere Gen", fileName = "Sphere Generator")]
    public class SphereGeneratorSO : ParametricGeometryGeneratorSO
    {
        [SerializeField]
        private float m_Radius = 5f;

//        public override void GenerateGroup(ChunkGroup cGroup)
//        {
//            base.GenerateGroup(cGroup);
//        }

//        public override void GenerateChunk(ChunkGroup cGroup, Chunk c)
//        {
//            int gridSize = cGroup.ChunkSize;
//
//            for (int x = 0; x < gridSize; x++)
//            {
//                for (int y = 0; y < gridSize; y++)
//                {
//                    for (int z = 0; z < gridSize; z++)
//                    {
//                        Vector3Int worldCoord = c.ConvertChunkToWorldCoord(x, y, z);
//
//                        // will fill the sphere
//                        if (worldCoord.magnitude <= m_Radius)
//                            c.SetPieceRollover(x, y, z, ChunkPiece.TypeList.Values.Shuffle().FirstOrDefault()());
//                    }
//                }
//            }
//        }

        protected override void GenerateChunkUV(ChunkGroup cGroup, float u, float v)
        {
            /*
             * Currently this will only create the shell of the shape
             */

            float x = Mathf.Cos(u) * Mathf.Sin(v);
            float y = Mathf.Sin(u) * Mathf.Sin(v);
            float z = Mathf.Cos(v);

            float stepSize = cGroup.ChunkScale;// / m_Radius;

            for(float step = m_Radius; step >= 0; step -= stepSize)
            {
                float xStep = x * step;
                float yStep = y * step;
                float zStep = z * step;

                Vector3Int[] pair = 
                    cGroup.ConvertWorldCoordToGroupAndChunk(
                        Mathf.RoundToInt(xStep),
                        Mathf.RoundToInt(yStep),
                        Mathf.RoundToInt(zStep)
                    );

                Chunk ch = null;

                try
                {
                    ch = cGroup [pair [0]];
                }
                catch (NoChunkException nce)
                {
                    ch = cGroup.TrySpawnChunk(pair [0]);
                }

                ch.SetPieceRollover(pair [1], ChunkPiece.TypeList.Values.Shuffle().FirstOrDefault()());

                if (ShellOnly)
                    break;
            }
        }
    }
}
