using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance;


        [SerializeField] private RectTransform boxVisual;

        [SerializeField] private LayerMask clickable;

        private readonly HashSet<Unit> _allUnits = new();


        public readonly HashSet<Unit> Selection = new();

        private RtsActions.UnitControlActions _actionMap;

        private bool _boxConfirmed;
        private bool _boxInProgress;

        private UnityEngine.Camera _camera;
        private Vector2 _endPosition;

        private Rect _selectionBox;

        private Vector2 _startPosition;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _actionMap = ActionAssetHolder.Instance.Actions.UnitControl;

            _actionMap.Select.performed += PerformSelection;

            _actionMap.BoxSelect.started += StartBoxSelection;
            _actionMap.BoxSelect.canceled += CancelBoxSelection;
            _actionMap.BoxSelect.performed += PerformBoxSelect;


            _camera = UnityEngine.Camera.main;

            GameObject.Find("SelectionCanvas").GetComponent<Canvas>();
        }


        private void Update()
        {
            if (_boxInProgress)
            {
                _endPosition = Input.mousePosition;

                UpdateRect();
                DrawSelection();
            }
        }

        public void RegisterUnit(Unit obj)
        {
            _allUnits.Add(obj);
        }

        public void DeregisterUnit(Unit obj)
        {
            _allUnits.Remove(obj);
        }

        private void UpdateRect()
        {
            var boxStart = _startPosition;
            var boxEnd = _endPosition;
            var boxCenter = (boxStart + boxEnd) / 2;
            boxVisual.position = boxCenter;
            var boxSize = (boxStart - boxEnd).Abs();
            boxVisual.sizeDelta = boxSize;

            // boxVisual.position = _selectionBox.position;
            // boxVisual.sizeDelta = _selectionBox.size;
        }

        private void StartBoxSelection(InputAction.CallbackContext obj)
        {
            // Debug.Log("Start Box");
            _boxConfirmed = false;
            _boxInProgress = true;

            _startPosition = Mouse.current.position.value;
            boxVisual.gameObject.SetActive(true);
        }

        private void DrawSelection()
        {
            if (Input.mousePosition.x < _startPosition.x)
            {
                _selectionBox.xMin = Input.mousePosition.x;
                _selectionBox.xMax = _startPosition.x;
            }
            else
            {
                _selectionBox.xMin = _startPosition.x;
                _selectionBox.xMax = Input.mousePosition.x;
            }


            if (Input.mousePosition.y < _startPosition.y)
            {
                _selectionBox.yMin = Input.mousePosition.y;
                _selectionBox.yMax = _startPosition.y;
            }
            else
            {
                _selectionBox.yMin = _startPosition.y;
                _selectionBox.yMax = Input.mousePosition.y;
            }
        }

        private void PerformBoxSelect(InputAction.CallbackContext obj)
        {
            // Debug.Log("Box select confirmed");
            _boxConfirmed = true;
        }


        private void CancelBoxSelection(InputAction.CallbackContext obj)
        {
            // Debug.Log("Cancel Box");
            if (_boxConfirmed)
            {
                Debug.Log("Box Select");
                // _endPosition = Input.mousePosition;
                // UpdateRect();

                List<Unit> boxSelection = new();

                Debug.Log(_selectionBox);

                foreach (var unit in _allUnits)
                {
                    Debug.Log(_camera.WorldToScreenPoint(unit.transform.position));

                    if (_selectionBox.Contains(_camera.WorldToScreenPoint(unit.transform.position)))
                    {
                        boxSelection.Add(unit);
                        Debug.Log(unit.name);
                    }
                    else
                    {
                        Debug.Log(unit.name + " Skipped");
                    }
                }

                if (!_actionMap.SelectionModifier.IsPressed())
                {
                    Select(boxSelection);
                }
                else
                {
                    if (!boxSelection.All(IsSelected))
                        AddToSelection(boxSelection);
                    else
                        Deselect(boxSelection);
                }
            }

            _boxInProgress = false;
            boxVisual.gameObject.SetActive(false);
        }

        private static bool IsSelected(Unit unit)
        {
            return unit.GetComponent<Unit>().IsSelected;
        }

        private void PerformSelection(InputAction.CallbackContext obj)
        {
            Debug.Log("Select Single");

            var ray = _camera.ScreenPointToRay(Mouse.current.position.value);

            var selectionModifier = _actionMap.SelectionModifier;
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, clickable))
            {
                var clicked = hit.collider.gameObject;
                if (selectionModifier.IsPressed())
                    ModifySelection(clicked.GetComponent<Unit>());
                else
                    Select(clicked.GetComponent<Unit>());
            }
            else
            {
                if (!selectionModifier.IsPressed())
                    DeselectAll();
            }
        }

        private void AddToSelection(Unit obj)
        {
            obj.GetComponent<Unit>().IsSelected = true;
            Selection.Add(obj);
        }

        private void AddToSelection(ICollection<Unit> objs)
        {
            foreach (var obj in objs) AddToSelection(obj);
        }

        private void Select(Unit obj)
        {
            DeselectAll();
            AddToSelection(obj);
        }

        private void Select(ICollection<Unit> objs)
        {
            DeselectAll();
            AddToSelection(objs);
        }

        private void ModifySelection(Unit obj)
        {
            if (!Deselect(obj)) AddToSelection(obj);
        }

        private bool Deselect(Unit obj)
        {
            return Selection.Remove(obj);
        }

        private void Deselect(ICollection<Unit> objs)
        {
            Selection.RemoveWhere(objs.Contains);
        }

        private void DeselectAll()
        {
            foreach (var selectable in Selection) selectable.GetComponent<Unit>().IsSelected = false;
            Selection.Clear();
        }
    }