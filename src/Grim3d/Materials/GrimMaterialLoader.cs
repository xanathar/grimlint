using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#if USE_SLIMDX
using SlimDX.Direct3D9;
#endif

namespace GrimrockModelToolkit.Grim3d
{
    public class GrimMaterialLoader
    {
        private String mAssetPackPath;
        private String mDungeonPath;

        public String AssetPackPath
        {
            get { return mAssetPackPath; }
            set { mAssetPackPath = value; }
        }

        public String DungeonPath
        {
            get { return mDungeonPath; }
            set { mDungeonPath = value; }
        }

        public List<GrimMaterial> LoadMaterials()
        {
            List<GrimMaterial> grimMaterials = new List<GrimMaterial>();

            if (!String.IsNullOrWhiteSpace(mAssetPackPath) && Directory.Exists(mAssetPackPath))
            {
                grimMaterials.AddRange(ParseLuaFile(Path.Combine(mAssetPackPath, "scripts\\materials.lua")));
            }

            if (!String.IsNullOrWhiteSpace(mDungeonPath) && Directory.Exists(mDungeonPath))
            {
                grimMaterials.AddRange(LoadMaterialsFromDir(mDungeonPath));
            }

            Logger.Instance.Info("GrimMaterialLoader: Loaded " + grimMaterials.Count + " Material Definitions.");
            return grimMaterials;
        }

        public List<GrimMaterial> LoadMaterialsFromDir(String path)
        {
            List<GrimMaterial> grimMaterials = new List<GrimMaterial>();

            foreach( String filePath in Directory.EnumerateFiles(path, "*.lua", SearchOption.AllDirectories) )
            {
                grimMaterials.AddRange(ParseLuaFile(filePath));
            }

            return grimMaterials;
        }

        public List<GrimMaterial> ParseLuaFile(String path)
        {
            List<GrimMaterial> grimMaterials = new List<GrimMaterial>();

            if (!File.Exists(path))
            {
                Logger.Instance.Warning("Material Loader: '" + path + "' does not exist.");
                return grimMaterials;
            }

            string[] lines = File.ReadAllLines(path);
            GrimMaterial currentMaterial = null;
            bool inMaterial = false;

            foreach (String line in lines)
            {
                if (line.Contains("defineMaterial"))
                {
                    if (inMaterial)
                    {
                        Logger.Instance.Warning("Material Loader: Found a defineMaterial command inside another. Cannot process.");
                        continue;
                    }

                    currentMaterial = new GrimMaterial();
                }

                if (line.Contains("{"))
                {
                    if (currentMaterial != null)
                    {
                        inMaterial = true;
                    }
                }

                if (inMaterial && line.Contains("="))
                {
                    string[] components = line.Split('=');

                    if (components.Length == 2)
                    {
                        components[0] = components[0].Replace(",", "").Replace("\"", "").Trim();
                        components[1] = components[1].Replace(",", "").Replace("\"", "").Trim();
                        currentMaterial.SetProperty(components[0], components[1]);
                    }
                }

                if (inMaterial && line.Contains("}"))
                {
                    inMaterial = false;
                    grimMaterials.Add(currentMaterial);
                    currentMaterial = null;
                }
            }

            Logger.Instance.Info("Loaded " + grimMaterials.Count + " Materials from '" + path + "'");
            return grimMaterials;
        }

#if USE_SLIMDX
        public void LoadTexturesForMaterial(Device device, GrimMaterial material)
        {
            // Only attempt to load the texture if we don't have one already and we have a map parameter
            if (material.DiffuseTexture == null && !String.IsNullOrWhiteSpace(material.DiffuseMap))
            {
                string diffusePath = material.DiffuseMap.Replace(".tga", ".dds").Replace("/", "\\");
                string diffuseModPath = Path.Combine(mDungeonPath, diffusePath);
                string diffuseAssetPath = Path.Combine(mAssetPackPath, diffusePath).Replace("assets\\assets\\", "assets\\");

                if (File.Exists(diffuseModPath))
                {
                    material.DiffuseTexture = Texture.FromFile(device, diffuseModPath, Usage.None, Pool.Managed);
                }
                else if (File.Exists(diffuseAssetPath))
                {
                    material.DiffuseTexture = Texture.FromFile(device, diffuseAssetPath, Usage.None, Pool.Managed);
                }
            }

            if (material.SpecularTexture == null && !String.IsNullOrWhiteSpace(material.SpecularMap))
            {
                string specularPath = material.SpecularMap.Replace(".tga", ".dds").Replace("/", "\\");
                string specularModPath = Path.Combine(mDungeonPath, specularPath);
                string specularAssetPath = Path.Combine(mAssetPackPath, specularPath).Replace("assets\\assets\\", "assets\\");

                if (File.Exists(specularModPath))
                {
                    material.SpecularTexture = Texture.FromFile(device, specularModPath, Usage.None, Pool.Managed);
                }
                else if (File.Exists(specularAssetPath))
                {
                    material.SpecularTexture = Texture.FromFile(device, specularAssetPath, Usage.None, Pool.Managed);
                }
            }

            if (material.NormalTexture == null && !String.IsNullOrWhiteSpace(material.NormalMap))            
            {
                string normalPath = material.NormalMap.Replace(".tga", ".dds").Replace("/", "\\");
                string normalModPath = Path.Combine(mDungeonPath, normalPath);
                string normalAssetPath = Path.Combine(mAssetPackPath, normalPath).Replace("assets\\assets\\", "assets\\");

                if (File.Exists(normalModPath))
                {
                    material.NormalTexture = Texture.FromFile(device, normalModPath, Usage.None, Pool.Managed);
                }
                else if (File.Exists(normalAssetPath))
                {
                    material.NormalTexture = Texture.FromFile(device, normalAssetPath, Usage.None, Pool.Managed);
                }
            }

            material.RetainTextures();
        }
#endif
    }

}
