using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityTools
{
    public class ChunkGroupException : Exception
    {
        public ChunkGroupException() : base()
        {
        }

        public ChunkGroupException(string message) : base(message)
        {
        }
    }
}