using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;

namespace DisableCameraPanning;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class DisableCameraPanning : BaseUnityPlugin
{
	private const string ModName = "Disable Camera Panning";
	private const string ModVersion = "1.0.0";
	private const string ModGUID = "org.bepinex.plugins.disablecamerapanning";

	public void Awake()
	{
		Harmony harmony = new(ModGUID);
		Assembly assembly = Assembly.GetExecutingAssembly();
		harmony.PatchAll(assembly);
	}

	[HarmonyPatch(typeof(Game), nameof(Game.UpdatePause))]
	public static class DisablePanning
	{
		private static readonly MethodInfo setLookDir = AccessTools.DeclaredMethod(typeof(Character), nameof(Character.SetLookDir));

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(setLookDir))
				{
					yield return new CodeInstruction(OpCodes.Pop);
					yield return new CodeInstruction(OpCodes.Pop);
					yield return new CodeInstruction(OpCodes.Pop);
				}
				else
				{
					yield return new CodeInstruction(instruction);
				}
			}
		}
	}
}
