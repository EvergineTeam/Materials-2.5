#region File Description
//-----------------------------------------------------------------------------
// StandardMaterial
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Linq;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Common.Graphics.VertexFormats;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.Materials
{
    /// <summary>
    /// Light PrePass Material
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Materials")]
    public class StandardMaterial : DeferredMaterial
    {
        /// <summary>
        /// The diffuse color
        /// </summary>
        [DataMember]
        private Vector3 diffuseColor;

        /// <summary>
        /// The emissive color
        /// </summary>
        [DataMember]
        private Vector3 emissiveColor;

        /// <summary>
        /// The ambient color
        /// </summary>
        [DataMember]
        private Vector3 ambientColor;

        /// <summary>
        /// The diffuse texture
        /// </summary>
        private Texture diffuse;

        /// <summary>
        /// The diffuse path
        /// </summary>
        [DataMember]
        private string diffusePath;

        /// <summary>
        /// The ambient texture
        /// </summary>
        private TextureCube ambient;

        /// <summary>
        /// The ambient path
        /// </summary>
        [DataMember]
        private string ambientPath;

        /// <summary>
        /// The emissive texture
        /// </summary>
        private Texture emissive;

        /// <summary>
        /// The emissive path
        /// </summary>
        [DataMember]
        private string emissivePath;

        /// <summary>
        /// The specular
        /// </summary>
        private Texture specular;

        /// <summary>
        /// The specular path
        /// </summary>
        [DataMember]
        private string specularPath;

        /// <summary>
        /// The normal texture
        /// </summary>
        private Texture normal;

        /// <summary>
        /// The normal path
        /// </summary>
        [DataMember]
        private string normalPath;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            // Forward pass            
            new ShaderTechnique("Simple",   "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat),
            
            new ShaderTechnique("L",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT" }),
            new ShaderTechnique("LA",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI" }),
            new ShaderTechnique("LAE",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "EMIS" }),
            new ShaderTechnique("LAES",     "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "EMIS", "SPEC" }),
            new ShaderTechnique("LAESD",    "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "EMIS", "SPEC", "DIFF" }),
            new ShaderTechnique("LAESDV",   "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "EMIS", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LAESDVT",  "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "EMIS", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAS",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "SPEC" }),
            new ShaderTechnique("LASD",     "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "SPEC", "DIFF" }),
            new ShaderTechnique("LASDV",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LASDVT",   "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAD",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "DIFF" }),
            new ShaderTechnique("LADV",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LADVT",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "VCOLOR" }),
            new ShaderTechnique("LAVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "AMBI", "ATEST" }),
            new ShaderTechnique("LE",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "EMIS" }),
            new ShaderTechnique("LES",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "EMIS", "SPEC" }),
            new ShaderTechnique("LESD",     "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "EMIS", "SPEC", "DIFF" }),
            new ShaderTechnique("LESDV",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LESDVT",   "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LED",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "EMIS", "DIFF" }),
            new ShaderTechnique("LEDV",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LEDVT",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LEV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "VCOLOR" }),
            new ShaderTechnique("LEVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "EMIS", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LET",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "EMIS", "ATEST" }),
            new ShaderTechnique("LS",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "SPEC" }),
            new ShaderTechnique("LSD",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "SPEC", "DIFF" }),
            new ShaderTechnique("LSDV",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LSDVT",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LSV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "SPEC", "VCOLOR" }),
            new ShaderTechnique("LSVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "SPEC", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LST",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "SPEC", "ATEST" }),
            new ShaderTechnique("LD",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "DIFF" }),
            new ShaderTechnique("LDV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LDVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LDT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "DIFF", "ATEST" }),
            new ShaderTechnique("LV",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "VCOLOR" }),
            new ShaderTechnique("LVT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "LIT", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LT",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "LIT", "ATEST" }),

            new ShaderTechnique("A",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI" }),
            new ShaderTechnique("AE",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "EMIS" }),
            new ShaderTechnique("AES",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "EMIS", "SPEC" }),
            new ShaderTechnique("AESD",     "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "EMIS", "SPEC", "DIFF" }),
            new ShaderTechnique("AESDV",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "EMIS", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("AESDVT",   "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "EMIS", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AS",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "SPEC" }),
            new ShaderTechnique("ASD",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "SPEC", "DIFF" }),
            new ShaderTechnique("ASDV",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("ASDVT",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AD",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "DIFF" }),
            new ShaderTechnique("ADV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("ADVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AV",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "VCOLOR" }),
            new ShaderTechnique("AVT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AT",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "AMBI", "ATEST" }),
            new ShaderTechnique("E",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "EMIS" }),
            new ShaderTechnique("ES",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "EMIS", "SPEC" }),
            new ShaderTechnique("ESD",      "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "EMIS", "SPEC", "DIFF" }),
            new ShaderTechnique("ESDV",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("ESDVT",    "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("ED",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "EMIS", "DIFF" }),
            new ShaderTechnique("EDV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "DIFF", "VCOLOR" }),
            new ShaderTechnique("EDVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("EV",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "VCOLOR" }),
            new ShaderTechnique("EVT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "EMIS", "VCOLOR", "ATEST" }),
            new ShaderTechnique("ET",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "EMIS", "ATEST" }),
            new ShaderTechnique("S",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "SPEC" }),
            new ShaderTechnique("SD",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "SPEC", "DIFF" }),
            new ShaderTechnique("SDV",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "SPEC", "DIFF", "VCOLOR" }),
            new ShaderTechnique("SDVT",     "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "SPEC", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("SV",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "SPEC", "VCOLOR" }),
            new ShaderTechnique("SVT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "SPEC", "VCOLOR", "ATEST" }),
            new ShaderTechnique("ST",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "SPEC", "ATEST" }),
            new ShaderTechnique("D",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "DIFF" }),
            new ShaderTechnique("DV",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "DIFF", "VCOLOR" }),
            new ShaderTechnique("DVT",      "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("DT",       "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "DIFF", "ATEST" }),
            new ShaderTechnique("V",        "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "VCOLOR" }),
            new ShaderTechnique("VT",       "vsStandardMaterial", "psStandardMaterial", VertexPositionColorTexture.VertexFormat,    new string[] { "VCOLOR" },  new string[] { "VCOLOR", "ATEST" }),
            new ShaderTechnique("T",        "vsStandardMaterial", "psStandardMaterial", VertexPositionTexture.VertexFormat,         null,                       new string[] { "ATEST" }),

            // GBuffer pass
            new ShaderTechnique("G",        "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       null),
            new ShaderTechnique("GN",       "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL" }),
            new ShaderTechnique("GD",       "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "DEPTH" }),
            new ShaderTechnique("GDN",      "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "DEPTH" }),
            new ShaderTechnique("GR",       "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "MRT" }),
            new ShaderTechnique("GRN",      "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "MRT" }),

#if ANDROID
            // Video texture
            new ShaderTechnique("Video", "vsStandardMaterialVideo", "psStandardMaterialVideo ", VertexPositionTexture.VertexFormat, null, null),  
#endif
        };

        #region Struct
        /// <summary>
        /// GBuffer Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct GBufferMaterialParameters
        {
            /// <summary>
            /// The Specular power
            /// </summary>
            [FieldOffset(0)]
            public float SpecularPower;

            /// <summary>
            /// Texture offset.
            /// </summary>
            [FieldOffset(4)]
            public Vector2 TextureOffset;
        }

        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 80)]
        private struct MaterialParameters
        {
            /// <summary>
            /// The camera position
            /// </summary>
            [FieldOffset(0)]
            public Vector3 CameraPosition;

            /// <summary>
            /// The reference alpha
            /// </summary>
            [FieldOffset(12)]
            public float ReferenceAlpha;

            /// <summary>
            /// The diffuse color
            /// </summary>
            [FieldOffset(16)]
            public Vector3 DiffuseColor;

            /// <summary>
            /// The alpha
            /// </summary>
            [FieldOffset(28)]
            public float Alpha;

            /// <summary>
            /// The ambient color
            /// </summary>
            [FieldOffset(32)]
            public Vector3 AmbientColor;

            /// <summary>
            /// The emissive color
            /// </summary>
            [FieldOffset(48)]
            public Vector3 EmissiveColor;

            /// <summary>
            /// Fix texture coordinates in OpenGL
            /// </summary>
            [FieldOffset(60)]
            public float TexCoordFix;

            /// <summary>
            /// Texture offset.
            /// </summary>
            [FieldOffset(64)]
            public Vector2 TextureOffset;
        }
        #endregion

        /// <summary>
        /// Handle the GBuffer shader parameters struct.
        /// </summary>
        private GBufferMaterialParameters gbufferShaderParameters;

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private MaterialParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>
        /// The alpha.
        /// </value>   
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float Alpha { get; set; }

        /// <summary>
        /// Gets or sets the reference alpha value.
        /// </summary>
        /// <value>
        /// The reference alpha.
        /// </value>      
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float ReferenceAlpha { get; set; }

        /// <summary>
        /// Gets or sets the specular power value.
        /// </summary>
        /// <value>
        /// The specular power.
        /// </value>      
        [DataMember]
        [RenderPropertyAsSlider(0, 255, 1)]
        public float SpecularPower { get; set; }

        /// <summary>
        /// Gets or sets the primary texcoord offset.
        /// </summary>
        [DataMember]
        public Vector2 TexcoordOffset { get; set; }

        /// <summary>
        /// Gets or sets the color of the diffuse.
        /// </summary>
        /// <value>
        /// The color of the diffuse.
        /// </value>
        public Color DiffuseColor
        {
            get
            {
                return Color.FromVector3(ref this.diffuseColor);
            }

            set
            {
                value.ToVector3(ref this.diffuseColor);
            }
        }

        /// <summary>
        /// Gets or sets the color of the emissive.
        /// </summary>
        /// <value>
        /// The color of the emissive.
        /// </value>
        public Color EmissiveColor
        {
            get
            {
                return Color.FromVector3(ref this.emissiveColor);
            }

            set
            {
                value.ToVector3(ref this.emissiveColor);
            }
        }

        /// <summary>
        /// Gets or sets the color of the ambient.
        /// </summary>
        /// <value>
        /// The color of the ambient.
        /// </value>
        public Color AmbientColor
        {
            get
            {
                return Color.FromVector3(ref this.ambientColor);
            }

            set
            {
                value.ToVector3(ref this.ambientColor);
            }
        }

        /// <summary>
        /// Gets or sets the diffuse texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string DiffusePath
        {
            get
            {
                return this.diffusePath;
            }

            set
            {
                this.diffusePath = value;
                this.RefreshTexture(this.diffusePath, ref this.diffuse);
            }
        }

        /// <summary>
        /// Gets or sets the normal texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string NormalPath
        {
            get
            {
                return this.normalPath;
            }

            set
            {
                this.normalPath = value;
                this.RefreshTexture(this.normalPath, ref this.normal);
            }
        }

        /// <summary>
        /// Gets or sets the specular texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string SpecularPath
        {
            get
            {
                return this.specularPath;
            }

            set
            {
                this.specularPath = value;
                this.RefreshTexture(this.specularPath, ref this.specular);
            }
        }

        /// <summary>
        /// Gets or sets the ambient texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Cubemap)]
        public string AmbientPath
        {
            get
            {
                return this.ambientPath;
            }

            set
            {
                this.ambientPath = value;
                this.RefreshTexture(this.ambientPath, ref this.ambient);
            }
        }

        /// <summary>
        /// Gets or sets the emissive texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string EmissivePath
        {
            get
            {
                return this.emissivePath;
            }

            set
            {
                this.emissivePath = value;
                this.RefreshTexture(this.emissivePath, ref this.emissive);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [lighting enable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [lighting enable]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool LightingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [vertex color enable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [vertex color enable]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool VertexColorEnable { get; set; }

        /// <summary>
        /// Gets or sets the diffuse.
        /// </summary>
        /// <value>
        /// The diffuse.
        /// </value>
        [DontRenderProperty]
        public Texture Diffuse
        {
            get
            {
                return this.diffuse;
            }

            set
            {
                this.diffuse = value;
            }
        }

        /// <summary>
        /// Gets or sets the emissive.
        /// </summary>
        /// <value>
        /// The emissive.
        /// </value>
        [DontRenderProperty]
        public Texture Emissive
        {
            get
            {
                return this.emissive;
            }

            set
            {
                this.emissive = value;
            }
        }

        /// <summary>
        /// Gets or sets the specular.
        /// </summary>
        /// <value>
        /// The specular.
        /// </value>
        [DontRenderProperty]
        public Texture Specular
        {
            get
            {
                return this.specular;
            }

            set
            {
                this.specular = value;
            }
        }

        /// <summary>
        /// Gets or sets the ambient.
        /// </summary>
        /// <value>
        /// The ambient.
        /// </value>
        [DontRenderProperty]
        public TextureCube Ambient
        {
            get
            {
                return this.ambient;
            }

            set
            {
                this.ambient = value;
            }
        }

        /// <summary>
        /// Gets the lighting texture
        /// </summary>
        [DontRenderProperty]
        public Texture LightingTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the GBuffer texture
        /// </summary>
        [DontRenderProperty]
        public Texture GBufferTexture
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>
        /// The normal.
        /// </value>
        [DontRenderProperty]
        public Texture Normal
        {
            get
            {
                return this.normal;
            }

            set
            {
                this.normal = value;
            }
        }

#if ANDROID
        /// <summary>
        /// Gets the diffuse video texture
        /// </summary>
        [DontRenderProperty]
        public VideoTexture VideoTexture
        {
            get { return this.Diffuse as VideoTexture; }
        }
#endif

        #region techniques array

        /// <summary>
        /// The techniques array
        /// </summary>
        private static string[] s = new string[]
                {
                    "Simple",
                    "L",
                    "LA",
                    "LAE",
                    "LAES",
                    "LAESD",
                    "LAESDV",
                    "LAESDVT",
                    "LAS",
                    "LASD",
                    "LASDV",
                    "LASDVT",
                    "LAD",
                    "LADV",
                    "LADVT",
                    "LAV",
                    "LAVT",
                    "LAT",                    
                    "LE",
                    "LES",
                    "LESD",
                    "LESDV",
                    "LESDVT",
                    "LED",
                    "LEDV",
                    "LEDVT",
                    "LEV",
                    "LEVT",
                    "LET",
                    "LS",
                    "LSD",
                    "LSDV",
                    "LSDVT",
                    "LSV",
                    "LSVT",
                    "LST",
                    "LD",
                    "LDV",
                    "LDVT",
                    "LDT",
                    "LV",
                    "LVT",
                    "LT",
                    "A",
                    "AE",
                    "AES",
                    "AESD",
                    "AESDV",
                    "AESDVT",
                    "AS",
                    "ASD",
                    "ASDV",
                    "ASDVT",
                    "AD",
                    "ADV",
                    "ADVT",
                    "AV",
                    "AVT",
                    "AT",                    
                    "E",
                    "ES",
                    "ESD",
                    "ESDV",
                    "ESDVT",
                    "ED",
                    "EDV",
                    "EDVT",
                    "EV",
                    "EVT",
                    "ET",
                    "S",
                    "SD",
                    "SDV",
                    "SDVT",
                    "SV",
                    "SVT",
                    "ST",
                    "D",
                    "DV",
                    "DVT",
                    "DT",
                    "V",
                    "VT",
                    "T",

                    "G",
                    "GN",
                    "GD",
                    "GDN",
                    "GR",
                    "GRN",

                    "Video",
                };

        #endregion

        /// <summary>
        /// Gets the current technique.
        /// </summary>
        /// <value>
        /// The current technique.
        /// </value>
        [DontRenderProperty]
        public override string CurrentTechnique
        {
            get
            {
                string technique = string.Empty;
                int index = 0;

                if (this.DeferredLightingPass == DeferredLightingPass.GBufferPass)
                {
                    technique += "G";

                    if (this.graphicsDevice != null)
                    {
                        if (this.graphicsDevice.RenderTargets.IsDepthAsTextureSupported)
                        {
                            technique += "D";
                        }
                        else if (this.graphicsDevice.RenderTargets.IsMRTsupported)
                        {
                            technique += "R";
                        }
                    }

                    if (this.normal != null)
                    {
                        technique += "N";
                    }
                }
                else
                {
                    if (this.LightingEnabled)
                    {
                        technique += "L";
                    }

                    if (this.ambient != null)
                    {
                        technique += "A";
                    }

                    if (this.emissive != null)
                    {
                        technique += "E";
                    }

                    if (this.specular != null)
                    {
                        technique += "S";
                    }

                    if (this.diffuse != null)
                    {
                        technique += "D";
                    }

                    if (this.VertexColorEnable)
                    {
                        technique += "V";
                    }

                    if (this.ReferenceAlpha > 0.0f)
                    {
                        technique += "T";
                    }
                }

                #if ANDROID
                if (this.VideoTexture != null)
                {
                    technique = "Video";
                }
                #endif

                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == technique)
                    {
                        index = i;
                        break;
                    }
                }

                ShaderTechnique seletedTechnique = techniques[index];

                // Lazy initialization
                if (!seletedTechnique.IsInitialized)
                {
                    if (this.DeferredLightingPass == DeferredLightingPass.GBufferPass)
                    {
                        this.Parameters = this.gbufferShaderParameters;
                    }
                    else
                    {
                        this.Parameters = this.shaderParameters;
                    }

                    seletedTechnique.Initialize(this);
                }

                return seletedTechnique.Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        public StandardMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        public StandardMaterial(Type layerType)
            : base(layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color.</param>
        /// <param name="layerType">The associated layer</param>
        public StandardMaterial(Color diffuseColor, Type layerType)
            : this(layerType)
        {
            this.DiffuseColor = diffuseColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffusePath">The diffuse path.</param>
        /// <param name="normalPath">The normal path.</param>
        /// <param name="specularPath">The specular path.</param>
        /// <param name="ambientPath">The ambient path.</param>
        /// <param name="emissivePath">The emissive path.</param>
        public StandardMaterial(Type layerType, string diffusePath = null, string normalPath = null, string specularPath = null, string ambientPath = null, string emissivePath = null)
            : this(layerType)
        {
            this.ambientPath = ambientPath;
            this.emissivePath = emissivePath;
            this.specularPath = specularPath;
            this.diffusePath = diffusePath;
            this.normalPath = normalPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="specular">The specular.</param>
        /// <param name="ambient">The ambient.</param>
        /// <param name="emissive">The emissive.</param>
        public StandardMaterial(Type layerType, Texture diffuse = null, Texture normal = null, Texture specular = null, TextureCube ambient = null, Texture emissive = null)
            : this(layerType)
        {
            this.diffuse = diffuse;
            this.normal = normal;
            this.specular = specular;
            this.ambient = ambient;
            this.emissive = emissive;
        }

        /// <summary>
        /// Defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Alpha = 1;
            this.ReferenceAlpha = 0;
            this.SpecularPower = 64;
            this.DiffuseColor = Color.White;
            this.EmissiveColor = Color.White;
            this.AmbientColor = Color.Black;
            this.LightingEnabled = true;
            this.shaderParameters = new MaterialParameters();
            this.gbufferShaderParameters = new GBufferMaterialParameters();

            // ToDo: You need to initialize at least one technique
            this.Parameters = this.shaderParameters;
            techniques[0].Initialize(this);
        }

        /// <summary>
        /// Initializes the specified assets.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public override void Initialize(WaveEngine.Framework.Services.AssetsContainer assets)
        {
            base.Initialize(assets);

            this.LoadTexture(this.diffusePath, ref this.diffuse);
            this.LoadTexture(this.emissivePath, ref this.emissive);
            this.LoadTexture(this.normalPath, ref this.normal);
            this.LoadTexture(this.specularPath, ref this.specular);
            this.RefreshTexture(this.ambientPath, ref this.ambient);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Check if this material require the specified pass
        /// </summary>
        /// <param name="pass">The deferred pass</param>
        /// <returns>True if this material require this pass</returns>
        public override bool RequireDeferredPass(DeferredLightingPass pass)
        {
            if (pass == DeferredLightingPass.GBufferPass)
            {
                return this.LightingEnabled || this.ambient != null;
            }

            return true;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="cached">if set to <c>true</c> [cached].</param>
        public override void SetParameters(bool cached)
        {
            base.SetParameters(cached);

            Camera camera = this.renderManager.CurrentDrawingCamera;

            if (this.DeferredLightingPass == DeferredLightingPass.ForwardPass)
            {
                this.shaderParameters.CameraPosition = camera.Position;
                this.shaderParameters.ReferenceAlpha = this.ReferenceAlpha;
                this.shaderParameters.DiffuseColor = this.diffuseColor;
                this.shaderParameters.EmissiveColor = this.emissiveColor;
                this.shaderParameters.AmbientColor = this.ambientColor;
                this.shaderParameters.Alpha = this.Alpha;
                this.shaderParameters.TextureOffset = this.TexcoordOffset;
                this.shaderParameters.TexCoordFix = this.renderManager.GraphicsDevice.RenderTargets.RenderTargetActive ? 1 : -1;

                this.Parameters = this.shaderParameters;

                if (this.diffuse != null)
                {
                    this.graphicsDevice.SetTexture(this.diffuse, 0);
                }

                if (this.emissive != null)
                {
                    this.graphicsDevice.SetTexture(this.emissive, 1);
                }

                if (this.specular != null)
                {
                    this.graphicsDevice.SetTexture(this.specular, 2);
                }

                if (this.ambient != null)
                {
                    this.graphicsDevice.SetTexture(this.ambient, 3);
                    this.graphicsDevice.SetTexture(camera.GBufferRT0, 5);
                }

                this.LightingTexture = camera.LightingRT;
                if (this.LightingTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.LightingTexture, 4);
                }
            }
            else
            {
                this.gbufferShaderParameters.SpecularPower = this.SpecularPower;
                this.gbufferShaderParameters.TextureOffset = this.TexcoordOffset;
                this.Parameters = this.gbufferShaderParameters;

                if (this.normal != null)
                {
                    this.graphicsDevice.SetTexture(this.normal, 0);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Refreshes the texture.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="texture">The texture.</param>
        private void RefreshTexture(string path, ref Texture texture)
        {
            this.UnloadTexture(ref texture);
            this.LoadTexture(path, ref texture);
        }

        /// <summary>
        /// Unload the texture.
        /// </summary>        
        /// <param name="texture">The texture.</param>
        private void UnloadTexture(ref Texture texture)
        {
            if (this.assetsContainer != null && texture != null && !string.IsNullOrEmpty(texture.AssetPath))
            {
                this.assetsContainer.UnloadAsset(texture.AssetPath);
                texture = null;
            }
        }

        /// <summary>
        /// Load the texture.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="texture">The texture.</param>
        private void LoadTexture(string path, ref Texture texture)
        {
            if (texture != null)
            {
                return;
            }

            if (this.assetsContainer != null && texture == null && !string.IsNullOrEmpty(path))
            {
                texture = this.assetsContainer.LoadAsset<Texture2D>(path);
            }
        }

        /// <summary>
        /// Refreshes the texture.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="texture">The texture.</param>
        private void RefreshTexture(string path, ref TextureCube texture)
        {
            if (this.assetsContainer != null && texture != null && !string.IsNullOrEmpty(texture.AssetPath))
            {
                this.assetsContainer.UnloadAsset(texture.AssetPath);
                texture = null;
            }

            if (this.assetsContainer != null && texture == null && !string.IsNullOrEmpty(path))
            {
                texture = this.assetsContainer.LoadAsset<TextureCube>(path);
            }
        }

        #endregion
    }
}