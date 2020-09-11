using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class OutOfChunkException : ChunkGroupException
    {
        public OutOfChunkException() : base()
        {
        }

        public OutOfChunkException(string message) : base(message)
        {
        }
    }
}