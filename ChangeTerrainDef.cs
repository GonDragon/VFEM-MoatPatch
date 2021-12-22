using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;

namespace VFEM_MoatPatch
{
    public abstract class ChangeTerrainDef : Def
    {
        public TerrainDef terrain;
        public TerrainDef newTerrain;

        public float successChance = 1f;

        public List<ThingDef> resources;
        public List<int> stackSizes;

        public bool consumeResources = false;
        public bool spawnResources = false;
    }
}
