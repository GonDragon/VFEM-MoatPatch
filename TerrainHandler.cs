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
        static TerrainHandler()
        {
            DefDatabase<TerrainListDef>.GetNamed("VFEM_MoatableTerrain").terrainDefs.Clear();
            digTerrainRelation = new Dictionary<TerrainDef, DigTerrainDef>();

            foreach(DigTerrainDef digTerrain in DefDatabase<DigTerrainDef>.AllDefs)
            {
                Mod.Debug(string.Format("DefName: {0} - Converts {1} to {2}.",digTerrain.defName,digTerrain.terrain.defName,digTerrain.newTerrain.defName));
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
                Mod.Error("No terrain found!");
                return (TerrainDef)null;
            }
        }
    }
}
