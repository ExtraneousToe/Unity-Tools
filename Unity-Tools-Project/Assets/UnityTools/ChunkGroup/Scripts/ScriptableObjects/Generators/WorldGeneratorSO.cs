using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public abstract class WorldGeneratorSO : GroupGeneratorSO
    {
        public override void GenerateGroup(ChunkGroup cGroup)
        {
            base.GenerateGroup(cGroup);
        }

        public override void GenerateChunk(ChunkGroup cGroup, Chunk c)
        {
            throw new System.NotImplementedException();
        }
    }
}
