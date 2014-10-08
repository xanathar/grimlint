using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if USE_SLIMDX
using SlimDX.Direct3D9;
#endif

namespace GrimrockModelToolkit.Grim3d
{
    /*
     * defineMaterial{
	name = "goromorg_statue_base",
	diffuseMap = "assets/textures/env/marble_generic_dif.tga",
	specularMap = "assets/textures/env/marble_generic_spec.tga",
	normalMap = "assets/textures/env/marble_generic_normal.tga",
	doubleSided = false,
	lighting = true,
	alphaTest = false,
	blendMode = "Opaque",
	textureAddressMode = "Wrap",
	glossiness = 20,
	depthBias = 0,
        }
     */

    public class GrimMaterial
    {
        #region Properties
        private string mName;
        private string mDiffuseMap;
        private string mSpecularMap;
        private string mNormalMap;
        private bool mDoubleSided;
        private bool mLighting;
        private bool mAlphaTest;
        private string mBlendMode;
        private string mTextureAddressMode;
        private float mGlossiness;
        private float mDepthBias;
        private int mTextureRetainCount = 0;

#if USE_SLIMDX
        private Texture mDiffuseTexture;
        private Texture mSpecularTexture;
        private Texture mNormalTexture;
#endif
        #endregion

        #region Getters and Setters

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public string DiffuseMap
        {
            get { return mDiffuseMap; }
            set { mDiffuseMap = value; }
        }

        public string SpecularMap
        {
            get { return mSpecularMap; }
            set { mSpecularMap = value; }
        }

        public string NormalMap
        {
            get { return mNormalMap; }
            set { mNormalMap = value; }
        }

        public bool DoubleSided
        {
            get { return mDoubleSided; }
            set { mDoubleSided = value; }
        }

        public bool Lighting
        {
            get { return mLighting; }
            set { mLighting = value; }
        }

        public bool AlphaTest
        {
            get { return mAlphaTest; }
            set { mAlphaTest = value; }
        }

        public string BlendMode
        {
            get { return mBlendMode; }
            set { mBlendMode = value; }
        }

        public string TextureAddressMode
        {
            get { return mTextureAddressMode; }
            set { mTextureAddressMode = value; }
        }

        public float Glossiness
        {
            get { return mGlossiness; }
            set { mGlossiness = value; }
        }

        public float DepthBias
        {
            get { return mDepthBias; }
            set { mDepthBias = value; }
        }
		#if USE_SLIMDX
        public Texture DiffuseTexture
        {
            get { return mDiffuseTexture; }
            set 
            { 
                if ( mDiffuseTexture != null ) 
                {
                    mDiffuseTexture.Dispose();
                    mDiffuseTexture = null;
                }

                mDiffuseTexture = value; 
            }
        }

        public Texture SpecularTexture
        {
            get { return mSpecularTexture; }
            set
            {
                if (mSpecularTexture != null)
                {
                    mSpecularTexture.Dispose();
                    mSpecularTexture = null;
                }

                mSpecularTexture = value;
            }
        }

        public Texture NormalTexture
        {
            get { return mNormalTexture; }
            set
            {
                if (mNormalTexture != null)
                {
                    mNormalTexture.Dispose();
                    mNormalTexture = null;
                }

                mNormalTexture = value;
            }
        }
#endif

        #endregion

        ~GrimMaterial()
        {
            Dispose();
        }


        public void Dispose()
        {
            ReleaseTextures(true);
        }


        public int RetainTextures()
        {
            mTextureRetainCount++;
            return mTextureRetainCount;
        }


        public int ReleaseTextures(bool force = false)
        {
            mTextureRetainCount--;

            if (force || mTextureRetainCount <= 0)
            {
                mTextureRetainCount = 0;

#if USE_SLIMDX
                if (mDiffuseTexture != null)
                {
                    mDiffuseTexture.Dispose();
                    mDiffuseTexture = null;
                }

                if (mSpecularTexture != null)
                {
                    mSpecularTexture.Dispose();
                    mSpecularTexture = null;
                }

                if (mNormalTexture != null)
                {
                    mNormalTexture.Dispose();
                    mNormalTexture = null;
                }
#endif
            }

            return mTextureRetainCount;
        }


        public void SetProperty(string property, string value)
        {
            String lowerProperty = property.ToLower();

            if (lowerProperty == "name")
            {
                mName = value;
            }
            else if (lowerProperty == "diffusemap")
            {
                mDiffuseMap = value;
            }
            else if (lowerProperty == "specularmap")
            {
                mSpecularMap = value;
            }
            else if (lowerProperty == "normalmap")
            {
                mNormalMap = value;
            }
            else if (lowerProperty == "doublesided")
            {
                bool val = false;

                if (bool.TryParse(value, out val))
                {
                    mDoubleSided = val;
                }
            }
            else if (lowerProperty == "lighting")
            {
                bool val = false;

                if (bool.TryParse(value, out val))
                {
                    mLighting = val;
                }
            }
            else if (lowerProperty == "alphatest")
            {
                bool val = false;

                if (bool.TryParse(value, out val))
                {
                    mAlphaTest = val;
                }
            }
            else if (lowerProperty == "blendmode")
            {
                mBlendMode = value;
            }
            else if (lowerProperty == "textureaddressmode")
            {
                mTextureAddressMode = value;
            }
            else if (lowerProperty == "glossiness")
            {
                float val = 0.0f;

                if (float.TryParse(value, out val))
                {
                    mGlossiness = val;
                }
            }
            else if (lowerProperty == "depthbias")
            {
                float val = 0.0f;

                if (float.TryParse(value, out val))
                {
                    mDepthBias = val;
                }
            }
            else
            {
                Logger.Instance.Warning("GrimMaterial: Property '" + property + "' Invalid");
            }
        }
    }
}
