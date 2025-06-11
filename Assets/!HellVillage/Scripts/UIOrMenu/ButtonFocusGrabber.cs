using System;
using System.Collections;
using HellVillage.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HellVillage.UIComponents {
    public class ButtonFocusGrabber : MonoBehaviour {
        [Tooltip("The button to grab focus from when this component is enabled. " +
                 "If not assigned, then find it from children of parent.")]
        [SerializeField] private Selectable _targetButton;
        [SerializeField] private bool _rememberLastFocus = true;
        [SerializeField] private bool _findRecursively = false;

        private void Awake() {
            if (_targetButton == null) _targetButton = GetFocusableButtonFromParentChildren(transform.parent);
            if (_targetButton == null) {
                Debug.LogWarning("No focusable button found in parent children. " +
                                 "Please assign a target button or ensure there are interactable buttons in the hierarchy.");
                return;
            }
        }

        private void OnEnable() {
            _ = StartCoroutine(GrabFocus());
            InputManager.UIAction.Navigate.performed += OnNavigate;

            if (_rememberLastFocus) ListenersToChildrenButtonInParent(transform.parent);
        }

        private void OnDisable() {
            InputManager.UIAction.Navigate.performed -= OnNavigate;

            if (_rememberLastFocus) ListenersToChildrenButtonInParent(transform.parent, true);
        }

        #region Private Methods

        private IEnumerator GrabFocus() {
            if (_targetButton != null && _targetButton.gameObject.activeInHierarchy) {
                EventSystem.current.SetSelectedGameObject(_targetButton.gameObject);
            } else {
                Debug.LogWarning("Target button is null or not active in hierarchy. Cannot grab focus.");
            }
            yield return null; // Wait for the next frame to ensure the focus is set
        }

        private Button GetFocusableButtonFromParentChildren(Transform parent) {
            foreach (Button button in parent.GetComponentsInChildren<Button>()) {
                if (button.interactable) {
                    if (EventSystem.current.currentSelectedGameObject != button.gameObject) {
                        return button;
                    }
                }

                if (_findRecursively) {
                    Button foundButton = GetFocusableButtonFromParentChildren(button.transform);
                    if (foundButton != null) {
                        return foundButton;
                    }
                }
            }

            return null;
        }

        private void ListenersToChildrenButtonInParent(Transform parent, bool removeInstead = false) {
            foreach (Selectable button in parent.GetComponentsInChildren<Button>(true)) {
                if (!button.TryGetComponent<EventTrigger>(out var trigger))
                    trigger = button.gameObject.AddComponent<EventTrigger>();

                EventTrigger.Entry selectEntry = new EventTrigger.Entry {
                    eventID = EventTriggerType.Select
                };

                if (removeInstead) {
                    selectEntry.callback.RemoveListener(OnButtonSelect);
                    trigger.triggers.Remove(selectEntry);
                } else {
                    selectEntry.callback.AddListener(OnButtonSelect);
                    trigger.triggers.Add(selectEntry);
                }
            }
        }

        private void OnButtonSelect(BaseEventData eventData) {
            if (_rememberLastFocus) {
                Selectable currentSelected = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
                if (currentSelected != null && currentSelected != _targetButton) {
                    _targetButton = currentSelected;
                }
            }
        }

        public void OnNavigate(UnityEngine.InputSystem.InputAction.CallbackContext context) {
            if (EventSystem.current.currentSelectedGameObject == null && _targetButton != null) {
                EventSystem.current.SetSelectedGameObject(_targetButton.gameObject);
            }
        }

        #endregion
    }
}
