using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AleLuduMod.Components;

[RegisterInIl2Cpp]
public class MeetingHudBehaviour : MonoBehaviour
{
    public MeetingHudBehaviour(IntPtr ptr) : base(ptr)
    {
    }

    internal MeetingHud meetingHud = null!;

    [HideFromIl2Cpp]
    public IEnumerable<PlayerVoteArea> Targets => meetingHud.playerStates.OrderBy(p => p.AmDead);

    public void Start()
    {
        var i = 0;

        foreach (var button in Targets)
        {

                button.gameObject.SetActive(true);

                var relativeIndex = i;
                var row = relativeIndex / 4;
                var col = relativeIndex % 4;
                var buttonTransform = button.transform;
                buttonTransform.localScale *= 0.75f;
                buttonTransform.localPosition = meetingHud.VoteOrigin +
                                          new Vector3(
                                              meetingHud.VoteButtonOffsets.x * col * 0.75f - 0.35f,
                                              meetingHud.VoteButtonOffsets.y * row * 0.75f,
                                              buttonTransform.localPosition.z
                                          );
            i++;
        }
    }

}
