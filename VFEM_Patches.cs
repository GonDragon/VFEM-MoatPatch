using System;
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

    [HarmonyPatch(typeof(JobDriver_FillTerrain), "DoEffect")]
    internal class VFEM_JobDriver_FillTerrain_Patch
    {
        internal static MethodInfo HandleFill = AccessTools.Method(typeof(TerrainHandler), nameof(TerrainHandler.HandleFill));
        internal static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool patched = false;
            foreach (CodeInstruction code in instructions)
            {
                if (!found)
                {
                    if (code.opcode == OpCodes.Callvirt) // Here is where the Switch Case starts
                    {
                        Mod.Debug("Callvirt found on Fill");
                        found = true;
                    }
                    else
                    {
                        yield return code;
                    }
                }
                else if (!patched)
                {
                    if (code.opcode == OpCodes.Ldarg_0) //Insert a call to custom handler after the switch code. TerrainDef is already on the stack.
                    {
                        Mod.Debug("Patching on Ldarg_0 on Fill");
                        yield return new CodeInstruction(OpCodes.Call, HandleFill);
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        yield return code;
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

    [HarmonyPatch]
    internal class VFEM_Designator_FillTerrain_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("VFEMedieval.Designator_FillTerrain");
            return AccessTools.Method(type, "CanDesignateCell");
        }

        internal static MethodInfo CheckIfHighest = AccessTools.Method(typeof(TerrainHandler), nameof(TerrainHandler.CheckIfHighest));
        internal static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            bool patched = false;
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
                else
                {
                    yield return code;
                }
            }
        }
    }
}
