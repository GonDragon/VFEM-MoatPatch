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

            foreach (DigTerrainDef digTerrain in DefDatabase<DigTerrainDef>.AllDefs)
            {
                digTerrainRelation[digTerrain.terrain] = digTerrain;
                if (!digTerrain.terrain.affordances.Contains(VFEM_DefOf.VFEM_Moatable))
                {
                    digTerrain.terrain.affordances.Add(VFEM_DefOf.VFEM_Moatable);
                }

                if (!DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Contains(digTerrain.terrain))
                {
                    DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Add(digTerrain.terrain);
                }

            }

            foreach (FillTerrainDef fillTerrain in DefDatabase<FillTerrainDef>.AllDefs)
            {
                fillTerrainRelation[fillTerrain.terrain] = fillTerrain;
                if (!fillTerrain.terrain.affordances.Contains(VFEM_DefOf.VFEM_Moatable))
                {
                    fillTerrain.terrain.affordances.Add(VFEM_DefOf.VFEM_Moatable);
                }

                if (!DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Contains(fillTerrain.terrain))
                {
                    DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Add(fillTerrain.terrain);
                }

            }

            foreach(TerrainDef terrainDef in digTerrainRelation.Keys)
            {
                if (!fillTerrainRelation.ContainsKey(terrainDef)) HighestLevel.Add(terrainDef.defName);
            }

            foreach (TerrainDef terrainDef in fillTerrainRelation.Keys)
            {
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
