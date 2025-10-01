using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AleLuduMod.Components;

[RegisterInIl2Cpp]
public class ShapeshifterBehaviour : MonoBehaviour
{
    public ShapeshifterBehaviour(IntPtr ptr) : base(ptr){}
    internal ShapeshifterMinigame shapeshifterMinigame = null!;

    [HideFromIl2Cpp] 
    public IEnumerable<ShapeshifterPanel> Targets => shapeshifterMinigame.potentialVictims.ToArray();

    public void Start()
    {
        if (Targets.Count() < 16 && !AleLuduModPlugin.Force4Columns.Value) return; // dont change layout if players count is below 16
        var i = 0;
        foreach (var panel in Targets)
        {
            panel.gameObject.SetActive(true);
            var row = i / 4;
            var col = i % 4;
            var buttonTransform = panel.transform;
            buttonTransform.localScale *= 0.75f;
            buttonTransform.localPosition = new Vector3(
                                            shapeshifterMinigame.XStart + shapeshifterMinigame.XOffset * col * 0.75f - 0.375f,
                                            shapeshifterMinigame.YStart + shapeshifterMinigame.YOffset * row * 0.75f,
                                            buttonTransform.localPosition.z
                                            );
            i++;
        }
    }
}