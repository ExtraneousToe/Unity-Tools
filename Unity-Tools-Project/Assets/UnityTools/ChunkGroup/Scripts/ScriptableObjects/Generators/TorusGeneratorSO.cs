using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace UnityTools
{
    [CreateAssetMenu(menuName="Group/Torus Gen", fileName="Torus Generator")]
    public class TorusGeneratorSO : ParametricGeometryGeneratorSO
    {
        [SerializeField]
        private float m_TorusRadius = 5f;
        [SerializeField]
        private float m_InnerRadius = 1f;

//        public override void GenerateGroup(ChunkGroup cGroup)
//        {
//            base.GenerateGroup(cGroup);
//        }

        protected override void GenerateChunkUV(ChunkGroup cGroup, float u, float v)
        {
            float stepSize = cGroup.ChunkScale;// / m_Radius;

            for (float step = m_InnerRadius; step >= 0; step -= stepSize)
            {
                float x = (m_TorusRadius + step * Mathf.Cos(v)) * Mathf.Cos(u);
                float y = (m_TorusRadius + step * Mathf.Cos(v)) * Mathf.Sin(u);
                float z = step * Mathf.Sin(v);

                Vector3Int[] pair = 
                    cGroup.ConvertWorldCoordToGroupAndChunk(
                        Mathf.RoundToInt(x),
                        Mathf.RoundToInt(y),
                        Mathf.RoundToInt(z)
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
