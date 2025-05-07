using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace HellVillage {
    public class MenuEventSystemHandler : MonoBehaviour {
        [Header("References")]
        public List<Selectable> Selectables = new List<Selectable>();
        [SerializeField] protected Selectable _firstSelected;

        [Header("Controls")]
        [SerializeField] protected InputActionReference _navigateReference;

        [Header("Animations")]
        [SerializeField] protected float _selectedAnimationScale = 1.1f;
        [SerializeField] protected float _scaleDuration = 0.25f;
        [SerializeField] protected List<GameObject> _animationExclusions = new List<GameObject>();
        [SerializeField] protected bool _textChangeOnSelect = false;

        [Header("Sounds")]
        [SerializeField] protected UnityEvent SoundEvent;

        protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

        protected Selectable _lastSelected;

        protected Tween _scaleUpTween;
        protected Tween _scaleDownTween;

        public virtual void Awake() {
            foreach (var selectable in Selectables) {
                AddSelectionListeners(selectable);
                _scales.Add(selectable, selectable.transform.localScale);
            }

            if (_textChangeOnSelect) {
                foreach (var selectable in Selectables) {
                    selectable.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                }
            }
        }

        public virtual void OnEnable() {
            _navigateReference.action.performed += OnNavigate;

            //ensure all selectables are reset back to original size
            for (int i = 0; i < Selectables.Count; i++) {
                Selectables[i].transform.localScale = _scales[Selectables[i]];
            }
            if (_textChangeOnSelect) {
                for (int i = 0; i < Selectables.Count; i++) {
                    Selectables[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                }
            }

            StartCoroutine(SelectAfterDelay());
        }

        protected virtual IEnumerator SelectAfterDelay() {
            yield return null;
            EventSystem.current.SetSelectedGameObject(_firstSelected.gameObject);
        }

        public virtual void OnDisable() {
            _navigateReference.action.performed -= OnNavigate;

            _scaleUpTween.Kill(true);
            _scaleDownTween.Kill(true);
        }

        protected virtual void AddSelectionListeners(Selectable selectable) {
            //add listener
            EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (trigger == null) {
                trigger = selectable.gameObject.AddComponent<EventTrigger>();
            }

            //add SELECT event
            EventTrigger.Entry SelectEntry = new EventTrigger.Entry {
                eventID = EventTriggerType.Select
            };
            SelectEntry.callback.AddListener(OnSelect);
            trigger.triggers.Add(SelectEntry);

            //add DESELECT event
            EventTrigger.Entry DeselectEntry = new EventTrigger.Entry {
                eventID = EventTriggerType.Deselect
            };
            DeselectEntry.callback.AddListener(OnDeselect);
            trigger.triggers.Add(DeselectEntry);

            //add ONPOINTERENTER event
            EventTrigger.Entry PointerEnter = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerEnter
            };
            PointerEnter.callback.AddListener(OnPointerEnter);
            trigger.triggers.Add(PointerEnter);

            //add ONPOINTEREXIT event
            EventTrigger.Entry PointerExit = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerExit
            };
            PointerExit.callback.AddListener(OnPointerExit);
            trigger.triggers.Add(PointerExit);

        }

        public void OnSelect(BaseEventData eventData) {
            SoundEvent?.Invoke();
            _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
            _scaleUpTween = eventData.selectedObject.transform.DOScale(newScale, _scaleDuration);
            if (_textChangeOnSelect) {
                eventData.selectedObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }

        public void OnDeselect(BaseEventData eventData) {
            if (_animationExclusions.Contains(eventData.selectedObject))
                return;

            Selectable sel = eventData.selectedObject.GetComponent<Selectable>();
            _scaleDownTween = eventData.selectedObject.transform.DOScale(_scales[sel], _scaleDuration);
            if (_textChangeOnSelect) {
                eventData.selectedObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            }
        }

        public void OnPointerEnter(BaseEventData eventData) {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null) {
                Selectable sel = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
                if (sel == null) {
                    sel = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
                }

                pointerEventData.selectedObject = sel.gameObject;
            }
        }

        public void OnPointerExit(BaseEventData eventData) {
            PointerEventData pointerEventData = eventData as PointerEventData;
            if (pointerEventData != null) {
                pointerEventData.selectedObject = null;
            }
        }

        protected virtual void OnNavigate(InputAction.CallbackContext context) {
            if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null) {
                EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
            }
        }
    }
}
