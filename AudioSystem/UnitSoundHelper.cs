using UnityEngine;
using static FMODUnity.RuntimeManager;
using static FMODAudioData.SoundID;
using FMOD;

public class UnitSoundHelper
{
    #region References

    private UnitStateManager unit;
    private UnitStateManager.UnitClass unitClass;
    private FMODAudioData audioSheet;

    private FMOD.Studio.EventInstance shooting;
    private FMOD.Studio.EventInstance moving;
    private FMOD.Studio.EventInstance pickUp;
    private FMOD.Studio.EventInstance drop;
    private FMOD.Studio.EventInstance deactivate;
    private FMOD.Studio.EventInstance activate;
    private FMOD.Studio.EventInstance dying;

    #endregion

    #region General Functionality
    public enum SoundType
    {
        SHOOTING,
        MOVING,
        PICKUP,
        DROP,
        DEACTIVATE,
        ACTIVATE,
        DEATH
    }

    public void Initialize(UnitStateManager _unit, UnitStateManager.UnitClass _unitClass, FMODAudioData _audioSheet)
    {
        unit = _unit;
        unitClass = _unitClass;
        audioSheet = _audioSheet;
    }

    public void PlaySoundByType(SoundType _type)
    {
        switch (_type)
        {
            case SoundType.SHOOTING:
                PlayShooting();
                break;

            case SoundType.MOVING:
                PlayMoving();
                break;

            case SoundType.PICKUP:
                PlayPickUp();
                break;

            case SoundType.DROP:
                PlayDrop();
                break;

            case SoundType.DEACTIVATE:
                PlayDeactivate();
                break;

            case SoundType.ACTIVATE:
                PlayActivate();
                break;

            case SoundType.DEATH:
                PlayDeath();
                break;
        }
    }

    public void StopSoundByType(SoundType _type)
    {
        switch (_type)
        {
            case SoundType.SHOOTING:
                StopShooting();
                break;
        }
    }

    #endregion

    #region Custom Functions

    private void PlayShooting()
    {
        if (unitClass == UnitStateManager.UnitClass.Fighter)
        {
            if (!IsPlaying(shooting))
            {
                shooting = CreateInstance(audioSheet.GetSFXByName(SFXUnitFighterShooting));

                shooting.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
                shooting.setParameterByName("Firing", 0);
                shooting.start();
                shooting.release();
            }
        }

        else if (unitClass == UnitStateManager.UnitClass.Recon)
        {
            shooting = CreateInstance(audioSheet.GetSFXByName(SFXUnitReconShot));

            shooting.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            shooting.start();
            shooting.release();
        }

        else if (unitClass == UnitStateManager.UnitClass.Worker)
        {
            shooting = CreateInstance(audioSheet.GetSFXByName(SFXUnitWorkerThrowPunch));

            shooting.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            shooting.start();
            shooting.release();
        }
    }

    private void StopShooting()
    {
        if (unitClass == UnitStateManager.UnitClass.Fighter)
        {
            shooting.setParameterByName("Firing", 1);
            shooting.release();
        }
    }

    private void PlayMoving()
    {
        if (unitClass == UnitStateManager.UnitClass.Fighter)
        {
            if (!IsPlaying(moving))
            {
                moving = CreateInstance(audioSheet.GetSFXByName(SFXUnitFighterMoving));

                moving.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
                moving.setParameterByName("Moving", 0);
                moving.start();
                moving.release();
            }
        }

        else if (unitClass == UnitStateManager.UnitClass.Recon)
        {
            moving = CreateInstance(audioSheet.GetSFXByName(SFXUnitReconFootstep));

            moving.setParameterByName("Pitch", Random.Range(0.9f, 1.1f));
            moving.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            moving.start();
            moving.release();
        }

        else if (unitClass == UnitStateManager.UnitClass.Worker) 
        {
            moving = CreateInstance(audioSheet.GetSFXByName(SFXUnitWorkerFootstep));

            moving.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            moving.start();
            moving.release();
        }

        
    }

    private void PlayPickUp()
    {
        if (unitClass == UnitStateManager.UnitClass.Worker)
        {
            pickUp = CreateInstance(audioSheet.GetSFXByName(SFXUnitWorkerCollectLoot));

            pickUp.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            pickUp.start();
            pickUp.release();
        }
    }

    private void PlayDrop()
    {
        if (unitClass == UnitStateManager.UnitClass.Worker)
        {
            drop = CreateInstance(audioSheet.GetSFXByName(SFXUnitWorkerDropLoot));

            drop.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            drop.start();
            drop.release();
        }
    }

    private void PlayDeactivate()
    {
        deactivate = CreateInstance(audioSheet.GetSFXByName(SFXUnitPoweringDown));

        deactivate.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        deactivate.start();
        deactivate.release();
    }

    private void PlayActivate()
    {
        activate = CreateInstance(audioSheet.GetSFXByName(SFXUnitPoweringUp));

        activate.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
        activate.start();
        activate.release();
    }

    private void PlayDeath()
    {
        if (unitClass == UnitStateManager.UnitClass.Fighter)
        {
            dying = CreateInstance(audioSheet.GetSFXByName(SFXUnitFighterDeath));

            dying.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            dying.start();
            dying.release();
        }

        else if (unitClass == UnitStateManager.UnitClass.Recon)
        {
            dying = CreateInstance(audioSheet.GetSFXByName(SFXUnitReconDeath));

            dying.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            dying.start();
            dying.release();
        }

        else if (unitClass == UnitStateManager.UnitClass.Worker)
        {
            if (audioSheet == null)
            {
                UnityEngine.Debug.Log("Warum bist du null?!?!?!?!?");
            }

            dying = CreateInstance(audioSheet.GetSFXByName(SFXUnitWorkerDeath));

            dying.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(unit.gameObject));
            dying.start();
            dying.release();
        }
    }

    private bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        instance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    #endregion
}
