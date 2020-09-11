using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    [CreateAssetMenu(menuName = "Drawerer/Voxels", fileName = "Voxel Drawerer")]
    public class VoxelDrawerSO : ChunkDrawerSO
    {
        protected override void MeshForCoord(ref MeshDetails details, int x, int y, int z, Chunk chunk)
        {
            ChunkPiece curr = null;
            ChunkPiece checking;

            try
            {
                curr = chunk.GetPieceRollover(x, y, z);
            }
            catch (NoChunkException nce)
            {
            }

            // if the current block is empty, skip it
            if (curr == null)
                return;

            // check each direction
            for (int i = 0; i < s_DirOffsets.GetLength(0); i++)
            {
                try
                {
                    checking = chunk.GetPieceRollover(
                        x + s_DirOffsets[i, 0],
                        y + s_DirOffsets[i, 1],
                        z + s_DirOffsets[i, 2]
                    );
                }
                catch (NoChunkException nce)
                {
                    if (!DrawEdgeChunkWalls)
                        // if there is no chunk there, don't draw the edge
                        continue;

                    checking = null;
                }

                // if the checking block is empty
                if (checking == null)
                    // generate the desired face
                    Face(ref details, x, y, z, i, chunk.ChunkScale, curr);
            }
        }

        private void Face(ref MeshDetails details, int x, int y, int z, int arrIndex, float scale, ChunkPiece cp)
        {
            int i;

            // vertices
            for (i = 0; i < s_FaceCorners.GetLength(1); i++)
            {
                // the face array is used to access the given cube corner index
                Vector3 point = new Vector3(
                                    x + s_CubeCorners[s_FaceCorners[arrIndex, i], 0],
                                    y + s_CubeCorners[s_FaceCorners[arrIndex, i], 1],
                                    z + s_CubeCorners[s_FaceCorners[arrIndex, i], 2]
                                ) * scale;

                details.AddPoint(point);
                details.AddTri(details.Tris.Count);

                details.AddUV(cp.GetUV(s_UVLookup[s_UVCorners[arrIndex, i]]));
            }

            // UVs

            // Normals?
        }

        public readonly Vector2[] s_UVLookup = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        public readonly int[,] s_CubeCorners = new int[8, 3]
        {
            { 0, 0, 0 }, // bottom front left
            { 1, 0, 0 }, // bottom front right
            { 1, 0, 1 }, // bottom back right
            { 0, 0, 1 }, // bottom back left
            { 0, 1, 0 }, // top front left
            { 1, 1, 0 }, // top front right
            { 1, 1, 1 }, // top back right
            { 0, 1, 1 } // top back left 
        };

        public readonly int[,] s_DirOffsets = new int[,]
        {
            { 0, -1, 0 },
            { 0, 0, -1 },
            { -1, 0, 0 },
            { 1, 0, 0 },
            { 0, 0, 1 },
            { 0, 1, 0 }
        };

        public readonly static int[,] s_UVCorners = new int[,] // 6, 4
        {
            { 0, 1, 2, 0, 2, 3 }, // D
            { 0, 1, 2, 0, 2, 3 }, // S
            { 0, 1, 2, 0, 2, 3 }, // W
            { 0, 1, 2, 0, 2, 3 }, // E
            { 0, 1, 2, 0, 2, 3 }, // N
            { 0, 1, 2, 0, 2, 3 } // U
        };

        public readonly static int[,] s_FaceCorners = new int[,] // 6, 4
        {
            { 3, 0, 1, 3, 1, 2 }, // D
            { 0, 4, 5, 0, 5, 1 }, // S
            { 3, 7, 4, 3, 4, 0 }, // W
            { 1, 5, 6, 1, 6, 2 }, // E
            { 2, 6, 7, 2, 7, 3 }, // N
            { 4, 7, 6, 4, 6, 5 }, // U
        };

        //        public readonly int[,] s_Tris = new int[,] // 6, 6
        //        {
        //            { 0, 1, 2, 0, 2, 3 }, // D
        //            { 0, 1, 2, 0, 2, 3 }, // S
        //            { 0, 1, 2, 0, 2, 3 }, // W
        //            { 0, 1, 2, 0, 2, 3 }, // E
        //            { 0, 1, 2, 0, 2, 3 }, // N
        //            { 0, 1, 2, 0, 2, 3 }, // U
        //        };
    }
}
