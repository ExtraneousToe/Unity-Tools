using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class GeometricGroup : ChunkGroup
    {
        public override void GeneratePreset()
        {
            try
            {
                Generator.GenerateGroup(this);
            }
            catch (NoChunkException nce)
            {
                Debug.Log(nce.Message);
            }
        }

        protected override void Start()
        {
            base.Start();

            GeneratePreset();
        }
    }
}