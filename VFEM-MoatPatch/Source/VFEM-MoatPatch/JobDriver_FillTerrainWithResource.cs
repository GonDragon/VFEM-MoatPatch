using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using VFEMedieval;

namespace VFEM_MoatPatch
{
    class JobDriver_FillTerrainWithResource : JobDriver_FillTerrain
    {
        protected override void DoEffect(IntVec3 c)
        {
            Mod.Debug("JobDriver_FillTerrainWithResource.DoEffect");
            TerrainDef terrain = this.TargetLocA.GetTerrain(this.Map);
            TerrainDef newTerr = TerrainHandler.HandleFill(terrain);
            this.Map.terrainGrid.SetTerrain(this.TargetLocA, newTerr);
            FilthMaker.RemoveAllFilth(this.TargetLocA, this.Map);
        }
    }
}
