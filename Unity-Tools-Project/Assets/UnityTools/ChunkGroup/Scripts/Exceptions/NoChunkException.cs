using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class NoChunkException : ChunkGroupException
    {
        private Vector3Int m_GroupCoord;
        public Vector3Int GroupCoord
        {
            get 
            { 
                return m_GroupCoord;
            }
        }

        public NoChunkException() : base()
        {
        }

        public NoChunkException(string message, Vector3Int groupCoord) : base(message)
        {
            this.m_GroupCoord = groupCoord;
        }
    }
}