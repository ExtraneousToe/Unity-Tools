using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public abstract class GroupGeneratorSO : BaseScriptable
    {
        public virtual void GenerateGroup(ChunkGroup cGroup)
        {
            foreach (Chunk c in cGroup.ChunkDict.Values)
            {
                GenerateChunk(cGroup, c);
            }
        }

        public abstract void GenerateChunk(ChunkGroup cGroup, Chunk c);
    }
}