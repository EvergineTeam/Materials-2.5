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
    public class EnvironmentMaterial : DeferredMaterial
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
        /// The environment texture
        /// </summary>
        private TextureCube environment;

        /// <summary>
        /// The environment path
        /// </summary>
        [DataMember]
        private string environmentPath;

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
            new ShaderTechnique("Simple", "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormal.VertexFormat),

            new ShaderTechnique("L",    "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPosition.VertexFormat,                          new string[] { "LIT" },             new string[] { "LIT" }),
            new ShaderTechnique("LD",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionTexture.VertexFormat,                   new string[] { "LIT", "DIFF" },     new string[] { "LIT", "DIFF" }),
            new ShaderTechnique("D",    "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTexture.VertexFormat,             new string[] { "DIFF" },            new string[] { "DIFF" }),

            new ShaderTechnique("E",    "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormal.VertexFormat,                    null,                               new string[] { "ENV" }),
            new ShaderTechnique("LE",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPosition.VertexFormat,                          new string[] { "LIT" },             new string[] { "LIT", "ENV" }),
            new ShaderTechnique("LFDE", "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionTexture.VertexFormat,                   new string[] { "LIT", "DIFF" },     new string[] { "LIT", "FRES", "DIFF", "ENV" }),
            new ShaderTechnique("LDE",  "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionTexture.VertexFormat,                   new string[] { "LIT", "DIFF" },     new string[] { "LIT", "DIFF", "ENV" }),
            new ShaderTechnique("FDE",  "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTexture.VertexFormat,             new string[] { "DIFF" },            new string[] { "FRES", "DIFF", "ENV" }),
            new ShaderTechnique("DE",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTexture.VertexFormat,             new string[] { "DIFF" },            new string[] { "DIFF", "ENV" }),

            new ShaderTechnique("EN",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTangentTexture.VertexFormat,     new string[] { "NORMAL" },          new string[] { "ENV", "NORMAL" }),
            new ShaderTechnique("FDEN",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTangentTexture.VertexFormat,   new string[] { "DIFF", "NORMAL" },  new string[] { "FRES", "DIFF", "ENV", "NORMAL" }),
            new ShaderTechnique("DEN",   "vsEnvironmentMaterial", "psEnvironmentMaterial", VertexPositionNormalTangentTexture.VertexFormat,    new string[] { "DIFF", "NORMAL" },  new string[] { "DIFF", "ENV", "NORMAL" }),

            // GBuffer pass
            new ShaderTechnique("G",   "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       null),
            new ShaderTechnique("GN",  "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL" }),
            new ShaderTechnique("GD",  "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "DEPTH" }),
            new ShaderTechnique("GDN", "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "DEPTH" }),
            new ShaderTechnique("GR",  "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormal.VertexFormat,                null,                       new string[] { "MRT" }),
            new ShaderTechnique("GRN", "LPPGBuffer", "vsGBuffer", "LPPGBuffer", "psGBuffer", VertexPositionNormalTangentTexture.VertexFormat,  new string[] { "NORMAL" },  new string[] { "NORMAL", "MRT" }),
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
            /// The camera position
            /// </summary>
            [FieldOffset(0)]
            public Vector3 CameraPosition;

            /// <summary>
            /// The fresnel Factor
            /// </summary>
            [FieldOffset(12)]
            public float FresnelFactor;

            /// <summary>
            /// The diffuse color
            /// </summary>
            [FieldOffset(16)]
            public Vector3 DiffuseColor;

            /// <summary>
            /// The Environment Amount
            /// </summary>
            [FieldOffset(28)]
            public float EnvironmentAmount;

            /// <summary>
            /// The ambient color
            /// </summary>
            [FieldOffset(32)]
            public Vector3 AmbientColor;

            /// <summary>
            /// Fix texture coordinates in OpenGL
            /// </summary>
            [FieldOffset(44)]
            public float TexCoordFix;
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
        /// Gets or sets the fresnel factor value.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0, 6, 0.1f)]
        public float FresnelFactor { get; set; }

        /// <summary>
        /// Gets or sets the environment amount value.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float EnvironmentAmount { get; set; }

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
        /// Gets or sets the environment texture path
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Cubemap)]
        public string EnvironmentPath
        {
            get
            {
                return this.environmentPath;
            }

            set
            {
                this.environmentPath = value;
                this.RefreshTexture(this.environmentPath, ref this.environment);
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
        /// Gets or sets a value indicating whether [fresnel enable].
        /// </summary>
        [DataMember]
        public bool FresnelEnabled { get; set; }

        /// <summary>
        /// Gets or sets the diffuse.
        /// </summary>
        /// <value>
        /// The diffuse.
        /// </value>
        /// <exception cref="System.NullReferenceException">Diffuse texture cannot be null.</exception>
        [DontRenderProperty]
        public Texture Diffuse
        {
            get
            {
                return this.diffuse;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Diffuse texture cannot be null.");
                }

                this.diffuse = value;
            }
        }

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        /// <exception cref="System.NullReferenceException">Environment texture cannot be null</exception>
        [DontRenderProperty]
        public TextureCube Environment
        {
            get
            {
                return this.environment;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Environment texture cannot be null");
                }

                this.environment = value;
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
            "Simple",
            "L",
            "LD",
            "D",
            "E",
            "LE",
            "LFDE",
            "LDE",
            "FDE",
            "DE",
            "EN",
            "FDEN",
            "DEN",

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

                    if (this.FresnelEnabled && this.diffuse != null && this.environment != null)
                    {
                        technique += "F";
                    }

                    if (this.diffuse != null)
                    {
                        technique += "D";
                    }

                    if (this.environment != null)
                    {
                        technique += "E";
                    }

                    if (!this.LightingEnabled && this.normal != null && this.environment != null)
                    {
                        technique += "N";
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
        /// Initializes a new instance of the <see cref="EnvironmentMaterial"/> class.
        /// </summary>
        public EnvironmentMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        public EnvironmentMaterial(Type layerType)
            : base(layerType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentMaterial"/> class.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color.</param>
        /// <param name="layerType">The associated layer</param>
        public EnvironmentMaterial(Color diffuseColor, Type layerType)
            : this(layerType)
        {
            this.DiffuseColor = diffuseColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffusePath">The diffuse path.</param>
        /// <param name="environmentPath">The environment path.</param>
        /// <param name="normalPath">The normal path.</param>
        public EnvironmentMaterial(Type layerType, string diffusePath = null, string environmentPath = null, string normalPath = null)
            : this(layerType)
        {
            this.environmentPath = environmentPath;
            this.diffusePath = diffusePath;
            this.normalPath = normalPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentMaterial"/> class.
        /// </summary>
        /// <param name="layerType">The associated layer</param>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="normal">The normal.</param>
        public EnvironmentMaterial(Type layerType, Texture diffuse = null, TextureCube environment = null, Texture normal = null)
            : this(layerType)
        {
            this.diffuse = diffuse;
            this.environment = environment;
            this.normal = normal;
        }

        /// <summary>
        /// Defaults the values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.EnvironmentAmount = 0.4f;
            this.FresnelFactor = 0.0f;
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

            this.LoadTexture(this.diffusePath, ref this.diffuse);
            this.LoadTexture(this.normalPath, ref this.normal);

            if (!string.IsNullOrEmpty(this.environmentPath))
            {
                this.RefreshTexture(this.environmentPath, ref this.environment);
            }
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
                this.shaderParameters.CameraPosition = camera.Position;
                this.shaderParameters.FresnelFactor = this.FresnelFactor;
                this.shaderParameters.DiffuseColor = this.diffuseColor;
                this.shaderParameters.EnvironmentAmount = this.EnvironmentAmount;
                this.shaderParameters.AmbientColor = this.ambientColor;
                this.shaderParameters.TexCoordFix = this.renderManager.GraphicsDevice.RenderTargets.RenderTargetActive ? 1 : -1;

                this.Parameters = this.shaderParameters;

                if (this.diffuse != null)
                {
                    this.graphicsDevice.SetTexture(this.diffuse, 0);
                }

                if (this.environment != null)
                {
                    this.graphicsDevice.SetTexture(this.environment, 1);
                }

                this.LightingTexture = camera.LightingRT;
                if (this.LightingTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.LightingTexture, 2);
                }

                if (camera.GBufferRT0 != null)
                {
                    this.graphicsDevice.SetTexture(camera.GBufferRT0, 3);
                }

                if (this.normal != null)
                {
                    this.graphicsDevice.SetTexture(this.normal, 4);
                }
            }
            else
            {
                this.gbufferShaderParameters.SpecularPower = this.SpecularPower;
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
