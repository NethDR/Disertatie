using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
    {
        // [SerializeField]
        // private InputActionAsset inputActionAsset;
        [SerializeField] private float mmbDragSensitivity = 0.1f;
        [SerializeField] private bool invertMmbDrag;

        [SerializeField] private float edgePanMargin = 10f;
        [SerializeField] private float edgePanSpeed = 0.5f;


        private RtsActions.CameraControlActions _cameraControlMap;
        private InputAction _mmbDrag;

        // [SerializeField]
        // private Cursor[,] cursors = new Cursor[3,3];

        private void Start()
        {
            // _terrainData = GetComponentInParent<TerrainAddedData>();

            var actions = new RtsActions();

            _cameraControlMap = actions.CameraControl;
            // _cameraControlMap = new RtsActions.CameraControlActions();;

            _cameraControlMap.MMBDrag.performed += MMBDragOnPerformed;

            actions.Enable();
        }


        private void Update()
        {
            // TODO fix scrolling during box select
            if (!_cameraControlMap.MMBDrag.inProgress &&
                !ActionAssetHolder.Instance.Actions.UnitControl.BoxSelect.inProgress)
            {
                var scrollDir = Vector2.zero;

                if (Mouse.current.position.x.value < edgePanMargin)
                    scrollDir.x = -1;

                if (Mouse.current.position.x.value > Screen.width - edgePanMargin)
                    scrollDir.x = 1;

                if (Mouse.current.position.y.value < edgePanMargin)
                    scrollDir.y = -1;

                if (Mouse.current.position.y.value > Screen.height - edgePanMargin)
                    scrollDir.y = 1;


                scrollDir.Normalize();
                scrollDir *= edgePanSpeed;

                transform.Translate(scrollDir.x, 0, scrollDir.y);
            }
        }


        private void MMBDragOnPerformed(InputAction.CallbackContext obj)
        {
            var val = _cameraControlMap.MMBDrag.ReadValue<Vector2>() * mmbDragSensitivity;
            if (!invertMmbDrag) val = -val;

            // UnityEngine.Debug.Log();
            transform.Translate(val.x, 0, val.y, Space.Self);
        }
    }