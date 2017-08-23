// Copyright © 2017 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials.VertexFormats;
#endregion

namespace WaveEngine.Materials
{
    /// <summary>
    /// Drawing mode for dual texture effect.
    /// </summary>
    public enum DualTextureMode
    {
        /// <summary>
        /// Lightmap blending (Difusse1 * (2 * Difusse2)).
        /// </summary>
        Lightmap,

        /// <summary>
        /// Multiplicative blending (Difusse1 * Difusse2)..
        /// </summary>
        Multiplicative,

        /// <summary>
        /// Additive blending. (Difusse1 + Difusse2).
        /// </summary>
        Additive,

        /// <summary>
        /// Second texture behaves as a decal.
        /// </summary>
        Mask,
    }

    /// <summary>
    /// Light PrePass Material
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Materials")]
    public class DualMaterial : DeferredMaterial
    {
        /// <summary>
        /// The diffuse color
        /// </summary>
        [DataMember]
        private Vector4 diffuseColor;

        /// <summary>
        /// The ambient color
        /// </summary>
        [DataMember]
        private Vector3 ambientColor;

        /// <summary>
        /// The texture1
        /// </summary>
        private Texture texture1;

        /// <summary>
        /// The texture1
        /// </summary>
        private Texture texture2;

        /// <summary>
        /// The diffuse1 path
        /// </summary>
        [DataMember]
        private string texture1Path;

        /// <summary>
        /// The diffuse1 path
        /// </summary>
        [DataMember]
        private string texture2Path;

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
            new ShaderTechnique("None", "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, null),
            new ShaderTechnique("F",    "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "FIRST" }),
            new ShaderTechnique("S",    "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "SECON" }),
            new ShaderTechnique("FSM",  "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "FIRST", "SECON", "MUL" }),
            new ShaderTechnique("FSA",  "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "FIRST", "SECON", "ADD" }),
            new ShaderTechnique("FSK",  "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "FIRST", "SECON", "MSK" }),
            new ShaderTechnique("FSI",  "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "FIRST", "SECON", "LMAP" }),

            new ShaderTechnique("L",    "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT" }),
            new ShaderTechnique("LF",   "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "FIRST" }),
            new ShaderTechnique("LS",   "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "SECON" }),
            new ShaderTechnique("LFSM", "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "FIRST", "SECON", "MUL" }),
            new ShaderTechnique("LFSA", "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "FIRST", "SECON", "ADD" }),
            new ShaderTechnique("LFSK", "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "FIRST", "SECON", "MSK" }),
            new ShaderTechnique("LFSI", "vsDualMaterial", "psDualMaterial", VertexPositionDualTexture.VertexFormat, null, new string[] { "LIT", "FIRST", "SECON", "LMAP" }),

            // GBuffer pass
            new ShaderTechnique("G",    "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       null),
            new ShaderTechnique("GN",   "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL" }),
            new ShaderTechnique("GD",   "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "DEPTH" }),
            new ShaderTechnique("GDN",  "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "DEPTH" }),
            new ShaderTechnique("GR",   "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "MRT" }),
            new ShaderTechnique("GRN",  "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "MRT" }),
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
        [StructLayout(LayoutKind.Explicit, Size = 48)]
        private struct MaterialParameters
        {
            /// <summary>
            /// The diffuse color
            /// </summary>
            [FieldOffset(0)]
            public Vector4 DiffuseColor;

            /// <summary>
            /// The ambient color
            /// </summary>
            [FieldOffset(16)]
            public Vector3 AmbientColor;

            /// <summary>
            /// Fix texture coordinates in OpenGL
            /// </summary>
            [FieldOffset(28)]
            public float TexCoordFix;

            /// <summary>
            /// The offset's texture1
            /// </summary>
            [FieldOffset(32)]
            public Vector2 TextureOffset1;

            /// <summary>
            /// The offset's texture2
            /// </summary>
            [FieldOffset(40)]
            public Vector2 TextureOffset2;
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
        /// Gets or sets the shader mode.
        /// </summary>
        [DataMember]
        public DualTextureMode EffectMode { get; set; }

        /// <summary>
        /// Gets or sets the specular power value.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0, 255, 1)]
        public float SpecularPower { get; set; }

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
                return Color.FromVector4(ref this.diffuseColor);
            }

            set
            {
                value.ToVector4(ref this.diffuseColor);
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
        public string Diffuse1Path
        {
            get
            {
                return this.texture1Path;
            }

            set
            {
                this.texture1Path = value;
                this.RefreshTexture(this.texture1Path, ref this.texture1);
            }
        }

        /// <summary>
        /// Gets or sets the diffuse texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string Diffuse2Path
        {
            get
            {
                return this.texture2Path;
            }

            set
            {
                this.texture2Path = value;
                this.RefreshTexture(this.texture2Path, ref this.texture2);
            }
        }

        /// <summary>
        /// Gets or sets the primary texcoord offset.
        /// </summary>
        [DataMember]
        public Vector2 TexcoordOffset1 { get; set; }

        /// <summary>
        /// Gets or sets the secondary texcoord offset.
        /// </summary>
        [DataMember]
        public Vector2 TexcoordOffset2 { get; set; }

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
        /// Gets or sets the diffuse texture1.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Diffuse texture cannot be null.</exception>
        [DontRenderProperty]
        public Texture Texture1
        {
            get
            {
                return this.texture1;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Diffuse texture cannot be null.");
                }

                this.texture1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the diffuse texture2.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Diffuse texture cannot be null.</exception>
        [DontRenderProperty]
        public Texture Texture2
        {
            get
            {
                return this.texture2;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Diffuse texture cannot be null.");
                }

                this.texture2 = value;
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
        /// Gets or sets a value indicating whether [lighting enable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [lighting enable]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool LightingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the normal.
        /// </summary>
        /// <value>
        /// The normal.
        /// </value>
        /// <exception cref="System.NullReferenceException">Normal texture cannot be null.</exception>
        [DontRenderProperty]
        public Texture Normal
        {
            get
            {
                return this.normal;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Normal texture cannot be null.");
                }

                this.normal = value;
            }
        }

        #region techniques array

        /// <summary>
        /// The techniques array
        /// </summary>
        private static string[] s = new string[]
                {
                    "None",
                    "F",
                    "S",
                    "FSM",
                    "FSA",
                    "FSK",
                    "FSI",
                    "L",
                    "LF",
                    "LS",
                    "LFSM",
                    "LFSA",
                    "LFSK",
                    "LFSI",

                    "G",
                    "GN",
                    "GD",
                    "GDN",
                    "GR",
                    "GRN",
                    "G",
                    "GN",
                    "GD",
                    "GDN",
                    "GR",
                    "GRN",
                };

        #endregion

        /// <summary>
        /// Gets gets or sets the current technique.
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

                    if (this.Texture1 != null)
                    {
                        technique += "F";
                    }

                    if (this.Texture2 != null)
                    {
                        technique += "S";
                    }

                    if (this.texture1 != null && this.texture2 != null)
                    {
                        switch (this.EffectMode)
                        {
                            case DualTextureMode.Multiplicative:
                                technique += "M";
                                break;
                            case DualTextureMode.Additive:
                                technique += "A";
                                break;
                            case DualTextureMode.Mask:
                                technique += "K";
                                break;
                            case DualTextureMode.Lightmap:
                                technique += "I";
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(technique))
                    {
                        technique = "None";
                    }
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
        /// Initializes a new instance of the <see cref="DualMaterial"/> class.
        /// </summary>
        public DualMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        public DualMaterial(Type layerType)
            : base(layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualMaterial"/> class.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color.</param>
        /// <param name="layerType">The associated layer</param>
        public DualMaterial(Color diffuseColor, Type layerType)
            : this(layerType)
        {
            this.DiffuseColor = diffuseColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="texture1Path">The texture1 path.</param>
        /// <param name="texture2Path">The texture2 path.</param>
        /// <param name="normalPath">The normal path.</param>
        public DualMaterial(Type layerType, string texture1Path = null, string texture2Path = null, string normalPath = null)
            : this(layerType)
        {
            this.texture1Path = texture1Path;
            this.texture2Path = texture2Path;
            this.normalPath = normalPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DualMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="texture1">The texture1.</param>
        /// <param name="texture2">The texture2.</param>
        /// <param name="normal">The normal.</param>
        public DualMaterial(Type layerType, Texture texture1 = null, Texture texture2 = null, Texture normal = null)
            : this(layerType)
        {
            this.texture1 = texture1;
            this.texture2 = texture2;
            this.normal = normal;
        }

        /// <summary>
        /// Defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.EffectMode = DualTextureMode.Multiplicative;
            this.DiffuseColor = Color.White;
            this.AmbientColor = Color.Black;
            this.SpecularPower = 64;
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

            this.LoadTexture(this.texture1Path, ref this.texture1);
            this.LoadTexture(this.texture2Path, ref this.texture2);
            this.LoadTexture(this.normalPath, ref this.normal);
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
                return this.LightingEnabled;
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
                this.shaderParameters.DiffuseColor = this.diffuseColor;
                this.shaderParameters.AmbientColor = this.ambientColor;
                this.shaderParameters.TexCoordFix = this.renderManager.GraphicsDevice.RenderTargets.RenderTargetActive ? 1 : -1;
                this.shaderParameters.TextureOffset1 = this.TexcoordOffset1;
                this.shaderParameters.TextureOffset2 = this.TexcoordOffset2;

                this.Parameters = this.shaderParameters;

                this.LightingTexture = camera.LightingRT;
                if (this.LightingTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.LightingTexture, 2);
                }

                if (this.texture1 != null)
                {
                    this.graphicsDevice.SetTexture(this.texture1, 0);
                }

                if (this.texture2 != null)
                {
                    this.graphicsDevice.SetTexture(this.texture2, 1);
                }
            }
            else
            {
                this.gbufferShaderParameters.SpecularPower = this.SpecularPower;
                this.gbufferShaderParameters.TextureOffset = this.TexcoordOffset1;
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
        #endregion
    }
}
