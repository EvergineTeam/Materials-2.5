#region File Description
//-----------------------------------------------------------------------------
// VertexPositionColorDualTexture
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;

#endregion

namespace WaveEngine.Materials.VertexFormats
{
    /// <summary>
    /// A vertex format structure containing position and two texture coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct VertexPositionDualTexture : IBasicVertex
    {
        /// <summary>
        /// Vertex position.
        /// </summary>
        public Vector3 Position;

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
        /// Initializes a new instance of the <see cref="VertexPositionDualTexture"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="texCoord">The first texture coordinate.</param>
        /// <param name="texCoord2">The second texture coordinate.</param>
        public VertexPositionDualTexture(Vector3 position, Vector2 texCoord, Vector2 texCoord2)
        {
            this.Position = position;
            this.TexCoord = texCoord;
            this.TexCoord2 = texCoord2;
        }

        /// <summary>
        /// Initializes static members of the <see cref="VertexPositionDualTexture"/> struct.
        /// </summary>
        static VertexPositionDualTexture()
        {
            VertexFormat = new VertexBufferFormat(new VertexElementProperties[]
                {
                    new VertexElementProperties(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElementProperties(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElementProperties(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                });
        }
        #endregion
    }
}
