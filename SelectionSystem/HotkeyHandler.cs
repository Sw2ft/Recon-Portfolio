using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnitStateManager.UnitClass;

public class HotkeyHandler : MonoBehaviour
{
    #region References

    [SerializeField] private Transform cameraFollowTarget;
    [SerializeField] private Transform dropShipPosition;

    [SerializeField] private UnitSelectionManager selectionManager;

    private InputActions inputActions;

    // lists to cycle through
    private List<UnitStateManager> units = new List<UnitStateManager>();
    private List<MineralQuencher> quenchers = new List<MineralQuencher>();

    #endregion

    #region Variables

    // unit indices
    private int workerIndex = 0;
    private int reconIndex = 0;
    private int fighterIndex = 0;
    private int activeIndex = 0;

    // for quenchers
    private int quencherIndex = 0;

    // to start cycles from 0 / keep cycling
    private UnitStateManager.UnitClass lastShortcutUnit;

    // offset to counteract initial camera offset --> center current unit
    private Vector3 cameraOffset = new Vector3(4.53f, 0f, -4.65f);

    #endregion

    #region Unity Built-In

    private void Start()
    {
        inputActions = new InputActions();
        inputActions.Enable();

        // Assign callbacks to handle input
        inputActions.Hotkeys.One.performed += ctx => CycleActivation(true);
        inputActions.Hotkeys.Two.performed += ctx => CycleActivation(false);
        inputActions.Hotkeys.Three.performed += ctx => CycleUnitClasses(Worker);
        inputActions.Hotkeys.Four.performed += ctx => CycleUnitClasses(Recon);
        inputActions.Hotkeys.Five.performed += ctx => CycleUnitClasses(Fighter);
        inputActions.Hotkeys.Six.performed += ctx => CycleQuenchers();
        inputActions.Hotkeys.Seven.performed += ctx => CycleActiveUnits();
        inputActions.Hotkeys.Eight.performed += ctx => SelectAllUnitsOfType(Worker);
        inputActions.Hotkeys.Nine.performed += ctx => SelectAllUnitsOfType(Recon);
        inputActions.Hotkeys.Null.performed += ctx => SelectAllUnitsOfType(Fighter);
        inputActions.Hotkeys.B.performed += ctx => MoveCameraToDropship();
        inputActions.Hotkeys.C.performed += ctx => ResetCameraValues();
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        inputActions.Hotkeys.One.performed -= ctx => CycleActivation(true);
        inputActions.Hotkeys.Two.performed -= ctx => CycleActivation(false);
        inputActions.Hotkeys.Three.performed -= ctx => CycleUnitClasses(Worker);
        inputActions.Hotkeys.Four.performed -= ctx => CycleUnitClasses(Recon);
        inputActions.Hotkeys.Five.performed -= ctx => CycleUnitClasses(Fighter);
        inputActions.Hotkeys.Six.performed -= ctx => CycleQuenchers();
        inputActions.Hotkeys.Seven.performed -= ctx => CycleActiveUnits();
        inputActions.Hotkeys.B.performed -= ctx => MoveCameraToDropship();
        inputActions.Hotkeys.C.performed -= ctx => ResetCameraValues();

        inputActions.Disable();
    }

    #endregion

    #region Custom Functions()

    private void CycleActivation(bool activate)
    {
        if (activate)
        {
            selectionManager.ActivateSelected();
        }
        else
        {
            selectionManager.ShutDownSelected();
        }
    }

    private void CycleUnitClasses(UnitStateManager.UnitClass _type)
    {
        if (lastShortcutUnit != _type)
        {
            workerIndex = 0;
            reconIndex = 0;
            fighterIndex = 0;
        }

        foreach (UnitStateManager unit in FindObjectsByType<UnitStateManager>(FindObjectsSortMode.None))
        {
            if (unit.unitClass == _type)
            {
                units.Add(unit);
            }
        }

        int maxIndex = units.Count - 1;

        int unitIndex = 0;

        switch (_type)
        {
            case Worker:
                unitIndex = workerIndex;

                if (workerIndex < maxIndex)
                {
                    workerIndex++;
                }
                else
                {
                    workerIndex = 0;
                }

                break;

            case Recon:
                unitIndex = reconIndex;

                if (reconIndex < maxIndex)
                {
                    reconIndex++;
                }
                else
                {
                    reconIndex = 0;
                }

                break;

            case Fighter:
                unitIndex = fighterIndex;

                if (fighterIndex < maxIndex)
                {
                    fighterIndex++;
                }
                else
                {
                    fighterIndex = 0;
                }

                break;
        }

        for (int i = 0; i < units.Count; i++)
        {
            if (i == unitIndex)
            {
                Vector3 unitPosition = units[i].GetComponent<Transform>().position;

                Vector3 focusPosition = new Vector3(unitPosition.x, cameraFollowTarget.position.y, unitPosition.z) + cameraOffset;

                cameraFollowTarget.position = focusPosition;
            }
        }

        units.Clear();
        lastShortcutUnit = _type;
    }

    private void CycleQuenchers()
    {
        foreach (MineralQuencher quencher in FindObjectsByType<MineralQuencher>(FindObjectsSortMode.None))
        {
            quenchers.Add(quencher);
        }

        int maxIndex = quenchers.Count - 1;

        for (int i = 0; i < quenchers.Count; i++)
        {
            if (i == quencherIndex)
            {
                Vector3 quencherPosition = quenchers[i].GetComponent<Transform>().position;

                Vector3 focusPosition = new Vector3(quencherPosition.x, cameraFollowTarget.position.y, quencherPosition.z) + cameraOffset;

                cameraFollowTarget.position = focusPosition;
            }
        }

        if (quencherIndex < maxIndex)
        {
            quencherIndex++;
        }
        else
        {
            quencherIndex = 0;
        }

        quenchers.Clear();
    }

    private void CycleActiveUnits()
    {
        workerIndex = 0;
        reconIndex = 0;
        fighterIndex = 0;

        foreach (UnitStateManager unit in FindObjectsByType<UnitStateManager>(FindObjectsSortMode.None))
        {
            if (unit.currentState != unit.deactivatedState)
            {
                units.Add(unit);
            }
        }

        int maxIndex = units.Count - 1;

        for (int i = 0; i < units.Count; i++)
        {
            if (i == activeIndex)
            {
                Vector3 unitPosition = units[i].GetComponent<Transform>().position;

                Vector3 focusPosition = new Vector3(unitPosition.x, cameraFollowTarget.position.y, unitPosition.z) + cameraOffset;

                cameraFollowTarget.position = focusPosition;
            }
        }

        if (activeIndex < maxIndex)
        {
            activeIndex++;
        }
        else
        {
            activeIndex = 0;
        }

        units.Clear();
    }

    private void SelectAllUnitsOfType(UnitStateManager.UnitClass _type)
    {
        if (selectionManager.selectedUnits.Count > 0)
        {
            foreach (UnitStateManager unit in selectionManager.selectedUnits)
            {
                unit.Deselect();
            }
        }

        selectionManager.selectedUnits.Clear();

        foreach (UnitStateManager unit in FindObjectsByType<UnitStateManager>(FindObjectsSortMode.None))
        {
            if (unit.unitClass == _type)
            {
                selectionManager.selectedUnits.Add(unit);
                unit.Select();
            }
        }

        if (selectionManager.selectedUnits.Count > 0)
        {
            selectionManager.ToggleActivationButtons(true);
        }
        else
        {
            selectionManager.ToggleActivationButtons(false);
        }
        selectionManager.UpdateUnitPreview();
    }

    private void MoveCameraToDropship()
    {
        if (cameraFollowTarget != null)
        {
            Vector3 focusPosition = new Vector3(dropShipPosition.position.x, cameraFollowTarget.position.y, dropShipPosition.position.z) + cameraOffset;

            cameraFollowTarget.position = focusPosition;
        }
    }

    private void ResetCameraValues()
    {
        CameraScript cam = FindAnyObjectByType<CameraScript>();

        if(cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(0, -45, 0);
            cam.cinCam.Lens.FieldOfView = cam.defaultFOV;
        }
    }

    #endregion
}
