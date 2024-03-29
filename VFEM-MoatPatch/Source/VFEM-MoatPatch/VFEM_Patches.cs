﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

using HarmonyLib;
using VFEMedieval;
using RimWorld;
using Verse;
using UnityEngine;

namespace VFEM_MoatPatch
{
    [HarmonyPatch(typeof(JobDriver_DigTerrain), "DoEffect")]
    internal class VFEM_JobDriver_DigTerrain_Patch
    {
        internal static MethodInfo HandleDig = AccessTools.Method(typeof(TerrainHandler), nameof(TerrainHandler.HandleDig));
        internal static MethodInfo GetMap = AccessTools.Property(typeof(JobDriver_DigTerrain), "Map").GetMethod;

        internal static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool patched = false;
            foreach (CodeInstruction code in instructions)
            {
                if (!found)
                {
                    if(code.opcode == OpCodes.Callvirt) // Here is where the Switch Case starts
                    {
                        Mod.Debug("Callvirt found on Dig");
                        found = true;
                    }
                    else
                    {
                        yield return code;
                    }                    
                }
                else if (!patched)
                {
                    if(code.opcode == OpCodes.Ldarg_0) //Insert a call to custom handler after the switch code. TerrainDef is already on the stack.
                    {
                        Mod.Debug("Patching on Ldarg_0 on Dig");
                        yield return new CodeInstruction(OpCodes.Ldarg_1); //IntVec3 c
                        yield return new CodeInstruction(OpCodes.Ldarg_0); //this ->
                        yield return new CodeInstruction(OpCodes.Call, GetMap); //this.map
                        yield return new CodeInstruction(OpCodes.Call, HandleDig); 
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        yield return code;
                        patched = true;
                    }                    
                } else
                {
                    yield return code;
                }
            }
        }
    }

    [HarmonyPatch]
    internal class VFEM_Designator_FillTerrain_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("VFEMedieval.Designator_FillTerrain");
            return AccessTools.Method(type, "CanDesignateCell");
        }

        internal static MethodInfo CheckIfHighest = AccessTools.Method(typeof(TerrainHandler), nameof(TerrainHandler.CheckIfHighest));
        internal static MethodInfo IsWater = AccessTools.Property(typeof(TerrainDef), "IsWater").GetMethod;
        internal static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool patched = false;

            bool found2 = false;
            bool patched2 = false;

            foreach (CodeInstruction code in instructions)
            {
                if (!found)
                {
                    if (code.operand == (object)"Soil") // Here is where the comparation with TerrainDef.Named("Soil") starts
                    {
                        Mod.Debug("Soil string found on Fill CanDesignateCell");
                        found = true;
                    }
                    else
                    {
                        yield return code;
                    }
                }
                else if (!patched)
                {
                    if (code.opcode == OpCodes.Ceq) //The comparation ends on Ceq. Replace the comparation with a custom boolean function.
                    {
                        Mod.Debug("Patching after the Ceq on Fill CanDesignateCell");
                        yield return new CodeInstruction(OpCodes.Call, CheckIfHighest);
                        patched = true;
                    }
                }
                else if (!found2)
                {
                    if (code.operand == (object)"WaterShallow") // Here is where the comparation with TerrainDef.Named("WaterShallow") starts
                    {
                        Mod.Debug("WaterShallow string found on Fill CanDesignateCell");
                        found2 = true;
                    }
                    else
                    {
                        yield return code;
                    }
                }
                else if (!patched2)
                {
                    if (code.opcode == OpCodes.Ceq) //The comparation ends on Ceq. Replace two comparations with a IsWater TerrainDef call.
                    {
                        Mod.Debug("Patching after the Ceq on Fill CanDesignateCell for water check");
                        yield return new CodeInstruction(OpCodes.Callvirt, IsWater);
                        patched2 = true;
                    }
                }
                else
                {
                    yield return code;
                }
            }
        }
    }

    [HarmonyPatch]
    internal class VFEM_Designator_DigTerrain_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("VFEMedieval.Designator_DigTerrain");
            return AccessTools.Method(type, "CanDesignateCell");
        }

        internal static MethodInfo CheckIfDeepest = AccessTools.Method(typeof(TerrainHandler), nameof(TerrainHandler.CheckIfDeepest));
        internal static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool patched = false;

            foreach (CodeInstruction code in instructions)
            {
                if (!found)
                {
                    if (code.operand == (object)"WaterShallow") // Here is where the comparation with TerrainDef.Named("WaterShallow") starts
                    {
                        Mod.Debug("WaterShallow string found on Dig CanDesignateCell");
                        found = true;
                    }
                    else
                    {
                        yield return code;
                    }
                }
                else if (!patched)
                {
                    if (code.opcode == OpCodes.Ceq) //The comparation ends on Ceq. Replace the comparation with a custom boolean function.
                    {
                        Mod.Debug("Patching after the Ceq on Dig CanDesignateCell");
                        yield return new CodeInstruction(OpCodes.Call, CheckIfDeepest);
                        patched = true;
                    }
                }
                else
                {
                    yield return code;
                }
            }
        }
    }
}
