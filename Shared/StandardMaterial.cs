// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// <example>
    /// #### Recipes and samples
    /// <list type="bullet">
    ///    <item>
    ///        <description>[Materials samples](https://github.com/WaveEngine/Samples/tree/master/Materials)</description>
    ///    </item>
    /// </list>
    /// </example>
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Materials")]
    public partial class StandardMaterial : DeferredMaterial
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
        /// The diffuse1 texture
        /// </summary>
        private Texture diffuse1;

        /// <summary>
        /// The diffuse1 path
        /// </summary>
        [DataMember]
        private string diffuse1Path;

        /// <summary>
        /// The diffuse2 texture
        /// </summary>
        private Texture diffuse2;

        /// <summary>
        /// The diffuse2 path
        /// </summary>
        [DataMember]
        private string diffuse2Path;

        /// <summary>
        /// The ambient texture
        /// </summary>
        private TextureCube ibl;

        /// <summary>
        /// The ambient path
        /// </summary>
        [DataMember]
        private string iblPath;

        /// <summary>
        /// The reflection texture
        /// </summary>
        private TextureCube environmentTexture;

        /// <summary>
        /// The reflection path
        /// </summary>
        [DataMember]
        private string environmentPath;

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
        /// <value>
        /// Drawing mode for dual texture effect.
        /// </value>
        [DataMember]
        public DualTextureMode EffectMode { get; set; }

        /// <summary>
        /// Gets or sets the reference alpha value.
        /// </summary>
        /// <value>
        /// The reference alpha value.</value>
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float ReferenceAlpha { get; set; }

        /// <summary>
        /// Gets or sets the diffuse color.
        /// </summary>
        /// <value>
        /// The diffuse color.</value>
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
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>
        /// The alpha value.</value>
        [DataMember]
        [RenderPropertyAsSlider(0, 1, 0.1f)]
        public float Alpha { get; set; }

        /// <summary>
        /// Gets or sets the ambient color.
        /// </summary>
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
        /// Gets or sets the emissive color.
        /// </summary>
        /// <value>
        /// The emissive color</value>
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
        /// Gets or sets the specular power value.
        /// </summary>
        /// <value>
        /// The specular power</value>
        [DataMember]
        [RenderPropertyAsSlider(0, 255, 1)]
        public float SpecularPower { get; set; }

        /// <summary>
        /// Gets or sets the primary texcoord offset.
        /// </summary>
        /// <value>
        /// The primary texcoord offset.
        /// </value>
        [DataMember]
        public Vector2 TexcoordOffset1 { get; set; }

        /// <summary>
        /// Gets or sets the secoundary texcoord offset.
        /// </summary>
        /// <value>
        /// The secondary texcoord offset.
        /// </value>
        [DataMember]
        public Vector2 TexcoordOffset2 { get; set; }

        /// <summary>
        /// Gets or sets the fresnel factor.
        /// </summary>
        /// <value>
        /// The Fresnel factor.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0f, 4f, 0.1f)]
        public float FresnelFactor { get; set; }

        /// <summary>
        /// Gets or sets the IBL factor.
        /// </summary>
        /// <value>
        /// The IBL factor value.</value>
        [DataMember]
        [RenderPropertyAsSlider(0f, 4f, 0.1f)]
        public float IBLFactor { get; set; }

        /// <summary>
        /// Gets or sets the Environment Amount.
        /// <value>
        /// 1 for reflective materials (mirrors, polished metals, etc).
        /// 0 for roughness materials.
        /// </value>
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0f, 1f, 0.1f)]
        public float EnvironmentAmount { get; set; }

        /// <summary>
        /// Gets or sets the diffuse1 texture path
        /// </summary>
        /// <value>
        /// The path to the texture.</value>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string Diffuse1Path
        {
            get
            {
                return this.diffuse1Path;
            }

            set
            {
                this.diffuse1Path = value;
                this.RefreshTexture(this.diffuse1Path, ref this.diffuse1);
            }
        }

        /// <summary>
        /// Gets or sets the normal texture path
        /// </summary>
        /// <value>
        /// The path to the texture.
        /// </value>
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
        /// Gets or sets the diffuse2 texture path
        /// </summary>
        /// <value>
        /// The path to the texture.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Texture)]
        public string Diffuse2Path
        {
            get
            {
                return this.diffuse2Path;
            }

            set
            {
                this.diffuse2Path = value;
                this.RefreshTexture(this.diffuse2Path, ref this.diffuse2);
            }
        }

        /// <summary>
        /// Gets or sets the emissive texture path
        /// </summary>
        /// <value>
        /// The path to the texture.
        /// </value>
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
        /// Gets or sets the specular texture path
        /// </summary>
        /// <value>
        /// The path to the texture.
        /// </value>
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
        /// Gets or sets the IBL texture path
        /// </summary>
        /// <value>
        /// The path to the texture.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Cubemap)]
        public string IBLPath
        {
            get
            {
                return this.iblPath;
            }

            set
            {
                this.iblPath = value;
                this.RefreshTexture(this.iblPath, ref this.ibl);
            }
        }

        /// <summary>
        /// Gets or sets the Environment texture path
        /// </summary>
        /// <value>The path to the texture.
        /// </value>
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
                this.RefreshTexture(this.environmentPath, ref this.environmentTexture);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether lighting is enabled.
        /// </summary>
        [DataMember]
        public bool LightingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether vertex color is enabled.
        /// </summary>
        [DataMember]
        public bool VertexColorEnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Fresnel Term is enabled.
        /// </summary>
        [DataMember]
        public bool FresnelTermEnabled { get; set; }

        /// <summary>
        /// Gets or sets the diffuse1 texture.
        /// </summary>
        [DontRenderProperty]
        public Texture Diffuse1
        {
            get
            {
                return this.diffuse1;
            }

            set
            {
                this.diffuse1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the diffuse2 texture.
        /// </summary>
        [DontRenderProperty]
        public Texture Diffuse2
        {
            get
            {
                return this.diffuse2;
            }

            set
            {
                this.diffuse2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the emissive texture.
        /// </summary>
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
        /// Gets or sets the specular texture.
        /// </summary>
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
        /// Gets or sets the IBL texture
        /// </summary>
        [DontRenderProperty]
        public TextureCube IBLTexture
        {
            get
            {
                return this.ibl;
            }

            set
            {
                this.ibl = value;
            }
        }

        /// <summary>
        /// Gets or sets the environment texture cube.
        /// </summary>
        [DontRenderProperty]
        public TextureCube ENVTexture
        {
            get
            {
                return this.environmentTexture;
            }

            set
            {
                this.environmentTexture = value;
            }
        }

        /// <summary>
        /// Gets the lighting texture.
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
        /// Gets or sets the normal texture.
        /// </summary>
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
        /// Gets the video texture.
        /// </summary>
        [DontRenderProperty]
        public VideoTexture VideoTexture
        {
            get { return this.Diffuse1 as VideoTexture; }
        }
#endif
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
        /// <param name="layerId">The associated layer.</param>
        public StandardMaterial(int layerId)
            : base(layerId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color.</param>
        /// <param name="layerId">The associated layer.</param>
        public StandardMaterial(Color diffuseColor, int layerId)
            : this(layerId)
        {
            this.DiffuseColor = diffuseColor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="layerId">The associated layer.</param>
        /// <param name="diffuse1Path">The diffuse1 path.</param>
        /// <param name="diffuse2Path">The diffuse2 path.</param>
        /// <param name="normalPath">The normal path.</param>
        /// <param name="specularPath">The specular path.</param>
        /// <param name="iblPath">The ibl path.</param>
        /// <param name="emissivePath">The emissive path.</param>
        /// <param name="environmentPath">The environment path.</param>
        public StandardMaterial(int layerId, string diffuse1Path = null, string diffuse2Path = null, string normalPath = null, string specularPath = null, string iblPath = null, string emissivePath = null, string environmentPath = null)
            : this(layerId)
        {
            this.emissivePath = emissivePath;
            this.specularPath = specularPath;
            this.diffuse1Path = diffuse1Path;
            this.diffuse2Path = diffuse2Path;
            this.normalPath = normalPath;
            this.iblPath = iblPath;
            this.environmentPath = environmentPath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardMaterial"/> class.
        /// </summary>
        /// <param name="layerId">The associated layer.</param>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="specular">The specular.</param>
        /// <param name="ibl">The ibl.</param>
        /// <param name="emissive">The emissive.</param>
        public StandardMaterial(int layerId, Texture diffuse = null, Texture normal = null, Texture specular = null, TextureCube ibl = null, Texture emissive = null)
            : this(layerId)
        {
            this.diffuse1 = diffuse;
            this.normal = normal;
            this.specular = specular;
            this.ibl = ibl;
            this.emissive = emissive;
        }

        /// <summary>
        /// Sets the defaults values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.ReferenceAlpha = 0;
            this.DiffuseColor = Color.White;
            this.Alpha = 1;
            this.AmbientColor = Color.Black;
            this.EmissiveColor = Color.White;
            this.SpecularPower = 64f;
            this.FresnelFactor = 1f;
            this.IBLFactor = 1f;
            this.EnvironmentAmount = 1f;
            this.FresnelTermEnabled = false;
            this.EffectMode = DualTextureMode.Lightmap;
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

            this.LoadTexture(this.diffuse1Path, ref this.diffuse1);
            this.LoadTexture(this.diffuse2Path, ref this.diffuse2);
            this.LoadTexture(this.emissivePath, ref this.emissive);
            this.LoadTexture(this.normalPath, ref this.normal);
            this.LoadTexture(this.specularPath, ref this.specular);
            this.RefreshTexture(this.iblPath, ref this.ibl);
            this.RefreshTexture(this.environmentPath, ref this.environmentTexture);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Check if this material require the specified pass
        /// </summary>
        /// <param name="pass">The deferred pass.</param>
        /// <returns>True if this material require this pass.</returns>
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

            if (this.DeferredLightingPass == DeferredLightingPass.ForwardPass)
            {
                if (this.renderManager != null)
                {
                    Camera camera = this.renderManager.CurrentDrawingCamera;

                    this.shaderParameters.CameraPosition = camera.Position;
                    this.shaderParameters.TexCoordFix = this.renderManager.GraphicsDevice.RenderTargets.RenderTargetActive ? 1 : -1;

                    if (this.ibl != null)
                    {
                        this.graphicsDevice.SetTexture(this.ibl, 4);
                    }

                    if (this.environmentTexture != null)
                    {
                        this.graphicsDevice.SetTexture(this.environmentTexture, 5);
                    }

                    this.LightingTexture = camera.LightingRT;
                    if (this.LightingTexture != null)
                    {
                        this.graphicsDevice.SetTexture(this.LightingTexture, 6);
                        this.graphicsDevice.SetTexture(camera.GBufferRT0, 7);
                    }
                }

                this.shaderParameters.ReferenceAlpha = this.ReferenceAlpha;
                this.shaderParameters.DiffuseColor = this.diffuseColor;
                this.shaderParameters.Alpha = this.Alpha;
                this.shaderParameters.AmbientColor = this.ambientColor;
                this.shaderParameters.EmissiveColor = this.emissiveColor;
                this.shaderParameters.TextureOffset1 = this.TexcoordOffset1;
                this.shaderParameters.TextureOffset2 = this.TexcoordOffset2;
                this.shaderParameters.FresnelFactor = this.FresnelFactor;
                this.shaderParameters.IBLFactor = this.IBLFactor;
                this.shaderParameters.EnvironmentAmount = this.EnvironmentAmount;

                this.Parameters = this.shaderParameters;

                if (this.diffuse1 != null)
                {
                    this.graphicsDevice.SetTexture(this.diffuse1, 0);
                }

                if (this.diffuse2 != null)
                {
                    this.graphicsDevice.SetTexture(this.diffuse2, 1);
                }

                if (this.emissive != null)
                {
                    this.graphicsDevice.SetTexture(this.emissive, 2);
                }

                if (this.specular != null)
                {
                    this.graphicsDevice.SetTexture(this.specular, 3);
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
