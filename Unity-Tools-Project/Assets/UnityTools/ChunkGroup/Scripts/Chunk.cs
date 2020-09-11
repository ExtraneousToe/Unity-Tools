using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    /// <summary>
    /// Chunk.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Chunk : BaseBehaviour
    {
        // required component references
        [SerializeField]
        private MeshFilter m_MeshFilter;

        public MeshFilter ChunkFilter { get { return m_MeshFilter; } }

        [SerializeField]
        private MeshRenderer m_MeshRenderer;

        public MeshRenderer ChunkRenderer { get { return m_MeshRenderer; } }

        [SerializeField]
        private MeshCollider m_MeshCollider;

        public MeshCollider ChunkCollider { get { return m_MeshCollider; } }

        /// <summary>
        /// The parent.
        /// </summary>
        private ChunkGroup m_Parent;
        public ChunkGroup Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        /// <summary>
        /// The piece grid.
        /// </summary>
        private ChunkPiece[,,] m_PieceGrid;
        public ChunkPiece[,,] PieceGrid
        {
            get { return m_PieceGrid; }
        }

        /// <summary>
        /// The size of the chunk.
        /// </summary>
        public int ChunkSize
        {
            get { return Parent.ChunkSize; }
        }

        public float ChunkScale
        {
            get { return Parent ? Parent.ChunkScale : 1f; }
        }

        /// <summary>
        /// Gets the drawer from the parent.
        /// </summary>
        /// <value>The drawer.</value>
        public ChunkDrawerSO Drawer
        {
            get
            {
                return Parent.Drawer;
            }
        }

        /// <summary>
        /// Gets the generator from the parent.
        /// </summary>
        /// <value>The generator.</value>
        public GroupGeneratorSO Generator
        {
            get
            {
                return Parent.Generator;
            }
        }

        /// <summary>
        /// The group coordinate.
        /// </summary>
        private Vector3Int m_GroupCoord;
        public Vector3Int GroupCoord
        {
            get { return m_GroupCoord; }
            set
            {
                m_GroupCoord = value;
            }
        }

        /// <summary>
        /// The active routine.
        /// This is set and used for generation and drawing psuedo-asyncronously
        /// </summary>
        private Coroutine m_ActiveRoutine;
        public Coroutine ActiveRoutine
        {
            get { return m_ActiveRoutine; }
            set
            {
                // if there is an active routine at the moment
                // and the new one is not the same one
                if (m_ActiveRoutine != null && m_ActiveRoutine != value)
                {
                    // stop the old one
                    StopCoroutine(m_ActiveRoutine);
                }

                m_ActiveRoutine = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is busy based on the 
        /// existence of an active routine.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get { return ActiveRoutine != null; }
        }

        /// <summary>
        /// Flag for setting the Chunk to be redrawn/recalculated
        /// </summary>
        private bool m_IsDirty;
        public bool IsDirty
        {
            get { return m_IsDirty; }
        }

        public void MakeDirty()
        {
            m_IsDirty = true;
        }

        public ChunkPiece this[Vector3Int chunkCoord]
        {
            get
            {
                return this[chunkCoord.x, chunkCoord.y, chunkCoord.z];
            }
            set
            {
                this[chunkCoord.x, chunkCoord.y, chunkCoord.z] = value;
            }
        }

        public ChunkPiece this[int x, int y, int z]
        {
            get
            {
                try
                {
                    return PieceGrid[x, y, z];
                }
                catch (System.IndexOutOfRangeException ioore)
                {
                    throw new OutOfChunkException(
                        string.Format("{0},{1},{2}", x, y, z)
                    );
                }
            }
            set
            {
                try
                {
                    PieceGrid[x, y, z] = value;
                }
                catch (System.IndexOutOfRangeException ioore)
                {
                    throw new OutOfChunkException(
                        string.Format("{0},{1},{2}", x, y, z)
                    );
                }
            }
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            // ensure that the required components are referenced
            if (!m_MeshFilter)
                m_MeshFilter = GetComponent<MeshFilter>();
            if (!m_MeshRenderer)
                m_MeshRenderer = GetComponent<MeshRenderer>();
            if (!m_MeshCollider)
                m_MeshCollider = GetComponent<MeshCollider>();

            Mesh m = new Mesh();
            m.name = name;

            m_MeshFilter.sharedMesh = m;
            m_MeshCollider.sharedMesh = m;
        }

        /// <summary>
        /// Sets the chunk variables.
        /// </summary>
        /// <param name="chunkSize">Chunk size.</param>
        public void SetChunkVars(int chunkSize)
        {
            m_PieceGrid = new ChunkPiece[chunkSize, chunkSize, chunkSize];

            m_IsDirty = true;
        }

        /// <summary>
        /// Gets the piece using a chunkCoordinate.
        /// Will rollover onto adjacent Chunks.
        /// </summary>
        /// <returns>The piece rollover.</returns>
        /// <param name="chunkCoord">Chunk coordinate.</param>
        public ChunkPiece GetPieceRollover(Vector3Int chunkCoord)
        {
            return GetPieceRollover(chunkCoord.x, chunkCoord.y, chunkCoord.z);
        }

        /// <summary>
        /// Gets the piece using a chunkCoordinate.
        /// Will rollover onto adjacent Chunks.
        /// </summary>
        /// <returns>The piece rollover.</returns>
        /// <param name="chunkCoord">Chunk coordinate.</param>
        public ChunkPiece GetPieceRollover(int x, int y, int z)
        {
            try
            {
                return this[x, y, z];
            }
            catch (OutOfChunkException ooce)
            {
                Vector3Int worldCoord = ConvertChunkToWorldCoord(x, y, z);
                Vector3Int[] coordPair = Parent.ConvertWorldCoordToGroupAndChunk(worldCoord);

                Chunk chunk = Parent[coordPair[0]];
                return chunk[coordPair[1]];
            }
        }

        /// <summary>
        /// Sets the piece, with rollover.
        /// </summary>
        /// <returns>The piece rollover.</returns>
        /// <param name="chunkCoord">Chunk coordinate.</param>
        /// <param name="val">Value.</param>
        public void SetPieceRollover(Vector3Int chunkCoord, ChunkPiece val)
        {
            SetPieceRollover(chunkCoord.x, chunkCoord.y, chunkCoord.z, val);
        }

        /// <summary>
        /// Sets the piece, with rollover.
        /// </summary>
        /// <returns>The piece rollover.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="val">Value.</param>
        public void SetPieceRollover(int x, int y, int z, ChunkPiece val)
        {
            try
            {
                this[x, y, z] = val;

                MakeDirty();
            }
            catch (OutOfChunkException ooce)
            {
                Vector3Int worldCoord = ConvertChunkToWorldCoord(x, y, z);
                Vector3Int[] coordPair = Parent.ConvertWorldCoordToGroupAndChunk(worldCoord);

                Chunk chunk = Parent[coordPair[0]];
                chunk[coordPair[1]] = val;
            }
        }

        /// <summary>
        /// Converts the chunk coordinate to world coordinate.
        /// </summary>
        /// <returns>The chunk to world coordinate.</returns>
        /// <param name="chunkCoord">Chunk coordinate.</param>
        public Vector3Int ConvertChunkToWorldCoord(Vector3Int chunkCoord)
        {
            return ConvertChunkToWorldCoord(chunkCoord.x, chunkCoord.y, chunkCoord.z);
        }

        /// <summary>
        /// Converts the chunk coordinate to world coordinate.
        /// </summary>
        /// <returns>The chunk to world coordinate.</returns>
        /// <param name="chunkCoord">Chunk coordinate.</param>
        public Vector3Int ConvertChunkToWorldCoord(int x, int y, int z)
        {
            return new Vector3Int(
                GroupCoord.x * ChunkSize + x,
                GroupCoord.y * ChunkSize + y,
                GroupCoord.z * ChunkSize + z
            );
        }

        /// <summary>
        /// Updates the instance after all regular update() calls.
        /// </summary>
        protected void LateUpdate()
        {
            // if the instance is 'dirty'
            if (IsDirty && !IsBusy)
            {
                // recalculate and rerender the mesh
                if (Drawer)
                    ActiveRoutine = Drawer.DrawChunk(this);
                else
                    // if no drawer, clear the mesh
                    ChunkFilter.sharedMesh.Clear();

                // no longer dirty
                m_IsDirty = false;
            }
        }

        protected void OnDrawGizmosSelected()
        {
            if (!Parent) return;

            Vector3 cubeSize = Vector3.one * ChunkSize;

            Gizmos.DrawWireCube(
                transform.position + cubeSize * 0.5f * ChunkScale,
                cubeSize * ChunkScale
            );
        }
    }
}