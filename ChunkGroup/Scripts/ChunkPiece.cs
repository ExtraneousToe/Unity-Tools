using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityTools
{
    public class ChunkPiece
    {
        public delegate ChunkPiece Const();

        private static Dictionary<Type , Const> _typeAndCtorList;

        public static Dictionary<Type , Const> TypeList
        {
            get
            {
                if (_typeAndCtorList == null)
                {
                    _typeAndCtorList = new Dictionary<Type , Const>();

                    // ensure populated
                    if (!_typeAndCtorList.ContainsKey(typeof(BasicPiece)))
                        _typeAndCtorList.Add(typeof(BasicPiece), () => new BasicPiece());
                }

                return _typeAndCtorList;
            }
        }

        private Vector2 _uvCoord;
        public Vector2 UVCoord
        {
            get
            {
                return _uvCoord;
            }
        }

        private Color _colour;
        public Color PieceColour
        {
            get
            {
                return _colour;
            }
        }

        public ChunkPiece(Vector2 uv, Color c)
        {
            _uvCoord = uv;
            _colour = c;
        }

        public Vector2 GetUV(Vector2 offset)
        {
            return offset;//UVManagerSO.Instance.GetUV(UVCoord, offset);
        }
    }
}