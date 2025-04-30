using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitSelectionManager : MonoBehaviour
{

    #region References

    public Camera mainCamera;
    public RectTransform selectionBox;
    
    private InputActions inputActions;

    [SerializeField] private GameObject activationButtons;
    [SerializeField] private GameObject clickVFXGO;

    [SerializeField] private TextMeshProUGUI workerLabel;
    [SerializeField] private TextMeshProUGUI quencherLabel;
    [SerializeField] private TextMeshProUGUI reconLabel;
    [SerializeField] private TextMeshProUGUI fighterLabel;

    [SerializeField] private GameObject workerIcon;
    [SerializeField] private GameObject quencherIcon;
    [SerializeField] private GameObject reconIcon;
    [SerializeField] private GameObject fighterIcon;

    [SerializeField] private GameObject DropMineralQuencherButton;

    private int workerCount;
    private int quencherCount;
    private int reconCount;
    private int fighterCount;

    public List<UnitStateManager> selectedUnits = new List<UnitStateManager>();
    private List<UnitStateManager> lastSelectedUnits = new List<UnitStateManager>();

    #endregion

    #region Variables

    public LayerMask unitLayer;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool keepSelected;
    private bool isDragging = false;

    #endregion


    #region Unity Build In

    private void Start()
    {
        inputActions = new InputActions();
        inputActions.Mouse.Enable();

        DisableAllIcons();

        selectionBox.gameObject.SetActive(false); // Hide the selection box initially
    }

    private void Update()
    {
        HandleSelection();
        HandleCommands();
    }

    #region Buttons

    public void OnHoverEnterButtons()
    {
        keepSelected = true;
    }

    public void OnHoverExitButtons()
    {
        keepSelected = false;
    }

    public void SelectAllWorkers()
    {
        SelectAllUnitsOfType(UnitStateManager.UnitClass.Worker);
    }

    public void SelectAllQuenchers()
    {
        foreach (var unit in selectedUnits)
        {
            unit.Deselect();
        }

        lastSelectedUnits = selectedUnits;
        selectedUnits.Clear();

        UnitStateManager[] units = FindObjectsByType<UnitStateManager>(FindObjectsSortMode.None);

        foreach (UnitStateManager unit in units)
        {
            if (unit.unitClass == UnitStateManager.UnitClass.Worker)
            {
                if (unit.holdsMineralQuencher)
                {
                    unit.Select();
                    selectedUnits.Add(unit);
                }
            }
        }

        if (selectedUnits.Count > 0)
        {
            activationButtons.SetActive(true);
        }

        UpdateUnitPreview();
    }

    public void SelectAllRecons()
    {
        SelectAllUnitsOfType(UnitStateManager.UnitClass.Recon);
    }

    public void SelectAllFighters()
    {
        SelectAllUnitsOfType(UnitStateManager.UnitClass.Fighter);
    }

    #endregion

    #endregion

    

    #region Custom Functions()

    private void HandleSelection()
    {
        if (keepSelected && !isDragging) // if we hover a button --> we quit the function early
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            selectionBox.sizeDelta = Vector2.zero;
            startPosition = Mouse.current.position.ReadValue();
            selectionBox.gameObject.SetActive(true);
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            endPosition = Mouse.current.position.ReadValue();
            UpdateSelectionBox();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (Vector2.Distance(startPosition, Mouse.current.position.ReadValue()) < 5f) // Detects a click
            {
                SelectSingleUnit();
            }
            else
            {
                SelectUnitsInBox();
            }

            UpdateUnitPreview();
            selectionBox.gameObject.SetActive(false);
            isDragging = false;
        }
    }

    private void UpdateSelectionBox()
    {
        if (isDragging == false)
        {
            isDragging = true;
        }
        
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxSize = boxEnd - boxStart;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y));
        selectionBox.anchoredPosition = boxStart + boxSize / 2;
    }

    private void SelectSingleUnit()
    {
        /// <summary>
        /// This function is called after the HandleSelection Function detects a singular mouse click.
        /// 
        /// First we check if the ray hit a unit, if yes, we continue and reset the selectedUnits-List, adding
        /// the new single unit. If we press shift while clicking, we add another unit to the List, same goes
        /// for removing the unit by clicking on it again.
        /// 
        /// The last else-statement handles a click into emptyspace, if not holding shift, every unit is going
        /// to be deselected.
        /// </summary>

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer))
        {
            UnitStateManager unit = hit.collider.GetComponent<UnitStateManager>();
            if (unit != null)
            {
                if (Keyboard.current.shiftKey.isPressed) // Add or remove from selection if Shift is held
                {
                    if (!selectedUnits.Contains(unit))
                    {
                        unit.Select();
                        selectedUnits.Add(unit);
                    }
                    else
                    {
                        unit.Deselect();
                        selectedUnits.Remove(unit);
                    }
                }
                else // Normal single selection
                {
                    // Deselect previously selected units
                    foreach (var selectedUnit in selectedUnits)
                    {
                        selectedUnit.Deselect();
                    }

                    lastSelectedUnits = selectedUnits;
                    selectedUnits.Clear();

                    DisableAllIcons();

                    // Select the clicked unit
                    unit.Select();
                    selectedUnits.Add(unit);
                }

                if (selectedUnits.Count > 0)
                {
                    activationButtons.SetActive(true);
                    DisplaySelectedIcons();
                }
            }
        }
        else
        {
            // Clicked on empty space, deselect all units if Shift is not held
            if (!Keyboard.current.shiftKey.isPressed)
            {
                foreach (var selectedUnit in selectedUnits)
                {
                    selectedUnit.Deselect();
                }

                selectedUnits.Clear();
                activationButtons.SetActive(false);

                DisableAllIcons();
            }
        }
    }

    private void SelectUnitsInBox()
    {
        Vector2 min = startPosition;
        Vector2 max = endPosition;

        if (min.x > max.x) (min.x, max.x) = (max.x, min.x);
        if (min.y > max.y) (min.y, max.y) = (max.y, min.y);

        foreach (var unit in selectedUnits)
        {
            unit.Deselect();
        }

        lastSelectedUnits = selectedUnits;
        selectedUnits.Clear();

        foreach (UnitStateManager unit in FindObjectsByType<UnitStateManager>(FindObjectsSortMode.None))
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x >= min.x && screenPosition.x <= max.x &&
                screenPosition.y >= min.y && screenPosition.y <= max.y)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }

        if (selectedUnits.Count > 0)
        {
            activationButtons.SetActive(true);
            DisplaySelectedIcons();
        }
    }

    private void HandleCommands()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedUnits.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 targetPosition = hit.point;             


                MoveUnitsToTargetWithSpacing(targetPosition);
                Instantiate(clickVFXGO, targetPosition, Quaternion.identity);
            }
        }
    }

    private void SelectAllUnitsOfType(UnitStateManager.UnitClass _type)
    {
        List<UnitStateManager> unitsOfType = new();

        foreach (var unit in selectedUnits)
        {
            if (unit.unitClass == _type)
            {
                unitsOfType.Add(unit);
            }
            else
            {
                unit.Deselect();
            }
        }

        selectedUnits = unitsOfType;

        if (selectedUnits.Count > 0)
        {
            activationButtons.SetActive(true);
        }

        UpdateUnitPreview();
    }

    private void DisplaySelectedIcons()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit.unitClass == UnitStateManager.UnitClass.Worker)
            {
                if (!workerIcon.activeInHierarchy)
                {
                    workerIcon.SetActive(true);
                }

                if (unit.holdsMineralQuencher)
                {
                    if (!quencherIcon.activeInHierarchy)
                    {
                        quencherIcon.SetActive(true);
                    }
                }
                
            }
            else if (unit.unitClass == UnitStateManager.UnitClass.Recon)
            {
                if (!reconIcon.activeInHierarchy)
                {
                    reconIcon.SetActive(true);
                }
            }
            else if (unit.unitClass == UnitStateManager.UnitClass.Fighter)
            {
                if (!fighterIcon.activeInHierarchy)
                {
                    fighterIcon.SetActive(true);
                }
            }
        }
    }

    private void DisableAllIcons()
    {
        workerIcon.SetActive(false);
        quencherIcon.SetActive(false);
        reconIcon.SetActive(false);
        fighterIcon.SetActive(false);
    }

    private void MoveUnitsToTargetWithSpacing(Vector3 targetPosition)
    {
        float spacing = 1f;

        foreach (UnitStateManager unit in selectedUnits)
        {
            Vector3 offset = new Vector3(Random.Range(-spacing, spacing), 0, Random.Range(-spacing, spacing));
            Vector3 adjustedTarget = targetPosition + offset;

            // snap to navmesh
            if (NavMesh.SamplePosition(adjustedTarget, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
            {
                adjustedTarget = navHit.position;
            }

            unit.OnCommandMove(adjustedTarget);
        }
    }

    public void UpdateUnitPreview()
    {
        if (selectedUnits.Count > 0)
        {
            foreach (UnitStateManager unit in selectedUnits)
            {
                if (unit.unitClass == UnitStateManager.UnitClass.Worker)
                {
                    if (unit.holdsMineralQuencher)
                    {
                        quencherCount++;
                    }

                    workerCount++;
                }
                else if (unit.unitClass == UnitStateManager.UnitClass.Recon)
                {
                    reconCount++;
                }
                else if(unit.unitClass== UnitStateManager.UnitClass.Fighter)
                {
                    fighterCount++;
                }
            }

            workerLabel.text = workerCount.ToString();
            quencherLabel.text = quencherCount.ToString();
            reconLabel.text = reconCount.ToString();
            fighterLabel.text = fighterCount.ToString();

        }
        else if (selectedUnits.Count == 0)
        {
            DisableAllIcons();
            workerLabel.text = 0.ToString();
            quencherLabel.text= 0.ToString();
            reconLabel.text = 0.ToString();
            fighterLabel.text = 0.ToString();
        }

        if (quencherCount > 0)
        {
            foreach (UnitStateManager unit in selectedUnits)
            {
                if (unit.unitClass == UnitStateManager.UnitClass.Worker)
                {
                    if (unit.currentState != unit.deactivatedState)
                    {
                        if (unit.holdsMineralQuencher)
                        {
                            DropMineralQuencherButton.SetActive(true);
                        }
                    }
                    else
                    {
                        DropMineralQuencherButton.SetActive(false);
                    }
                }
            }
        }
        else
        {
            DropMineralQuencherButton.SetActive(false);
        }

        workerCount = 0;
        quencherCount = 0;
        reconCount = 0;
        fighterCount = 0;

        DisableAllIcons();
        DisplaySelectedIcons();
    }

    public void RemoveDestroyedUnits(UnitStateManager _unit)
    {
        if (lastSelectedUnits.Contains(_unit))
        {
            lastSelectedUnits.Remove(_unit);
        }
        if (selectedUnits.Contains(_unit))
        {
            selectedUnits.Remove(_unit);
        }
    }

    public void ToggleActivationButtons(bool setActive)
    {
        activationButtons.SetActive(setActive);
    }

    public void ShutDownSelected()
    {
        if (selectedUnits.Count > 0)
        {
            foreach (UnitStateManager unit in selectedUnits)
            {
                unit.EnergyLogic("Deactivate");
            }

            UpdateUnitPreview();
        }
    }

    public void ActivateSelected()
    {
        if (selectedUnits.Count > 0)
        {
            foreach (UnitStateManager unit in selectedUnits)
            {
                unit.EnergyLogic("Reactivate");
            }

            UpdateUnitPreview();
        }
    }

    #endregion
}
