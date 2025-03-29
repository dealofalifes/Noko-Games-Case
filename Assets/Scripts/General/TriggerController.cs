using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    [SerializeField] private StackController _StackController;
    [SerializeField] private CharController _CharController;

    [Header("DEBUG")]
    [SerializeField] private bool _IsInteracting;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _interactable))
        {
            if (_interactable.IsCollectable())
            {
                StorageController interactedStorage = _interactable.GetCollectable().GetStorage();
                if (interactedStorage != null && !_IsInteracting)
                {
                    int[] focusIDs = _CharController.GetFocusIDs();
                    if (focusIDs.Length > 0 && !focusIDs.Contains(interactedStorage.GetProducedStackID()))
                        return;

                    _IsInteracting = true;
                    _StackController.CollectItemsStart(interactedStorage);
                }
            }
            else if (_interactable.IsDroppable())
            {
                IDroppable droppable = _interactable.GetDroppable();
                int index = droppable.GetInputIndex();

                InputController interactedInput = _interactable.GetDroppable().GetInputController();
                if (!_CharController.CanInteract(interactedInput))
                    return;

                if (interactedInput != null && !_IsInteracting)
                {
                    InputStat stat = interactedInput.GetStatByIndex(index);

                    _IsInteracting = true;
                    _StackController.ItemDropStart(interactedInput, stat);
                }
            }
            else if (_interactable.IsDestructible())
            {
                TrashController interactedTrash = _interactable.GetDestructible().GetTrashController();

                if (!_CharController.CanInteract(interactedTrash))
                    return;

                TrashStat stat = interactedTrash.GetStat();
                if (interactedTrash != null && !_IsInteracting)
                {
                    _IsInteracting = true;
                    _StackController.ItemDropStart(interactedTrash, stat);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IInteractable _interactable))
        {
            if (_IsInteracting)
            {
                _IsInteracting = false;
                _StackController.CollectItemsStop();
            }
        }
    }
}
