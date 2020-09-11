using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityTools
{
    /// <summary>
    /// Chunk group.
    /// 
    /// Base class controls the shared requisite components of all ChunkGroups.
    /// </summary>
    public abstract class ChunkGroup : BaseBehaviour
    {
        /// <summary>
        /// The chunk dictionary.
        /// This contains all chunks, accessed via a given key. The format for 
        /// the key is given by a constant string variable.
        /// </summary>
        private Dictionary<string, Chunk> _chunkDictionary;

        public Dictionary<string, Chunk> ChunkDict
        {
            get
            {
                if (_chunkDictionary == null)
                    _chunkDictionary = new Dictionary<string, Chunk>();

                return _chunkDictionary;
            }
        }

        /// <summary>
        /// The Key Format. Format is be x,y,z.
        /// </summary>
        public const string KEY_FORMAT = "{0},{1},{2}";

        /// <summary>
        /// The size of the chunk children.
        /// </summary>
        [SerializeField]
        private int _chunkSize = 4;

        public int ChunkSize
        {
            get { return _chunkSize; }
            set
            {
                _chunkSize = value;
            }
        }

        /// <summary>
        /// The scale.
        /// Controls the rendering scale of the chunks.
        /// </summary>
        [SerializeField]
        private float _chunkScale = 1f;

        public float ChunkScale
        {
            get { return _chunkScale; }
            set { _chunkScale = value; }
        }

        /// <summary>
        /// The drawer.
        /// </summary>
        [SerializeField]
        private ChunkDrawerSO _drawer;

        public ChunkDrawerSO Drawer
        {
            get { return _drawer; }
            set { _drawer = value; }
        }

        /// <summary>
        /// The generator.
        /// </summary>
        [SerializeField]
        private GroupGeneratorSO _generator;

        public GroupGeneratorSO Generator
        {
            get { return _generator; }
            set { _generator = value; }
        }

        /// <summary>
        /// The chunk prefab.
        /// </summary>
        [SerializeField]
        private Chunk _chunkPrefab;


        [Header("World Vars")]
        [MinMaxIntRange(-10, 10)]
        public RangedInt xRange = new RangedInt(-1, 1);
        [MinMaxIntRange(-10, 10)]
        public RangedInt yRange = new RangedInt(-1, 1);
        [MinMaxIntRange(-10, 10)]
        public RangedInt zRange = new RangedInt(-1, 1);

        /// <summary>
        /// Gets the <see cref="UnityTools.ChunkGroup`1"/> with the specified key.
        /// 
        /// Primary access for Chunks via the ChunkGroup.
        /// </summary>
        /// <param name="key">Key.</param>
        public Chunk this[string key]
        {
            get
            {
                if (!ChunkDict.ContainsKey(key))
                {
                    string[] xyz = key.Split(',');
                    //                    Debug.Log();
                    throw new NoChunkException(
                        string.Format("No Chunk present at key [{0}]", key),
                        new Vector3Int(
                            int.Parse(xyz[0]),
                            int.Parse(xyz[1]),
                            int.Parse(xyz[2])
                        )
                    );
                }

                return ChunkDict[key];
            }
        }

        /// <summary>
        /// Gets the <see cref="UnityTools.ChunkGroup`1"/> with the specified groupCoord.
        /// 
        /// Primary access for Chunks via the ChunkGroup.
        /// </summary>
        /// <param name="groupCoord">Group coordinate.</param>
        public Chunk this[Vector3Int groupCoord]
        {
            get
            {
                return this[GenerateKey(
                    groupCoord.x,
                    groupCoord.y,
                    groupCoord.z)
                ];
            }
        }

        /// <summary>
        /// Gets the <see cref="UnityTools.ChunkGroup`1"/> with the specified x y z.
        /// 
        /// Primary access for Chunks via the ChunkGroup.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Chunk this[int x, int y, int z]
        {
            get
            {
                return this[GenerateKey(x, y, z)];
            }
        }

        /// <summary>
        /// Generates the key, given a Vector3Int
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="groupCoord">Group coordinate.</param>
        public string GenerateKey(Vector3Int groupCoord)
        {
            return string.Format(
                KEY_FORMAT,
                groupCoord.x,
                groupCoord.y,
                groupCoord.z
            );
        }

        /// <summary>
        /// Generates the key, given three ints
        /// </summary>
        /// <returns>The key.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public string GenerateKey(int x, int y, int z)
        {
            return string.Format(KEY_FORMAT, x, y, z);
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <returns><c>true</c>, if to file was saved, <c>false</c> otherwise.</returns>
        public bool SaveToFile()
        {
            throw new NotImplementedException("NYI - SaveToFile()");
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <returns><c>true</c>, if from file was loaded, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        public bool LoadFromFile(string fileName)
        {
            throw new NotImplementedException("NYI - LoadFromFile(string)");
        }

        /// <summary>
        /// Converts a given world point to a valid group coordinate.
        /// </summary>
        /// <returns>The world to group coordinate.</returns>
        /// <param name="worldPoint">World point.</param>
        public Vector3Int ConvertWorldToGroupCoord(Vector3 worldPoint)
        {
            return ConvertLocalisedToGroupCoord(
                transform.InverseTransformPoint(worldPoint)
            );
        }

        /// <summary>
        /// Converts the a localised point to a group coordinate.
        /// </summary>
        /// <returns>The localised to group coordinate.</returns>
        /// <param name="localPoint">Local point.</param>
        public Vector3Int ConvertLocalisedToGroupCoord(Vector3 localPoint)
        {
            // first divide by the scale to bring it down to whole-ish values.
            localPoint /= ChunkScale;

            return new Vector3Int(
                Mathf.FloorToInt(localPoint.x / (float)ChunkSize),
                Mathf.FloorToInt(localPoint.y / (float)ChunkSize),
                Mathf.FloorToInt(localPoint.z / (float)ChunkSize)
            );
        }

        /// <summary>
        /// Converts the a world point to a chunk coordinate.
        /// </summary>
        /// <returns>The world to chunk coordinate.</returns>
        /// <param name="worldPoint">World point.</param>
        public Vector3Int ConvertWorldToChunkCoord(Vector3 worldPoint)
        {
            return ConvertLocalisedToChunkCoord(
                transform.InverseTransformPoint(worldPoint)
            );
        }

        /// <summary>
        /// Converts a localised point to a chunk coordinate.
        /// </summary>
        /// <returns>The localised to chunk coordinate.</returns>
        /// <param name="localPoint">Local point.</param>
        public Vector3Int ConvertLocalisedToChunkCoord(Vector3 localPoint)
        {
            // first divide by the scale to bring it down to whole-ish values.
            localPoint /= ChunkScale;

            return new Vector3Int(
                (int)Mathf.Repeat(localPoint.x % ChunkSize, ChunkSize),
                (int)Mathf.Repeat(localPoint.y % ChunkSize, ChunkSize),
                (int)Mathf.Repeat(localPoint.z % ChunkSize, ChunkSize)
            );
        }

        /// <summary>
        /// Converts a world point to group and chunk coordinates.
        /// </summary>
        /// <returns>The world to group and chunk coordinate.</returns>
        /// <param name="worldPoint">World point.</param>
        public Vector3Int[] ConvertWorldToGroupAndChunkCoord(Vector3 worldPoint)
        {
            return new Vector3Int[]
            {
                ConvertWorldToGroupCoord(worldPoint),
                ConvertWorldToChunkCoord(worldPoint)
            };
        }

        /// <summary>
        /// Converts a localised point to group and chunk coordinates.
        /// </summary>
        /// <returns>The localised to group and chunk coordinate.</returns>
        /// <param name="localPoint">Local point.</param>
        public Vector3Int[] ConvertLocalisedToGroupAndChunkCoord(Vector3 localPoint)
        {
            return new Vector3Int[]
            {
                ConvertLocalisedToGroupCoord(localPoint),
                ConvertLocalisedToChunkCoord(localPoint)
            };
        }

        /// <summary>
        /// Converts the world to chunk and local.
        /// </summary>
        /// <returns>The world to chunk and local.</returns>
        /// <param name="xyz">Xyz.</param>
        public Vector3Int[] ConvertWorldCoordToGroupAndChunk(Vector3Int xyz)
        {
            return ConvertWorldCoordToGroupAndChunk(xyz.x, xyz.y, xyz.z);
        }

        /// <summary>
        /// Converts the world to chunk and local.
        /// </summary>
        /// <returns>The world to chunk and local.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Vector3Int[] ConvertWorldCoordToGroupAndChunk(int x, int y, int z)
        {
            return new Vector3Int[]
            {
                ConvertCoordToGroup(x, y, z),
                ConvertCoordToChunk(x, y, z)
            };
        }

        public Vector3Int ConvertCoordToGroup(Vector3Int xyz)
        {
            return ConvertCoordToGroup(xyz.x, xyz.y, xyz.z);
        }

        public Vector3Int ConvertCoordToGroup(int x, int y, int z)
        {
            return new Vector3Int(
                Mathf.FloorToInt(x / (float)ChunkSize),
                Mathf.FloorToInt(y / (float)ChunkSize),
                Mathf.FloorToInt(z / (float)ChunkSize)
            );
        }

        public Vector3Int ConvertCoordToChunk(Vector3Int xyz)
        {
            return ConvertCoordToGroup(xyz.x, xyz.y, xyz.z);
        }

        public Vector3Int ConvertCoordToChunk(int x, int y, int z)
        {
            return new Vector3Int(
                (int)Mathf.Repeat(x % ChunkSize, ChunkSize),
                (int)Mathf.Repeat(y % ChunkSize, ChunkSize),
                (int)Mathf.Repeat(z % ChunkSize, ChunkSize)
            );
        }

        /// <summary>
        /// Generates a 3D local position based on a given world coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromWorldCoord(Vector3Int worldCoord)
        {
            return LocalisedPositionFromWorldCoord(worldCoord.x, worldCoord.y, worldCoord.z);
        }

        /// <summary>
        /// Generates a 3D local position based on a given world coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromWorldCoord(int x, int y, int z)
        {
            return new Vector3(
                x, y, z
            ) * ChunkScale;
        }

        /// <summary>
        /// Generates a 3D local position based on a given group coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromGroupCoord(Vector3Int groupCoord)
        {
            return LocalisedPositionFromGroupCoord(groupCoord.x, groupCoord.y, groupCoord.z);
        }

        /// <summary>
        /// Generates a 3D local position based on a given group coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromGroupCoord(int x, int y, int z)
        {
            return new Vector3(
                x, y, z
            ) * ChunkScale * ChunkSize;
        }

        /// <summary>
        /// Generates a 3D local position based on a given group and local coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromGroupAndLocalCoord(Vector3Int groupCoord, Vector3Int localCoord)
        {
            return LocalisedPositionFromGroupAndLocalCoord(
                groupCoord.x, groupCoord.y, groupCoord.z,
                localCoord.x, localCoord.y, localCoord.z
            );
        }

        /// <summary>
        /// Generates a 3D local position based on a given group and local coordinate.
        /// </summary>
        /// <returns>The position from world coordinate.</returns>
        /// <param name="worldCoord">World coordinate.</param>
        public Vector3 LocalisedPositionFromGroupAndLocalCoord(
            int xG, int yG, int zG,
            int xL, int yL, int zL
        )
        {
            return new Vector3(
                (xG * ChunkSize + xL),
                (yG * ChunkSize + yL),
                (zG * ChunkSize + zL)
            ) * ChunkScale;
        }

        /// <summary>
        /// Generates a preset fill based on an attached generator.
        /// </summary>
        public abstract void GeneratePreset();

        /// <summary>
        /// Tries to spawn a child chunk.
        /// </summary>
        /// <returns>The spawn chunk.</returns>
        /// <param name="groupCoord">Group coordinate.</param>
        public Chunk TrySpawnChunk(Vector3Int groupCoord)
        {
            return TrySpawnChunk(groupCoord.x, groupCoord.y, groupCoord.z);
        }

        /// <summary>
        /// Tries to spawn a child chunk.
        /// </summary>
        /// <returns>The spawn chunk.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Chunk TrySpawnChunk(int x, int y, int z)
        {
            // create the key
            string key = GenerateKey(x, y, z);

            // exit if the key already exists
            if (ChunkDict.ContainsKey(key))
                return null;

            // spawn the chunk, and parent it
            Chunk newChunk = Instantiate(_chunkPrefab, transform);

            // generate and set the position
            Vector3 localPos = LocalisedPositionFromGroupCoord(x, y, z);
            newChunk.transform.localPosition = localPos;

            // set basic values
            // name
            newChunk.name = "Chunk_" + key;
            // parent
            newChunk.Parent = this;
            // group coord
            newChunk.GroupCoord = new Vector3Int(x, y, z);
            newChunk.SetChunkVars(ChunkSize);

            ChunkDict.Add(key, newChunk);

            return newChunk;
        }
    }
}