// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    public class ForwardMaterial : Material
    {
        /// <summary>
        /// The diffuse color
        /// </summary>
        [DataMember]
        private Vector3 diffuseColor;

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
        /// The light properties.
        /// </summary>
        public LightStruct Light;

        /// <summary>
        /// Closer light to the current object.
        /// </summary>
        private LightProperties nearbyLight;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            // Forward pass
            new ShaderTechnique("Simple",   "vsForwardMaterial", "psForwardMaterial", VertexPosition.VertexFormat),

            new ShaderTechnique("A",        "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "AMBI" },                             new string[] { "AMBI" }),
            new ShaderTechnique("AD",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "AMBI", "VTEX" },                     new string[] { "AMBI", "DIFF" }),
            new ShaderTechnique("ADV",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "AMBI", "VTEX", "VCOLOR" },           new string[] { "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("ADVT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "AMBI", "VTEX", "VCOLOR" },           new string[] { "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AV",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "AMBI", "VCOLOR" },                   new string[] { "AMBI", "VCOLOR" }),
            new ShaderTechnique("AVT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "AMBI", "VCOLOR" },                   new string[] { "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("AT",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "AMBI" },                             new string[] { "AMBI", "ATEST" }),
            new ShaderTechnique("D",        "vsForwardMaterial", "psForwardMaterial", VertexPositionTexture.VertexFormat,                 new string[] { "VTEX" },                             new string[] { "DIFF" }),
            new ShaderTechnique("DV",       "vsForwardMaterial", "psForwardMaterial", VertexPositionColorTexture.VertexFormat,            new string[] { "VTEX", "VCOLOR" },                   new string[] { "DIFF", "VCOLOR" }),
            new ShaderTechnique("DVT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionColorTexture.VertexFormat,            new string[] { "VTEX", "VCOLOR" },                   new string[] { "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("DT",       "vsForwardMaterial", "psForwardMaterial", VertexPositionTexture.VertexFormat,                 new string[] { "VTEX" },                             new string[] { "DIFF", "ATEST" }),
            new ShaderTechnique("V",        "vsForwardMaterial", "psForwardMaterial", VertexPositionColor.VertexFormat,                   new string[] { "VCOLOR" },                           new string[] { "VCOLOR" }),
            new ShaderTechnique("VT",       "vsForwardMaterial", "psForwardMaterial", VertexPositionColor.VertexFormat,                   new string[] { "VCOLOR" },                           new string[] { "VCOLOR", "ATEST" }),
            new ShaderTechnique("T",        "vsForwardMaterial", "psForwardMaterial", VertexPosition.VertexFormat,                        null,                                                new string[] { "ATEST" }),
            new ShaderTechnique("LP",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "POINT" }),
            new ShaderTechnique("LPA",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "POINT", "AMBI" }),
            new ShaderTechnique("LPAD",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "AMBI", "VTEX" },             new string[] { "LIT", "POINT", "AMBI", "DIFF" }),
            new ShaderTechnique("LPADV",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "POINT", "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LPADVT",   "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "POINT", "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LPAV",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "POINT", "AMBI", "VCOLOR" }),
            new ShaderTechnique("LPAVT",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "POINT", "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LPAT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "POINT", "AMBI", "ATEST" }),
            new ShaderTechnique("LPD",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "POINT", "DIFF" }),
            new ShaderTechnique("LPDV",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "POINT", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LPDVT",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "POINT", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LPDT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "POINT", "DIFF", "ATEST" }),
            new ShaderTechnique("LPV",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "POINT", "VCOLOR" }),
            new ShaderTechnique("LPVT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "POINT", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LPT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "POINT", "ATEST" }),
            new ShaderTechnique("L",        "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "DIRECTIONAL" }),
            new ShaderTechnique("LA",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "DIRECTIONAL", "AMBI" }),
            new ShaderTechnique("LAD",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "AMBI", "VTEX" },             new string[] { "LIT", "DIRECTIONAL", "AMBI", "DIFF" }),
            new ShaderTechnique("LADV",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "DIRECTIONAL", "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LADVT",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "DIRECTIONAL", "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAV",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "DIRECTIONAL", "AMBI", "VCOLOR" }),
            new ShaderTechnique("LAVT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "DIRECTIONAL", "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LAT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "DIRECTIONAL", "AMBI", "ATEST" }),
            new ShaderTechnique("LD",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "DIRECTIONAL", "DIFF" }),
            new ShaderTechnique("LDV",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "DIRECTIONAL", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LDVT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "DIRECTIONAL", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LDT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "DIRECTIONAL", "DIFF", "ATEST" }),
            new ShaderTechnique("LV",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "DIRECTIONAL", "VCOLOR" }),
            new ShaderTechnique("LVT",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "DIRECTIONAL", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LT",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "DIRECTIONAL", "ATEST" }),
            new ShaderTechnique("LS",       "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "SPOT" }),
            new ShaderTechnique("LSA",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "SPOT", "AMBI" }),
            new ShaderTechnique("LSAD",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "AMBI", "VTEX" },             new string[] { "LIT", "SPOT", "AMBI", "DIFF" }),
            new ShaderTechnique("LSADV",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "SPOT", "AMBI", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LSADVT",   "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "AMBI", "VTEX", "VCOLOR" },   new string[] { "LIT", "SPOT", "AMBI", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LSAV",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "SPOT", "AMBI", "VCOLOR" }),
            new ShaderTechnique("LSAVT",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "AMBI", "VCOLOR" },           new string[] { "LIT", "SPOT", "AMBI", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LSAT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT", "AMBI" },                     new string[] { "LIT", "SPOT", "AMBI", "ATEST" }),
            new ShaderTechnique("LSD",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "SPOT", "DIFF" }),
            new ShaderTechnique("LSDV",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "SPOT", "DIFF", "VCOLOR" }),
            new ShaderTechnique("LSDVT",    "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColorTexture.VertexFormat,      new string[] { "VLIT", "VTEX", "VCOLOR" },           new string[] { "LIT", "SPOT", "DIFF", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LSDT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalTexture.VertexFormat,           new string[] { "VLIT", "VTEX" },                     new string[] { "LIT", "SPOT", "DIFF", "ATEST" }),
            new ShaderTechnique("LSV",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "SPOT", "VCOLOR" }),
            new ShaderTechnique("LSVT",     "vsForwardMaterial", "psForwardMaterial", VertexPositionNormalColor.VertexFormat,             new string[] { "VLIT", "VCOLOR" },                   new string[] { "LIT", "SPOT", "VCOLOR", "ATEST" }),
            new ShaderTechnique("LST",      "vsForwardMaterial", "psForwardMaterial", VertexPositionNormal.VertexFormat,                  new string[] { "VLIT" },                             new string[] { "LIT", "SPOT", "ATEST" }),
        };

        #region Struct

        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 112)]
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
            /// The specular power
            /// </summary>
            [FieldOffset(44)]
            public float SpecularPower;

            /// <summary>
            /// Texture offset.
            /// </summary>
            [FieldOffset(48)]
            public Vector2 TextureOffset;

            /// <summary>
            /// Specular Intensity
            /// </summary>
            [FieldOffset(56)]
            public float SpecularIntensity;

            /// <summary>
            /// Light struct.
            /// </summary>
            [FieldOffset(64)]
            public LightStruct Light;
        }
        #endregion

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
        /// Gets or sets the specular intensity.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float SpecularIntensity { get; set; }

        /// <summary>
        /// Gets or sets the specular power value.
        /// </summary>
        /// <value>
        /// The specular power.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(1, 255, 1)]
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

        #region techniques array

        /// <summary>
        /// The techniques array
        /// </summary>
        private static string[] s = new string[]
                {
                    "Simple",
                    "A",
                    "AD",
                    "ADV",
                    "ADVT",
                    "AV",
                    "AVT",
                    "AT",
                    "D",
                    "DV",
                    "DVT",
                    "DT",
                    "V",
                    "VT",
                    "T",
                    "LP",
                    "LPA",
                    "LPAD",
                    "LPADV",
                    "LPADVT",
                    "LPAV",
                    "LPAVT",
                    "LPAT",
                    "LPD",
                    "LPDV",
                    "LPDVT",
                    "LPDT",
                    "LPV",
                    "LPVT",
                    "LPT",
                    "L",
                    "LA",
                    "LAD",
                    "LADV",
                    "LADVT",
                    "LAV",
                    "LAVT",
                    "LAT",
                    "LD",
                    "LDV",
                    "LDVT",
                    "LDT",
                    "LV",
                    "LVT",
                    "LT",
                    "LS",
                    "LSA",
                    "LSAD",
                    "LSADV",
                    "LSADVT",
                    "LSAV",
                    "LSAVT",
                    "LSAT",
                    "LSD",
                    "LSDV",
                    "LSDVT",
                    "LSDT",
                    "LSV",
                    "LSVT",
                    "LST",
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

                if (this.LightingEnabled)
                {
                    this.nearbyLight = this.SearchForNearbyLight();

                    if (this.nearbyLight != null)
                    {
                        technique += "L";

                        if (this.nearbyLight is PointLightProperties)
                        {
                            technique += "P";
                        }
                        else if (this.nearbyLight is SpotLightProperties)
                        {
                            technique += "S";
                        }
                    }
                }

                if (this.ambient != null)
                {
                    technique += "A";
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
                    this.Parameters = this.shaderParameters;
                    seletedTechnique.Initialize(this);
                }

                return seletedTechnique.Name;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardMaterial"/> class.
        /// </summary>
        public ForwardMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        public ForwardMaterial(Type layerType)
            : base(layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardMaterial"/> class.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color.</param>
        /// <param name="layerType">The associated layer</param>
        public ForwardMaterial(Color diffuseColor, Type layerType)
            : this(layerType)
        {
            this.DiffuseColor = diffuseColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffusePath">The diffuse path.</param>
        /// <param name="ambientPath">The ambient path.</param>
        public ForwardMaterial(Type layerType, string diffusePath = null, string ambientPath = null)
            : this(layerType)
        {
            this.ambientPath = ambientPath;
            this.diffusePath = diffusePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="ambient">The ambient.</param>
        public ForwardMaterial(Type layerType, Texture diffuse = null, TextureCube ambient = null)
            : this(layerType)
        {
            this.diffuse = diffuse;
            this.ambient = ambient;
        }

        /// <summary>
        /// Defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Alpha = 1;
            this.ReferenceAlpha = 0;
            this.SpecularPower = 32;
            this.SpecularIntensity = 1;
            this.DiffuseColor = Color.White;
            this.AmbientColor = Color.Black;
            this.LightingEnabled = false;
            this.shaderParameters = new MaterialParameters();

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
            this.RefreshTexture(this.ambientPath, ref this.ambient);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="cached">if set to <c>true</c> [cached].</param>
        public override void SetParameters(bool cached)
        {
            base.SetParameters(cached);

            Camera camera = this.renderManager.CurrentDrawingCamera;

            this.shaderParameters.CameraPosition = camera.Position;
            this.shaderParameters.ReferenceAlpha = this.ReferenceAlpha;
            this.shaderParameters.DiffuseColor = this.diffuseColor;
            this.shaderParameters.Alpha = this.Alpha;
            this.shaderParameters.AmbientColor = this.ambientColor;
            this.shaderParameters.SpecularPower = this.SpecularPower;
            this.shaderParameters.SpecularIntensity = this.SpecularIntensity;
            this.shaderParameters.TextureOffset = this.TexcoordOffset;

            if (this.LightingEnabled)
            {
                if (this.nearbyLight != null)
                {
                    // Common parameters
                    this.Light.Color = this.nearbyLight.Color.ToVector3();
                    this.Light.Intensity = this.nearbyLight.Intensity;

                    if (this.nearbyLight is DirectionalLightProperties)
                    {
                        DirectionalLightProperties directional = this.nearbyLight as DirectionalLightProperties;
                        this.Light.Direction = directional.Direction;
                    }
                    else if (this.nearbyLight is PointLightProperties)
                    {
                        PointLightProperties point = this.nearbyLight as PointLightProperties;
                        this.Light.Position = point.Position;
                        this.Light.LightRange = point.LightRange;
                    }
                    else
                    {
                        SpotLightProperties spot = this.nearbyLight as SpotLightProperties;
                        this.Light.Direction = spot.Direction;
                        this.Light.LightRange = spot.LightRange;
                        this.Light.Position = spot.Position;
                        this.Light.ConeAngle = (float)Math.Cos(spot.LightConeAngle / 2);
                    }

                    this.shaderParameters.Light = this.Light;
                }
            }

            this.Parameters = this.shaderParameters;

            if (this.diffuse != null)
            {
                this.graphicsDevice.SetTexture(this.diffuse, 0);
            }

            if (this.ambient != null)
            {
                this.graphicsDevice.SetTexture(this.ambient, 1);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Search which is the nearby light for the current object.
        /// </summary>
        /// <returns>The nearby light.</returns>
        private LightProperties SearchForNearbyLight()
        {
            LightProperties nearbyLight = null;
            float maxIntensity = 0;

            Vector3 objectPosition = this.Matrices.World.Translation;

            if (this.renderManager != null)
            {
                foreach (var light in this.renderManager.Lights)
                {
                    if (light.IsLightEnabled && light.Intensity > 0)
                    {
                        float intensity = 0;

                        if (light is DirectionalLightProperties)
                        {
                            intensity = light.Intensity;
                        }
                        else
                        {
                            Vector3 lightPosition = Vector3.Zero;
                            float lightRange = 0;

                            if (light is PointLightProperties)
                            {
                                PointLightProperties pointLight = light as PointLightProperties;
                                lightPosition = pointLight.Position;
                                lightRange = pointLight.LightRange;
                            }
                            else if (light is SpotLightProperties)
                            {
                                SpotLightProperties spotLight = light as SpotLightProperties;
                                lightPosition = spotLight.Position;
                                lightRange = spotLight.LightRange;
                            }

                            float distance = (lightPosition - objectPosition).Length();

                            if (distance < lightRange)
                            {
                                intensity = light.Intensity * (1 - (distance / lightRange));
                            }
                        }

                        intensity *= light.Color.Luminance;

                        if (intensity > maxIntensity)
                        {
                            maxIntensity = intensity;
                            nearbyLight = light;
                        }
                    }
                }
            }

            return nearbyLight;
        }

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
