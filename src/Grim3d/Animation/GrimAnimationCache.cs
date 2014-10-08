using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimAnimationCache
    {
        protected List<GrimAnimation> mLoadedAnimations = new List<GrimAnimation>();

        public List<GrimAnimation> LoadedAnimations
        {
            get { return mLoadedAnimations; }
        }

        public GrimAnimationCache()
        {

        }

        public int LoadAnimationsFromDirectory(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Logger.Instance.Warning("GrimAnimationCache: Attempted to load animations from '" + folderPath + "', but folder doesn't exist!");
                return 0;
            }

            string[] files = Directory.GetFiles(folderPath, "*.animation", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                Logger.Instance.Info("GrimAnimationCache: No animations found in '" + folderPath + "'");
                return 0;
            }

            int loadedAnims = 0;

            for (int i = 0; i < files.Length; i++)
            {
                if (LoadAnimation(files[i]))
                {
                    loadedAnims++;
                }
            }

            Logger.Instance.Info("GrimAnimationCache: Loaded " + loadedAnims + " Animations");
            return loadedAnims;
        }

        public bool LoadAnimation(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Logger.Instance.Warning("GrimAnimationCache: Attempted to load animation at '" + filePath + "', but no file exists.");
                return false;
            }

            GrimAnimation anim = GrimAnimation.LoadFromPath(filePath);

            if (anim == null)
            {
                Logger.Instance.Warning("GrimAnimationCache: Failed to load animation at '" + filePath + "'.");
                return false;
            }

            mLoadedAnimations.Add(anim);
            return true;
        }

        public void AddAnimation(GrimAnimation animation)
        {
            if (animation == null)
                return;

            mLoadedAnimations.Add(animation);
            return;
        }

        public void UnloadAllAnimations()
        {
            mLoadedAnimations.Clear();
        }
    }
}
