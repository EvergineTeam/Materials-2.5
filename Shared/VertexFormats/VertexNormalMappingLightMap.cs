// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;

#endregion

namespace WaveEngine.Materials.VertexFormats
{
    /// <summary>
    /// A vertex format structure containing position and two texture coordinates.
    /// </summary>
    public struct VertexNormalMappingLightMap : IBasicVertex
    {
        /// <summary>
        /// Vertex position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Vertex normal.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Vertex binormal.
        /// </summary>
        public Vector3 Binormal;

        /// <summary>
        /// Vertex tangent.
        /// </summary>
        public Vector3 Tangent;

        /// <summary>
        /// First vertex texture coordinate.
        /// </summary>
        public Vector2 TexCoord;

        /// <summary>
        /// Second vertex texture coordinate.
        /// </summary>
        public Vector2 TexCoord2;

        /// <summary>
        /// Vertex format of this vertex.
        /// </summary>
        public static readonly VertexBufferFormat VertexFormat;

        #region Properties

        /// <summary>
        /// Gets the vertex format.
        /// </summary>
        VertexBufferFormat IBasicVertex.VertexFormat
        {
            get
            {
                return VertexFormat;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexNormalMappingLightMap" /> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="binormal">The binormal.</param>
        /// <param name="tangent">The tangent.</param>
        /// <param name="texCoord">The first texture coordinate.</param>
        /// <param name="texCoord2">The tex coord2.</param>
        public VertexNormalMappingLightMap(Vector3 position, Vector3 normal, Vector3 binormal, Vector3 tangent, Vector2 texCoord, Vector2 texCoord2)
        {
            this.Position = position;
            this.Normal = normal;
            this.Binormal = binormal;
            this.Tangent = tangent;
            this.TexCoord = texCoord;
            this.TexCoord2 = texCoord2;
        }

        /// <summary>
        /// Initializes static members of the <see cref="VertexNormalMappingLightMap" /> struct.
        /// </summary>
        static VertexNormalMappingLightMap()
        {
            VertexFormat = new VertexBufferFormat(new VertexElementProperties[]
                {
                    new VertexElementProperties(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElementProperties(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElementProperties(24, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
                    new VertexElementProperties(36, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                    new VertexElementProperties(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElementProperties(56, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                });
        }
        #endregion
    }
}
