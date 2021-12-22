using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VFEMedieval;
using Verse;
using RimWorld;

namespace VFEM_MoatPatch
{
    [StaticConstructorOnStartup]
    public static class TerrainHandler
    {
        private static readonly Dictionary<TerrainDef, DigTerrainDef> digTerrainRelation;
        private static readonly Dictionary<TerrainDef, FillTerrainDef> fillTerrainRelation;

        public static readonly HashSet<string> HighestLevel;
        public static readonly HashSet<string> DeepestLevel;
        static TerrainHandler()
        {
            digTerrainRelation = new Dictionary<TerrainDef, DigTerrainDef>();
            fillTerrainRelation = new Dictionary<TerrainDef, FillTerrainDef>();
            HighestLevel = new HashSet<string>();
            DeepestLevel = new HashSet<string>();

            DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Clear();

            HashSet<TerrainDef> moatableTerrain = new HashSet<TerrainDef>();

            foreach (DigTerrainDef digTerrain in DefDatabase<DigTerrainDef>.AllDefs)
            {
                moatableTerrain.Add(digTerrain.terrain);
                moatableTerrain.Add(digTerrain.newTerrain);
                digTerrainRelation[digTerrain.terrain] = digTerrain;
            }

            foreach (FillTerrainDef fillTerrain in DefDatabase<FillTerrainDef>.AllDefs)
            {
                moatableTerrain.Add(fillTerrain.terrain);
                moatableTerrain.Add(fillTerrain.newTerrain);
                fillTerrainRelation[fillTerrain.terrain] = fillTerrain;
            }

            foreach(TerrainDef def in moatableTerrain)
            {
                if (!def.affordances.Contains(VFEM_DefOf.VFEM_Moatable))
                    def.affordances.Add(VFEM_DefOf.VFEM_Moatable);
                DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Add(def);
            }

            foreach(TerrainDef terrainDef in moatableTerrain)
            {
                if (!fillTerrainRelation.ContainsKey(terrainDef) && !terrainDef.IsWater) HighestLevel.Add(terrainDef.defName);
                if (!digTerrainRelation.ContainsKey(terrainDef)) DeepestLevel.Add(terrainDef.defName);
            }

        }
        public static TerrainDef HandleDig(TerrainDef def)
        {
            try
            {
                DigTerrainDef digTerrain = digTerrainRelation[def];
                if (Rand.Chance(digTerrain.successChance))
                    return digTerrain.newTerrain;
                return digTerrain.terrain;
            } catch
            {
                Mod.Error("No terrain found! Digging" + def?.defName);
                return (TerrainDef)null;
            }
        }

        public static TerrainDef HandleFill(TerrainDef def)
        {
            try
            {
                FillTerrainDef fillTerrain = fillTerrainRelation[def];
                if (Rand.Chance(fillTerrain.successChance))
                    return fillTerrain.newTerrain;
                return fillTerrain.terrain;
            }
            catch
            {
                Mod.Error("No terrain found! Filling" + def?.defName);
                return (TerrainDef)null;
            }
        }

        public static bool CheckIfHighest(TerrainDef def)
        {
            return HighestLevel.Contains(def.defName);
        }

        public static bool CheckIfDeepest(TerrainDef def)
        {
            return DeepestLevel.Contains(def.defName);
        }
    }
}
