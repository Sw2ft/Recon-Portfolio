using System;
using FMODUnity;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SO/Audio/Player", fileName = "Unit Sheet")]
public class FMODAudioData : ScriptableObject
{
    public enum SoundID
    {
        // Unit References
        SFXUnitPoweringUp,
        SFXUnitPoweringDown,

        // Unit Worker
        SFXUnitWorkerFootstep,
        SFXUnitWorkerThrowPunch,
        SFXUnitWorkerHitPunch,
        SFXUnitWorkerDropLoot,
        SFXUnitWorkerCollectLoot,
        SFXUnitWorkerIdle,
        SFXUnitWorkerBuilding,
        SFXUnitWorkerDeath,

        // Unit Recon
        SFXUnitReconFootstep,
        SFXUnitReconShot,
        SFXUnitReconCollectLoot,
        SFXUnitReconIdle,
        SFXUnitReconDeath,

        // Unit Fighter
        SFXUnitFighterMoving,
        SFXUnitFighterShooting,
        SFXUnitFighterOverheating,
        SFXUnitFighterMelee,
        SFXUnitFighterIdle,
        SFXUnitFighterDeath,

        // Enemy Screamer
        SFXEnemyScreamerVocalisations,
        SFXEnemyScreamerSteps,
        SFXEnemyScreamerAttack,
        SFXEnemyScreamerAttackImpact,
        SFXEnemyScreamerScream,
        SFXEnemyScreamerDeath,

        // UI
        SFXMenuUIHover,
        SFXMenuUIClick,
        SFXMenuUIDeselect,
        SFXMenuUIError,
        SFXDeployMenuUITooHeavy,

        // Ambience
        AMBBaseLevelBirdsong,
        AMBBaseLevelWind,
        AMBBaseLevelCritters,
        AMBBaseLevelRoars
    }

    
    [Serializable]
    public class NamedSFX
    {
        public string name;
        public SoundID eventID;
        public EventReference Event;
    }

    // this is used to reference Events for the sfx via code in the player
    [Header("SFX")]
    public List<NamedSFX> SFXList;

    public EventReference GetSFXByName(SoundID _eventID)
    {
        foreach (var sfx in SFXList)
        {
            if (sfx.eventID == _eventID)
            {
                return sfx.Event;
            }
        }
        Debug.LogWarning($"No SFX found with name {_eventID}!");
        return default;
    }
}