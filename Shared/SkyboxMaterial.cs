// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics.VertexFormats;
using System;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Materials
{
    /// <summary>
    /// Skybox effect material.
    /// </summary>
    public class SkyboxMaterial : Material
    {
        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Skybox", "SkyboxEffectvsSkybox", "SkyboxEffectpsSkybox", VertexPositionTexture.VertexFormat),
        };

        /// <summary>
        /// The environment map
        /// </summary>
        [DataMember]
        private string environmentMapPath;

        #region Properties

        /// <summary>
        /// Gets gets or sets the current technique.
        /// </summary>
        /// <value>
        /// The current technique.
        /// </value>
        public override string CurrentTechnique
        {
            get
            {
                return techniques[0].Name;
            }
        }

        /// <summary>
        /// Gets the environment texture.
        /// </summary>
        /// <value>
        /// The environment texture.
        /// </value>
        public TextureCube EnvironmentMap { get; private set; }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyboxMaterial" /> class.
        /// </summary>
        /// <param name="environmentMap">The environment map.</param>
        public SkyboxMaterial(string environmentMap)
            : this(environmentMap, DefaultLayers.Skybox)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyboxMaterial" /> class.
        /// </summary>
        /// <param name="environmentMap">The environment map.</param>
        public SkyboxMaterial(TextureCube environmentMap)
            : this(environmentMap, DefaultLayers.Skybox)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyboxMaterial" /> class.
        /// </summary>
        /// <param name="environmentMap">The environment map.</param>
        /// <param name="layer">The skybox associated layer</param>
        public SkyboxMaterial(string environmentMap, int layer)
            : base(layer)
        {
            this.environmentMapPath = environmentMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkyboxMaterial" /> class.
        /// </summary>
        /// <param name="environmentMap">The environment map.</param>
        /// <param name="layer">The skybox associated layer</param>
        public SkyboxMaterial(TextureCube environmentMap, int layer)
            : base(layer)
        {
        }

        /// <summary>
        /// Generate default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.InitializeTechniques(techniques);
        }

        /// <summary>
        /// Initializes the specified assets.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public override void Initialize(AssetsContainer assets)
        {
            base.Initialize(assets);

            if (this.EnvironmentMap == null && !string.IsNullOrEmpty(this.environmentMapPath))
            {
                this.EnvironmentMap = assets.LoadAsset<TextureCube>(this.environmentMapPath);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Unload this material.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public void Unload(AssetsContainer assets)
        {
            if (this.EnvironmentMap != null && !string.IsNullOrEmpty(this.EnvironmentMap.AssetPath))
            {
                assets.UnloadAsset(this.EnvironmentMap.AssetPath);
                this.EnvironmentMap = null;
            }
        }

        /// <summary>
        /// Applies the pass.
        /// </summary>
        /// <param name="cached">The efect is cached.</param>
        public override void SetParameters(bool cached)
        {
            base.SetParameters(cached);

            if (!cached)
            {
                if (this.EnvironmentMap != null)
                {
                    this.graphicsDevice.SetTexture(this.EnvironmentMap, 0);
                }
            }
        }

        #endregion
    }
}
