using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class BasicPiece : ChunkPiece
    {
        public BasicPiece() : base(new Vector2Int(0, 0), new Color(0.5f, 0.5f, 0.5f, 1))
        {
        }
    }
}