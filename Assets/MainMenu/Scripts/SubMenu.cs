using UnityEngine;
using UnityEngine.EventSystems;

    public class SubMenu : MonoBehaviour
    {
        [field: Header("Current Menu")]
        [field: SerializeField]
        public CanvasGroup CanvasGroup { get; private set; }

        [field: SerializeField] public GameObject FirstSelectedButton { get; private set; }

        [field: Header("Top Menu")]
        [field: SerializeField]
        public SubMenu TopSubmenu { get; private set; }

        [field: SerializeField] public GameObject TopSelectedButton { get; private set; }

        public void OpenSubMenu(SubMenu submenu)
        {
            OpenMenu(submenu, submenu.FirstSelectedButton);
            CloseMenu(this);
        }

        public void CloseSubMenu()
        {
            if (TopSubmenu != null) OpenMenu(TopSubmenu, TopSelectedButton);
            CloseMenu(this);
        }

        protected void OpenMenu(SubMenu submenu, GameObject firstSelected = null)
        {
            submenu.CanvasGroup.interactable = true;
            submenu.CanvasGroup.alpha = 1;
            submenu.CanvasGroup.blocksRaycasts = true;
            if (firstSelected != null) EventSystem.current.SetSelectedGameObject(firstSelected);
            //submenu.Inputs.UnityUI.Cancel.performed += submenu.TryCloseSubMenu;
        }

        protected void CloseMenu(SubMenu submenu, GameObject firstSelected = null)
        {
            submenu.CanvasGroup.interactable = false;
            submenu.CanvasGroup.alpha = 0;
            submenu.CanvasGroup.blocksRaycasts = false;
            if (firstSelected != null) EventSystem.current.SetSelectedGameObject(firstSelected);
            //submenu.Inputs.UnityUI.Cancel.performed -= submenu.TryCloseSubMenu;
        }
        
        // New Input System
        /*
        #region Cancel Input

        // Inputs
        public PlayerInputs Inputs { get; private set; }
        public static bool CanPressCancel { get; set; } = true;

        private void Awake()
        {
            Inputs = new PlayerInputs();
        }

        protected void OnEnable()
        {
            Inputs.Enable();
            Inputs.UnityUI.Cancel.canceled += PressCancel;
        }

        protected void OnDisable()
        {
            Inputs.Disable();
            Inputs.UnityUI.Cancel.canceled -= PressCancel;
        }

        protected virtual void TryCloseSubMenu(InputAction.CallbackContext context)
        {
            //if (CanvasGroup.alpha == 0) return;
            if (TopSubmenu == null) return;
            if (!CanPressCancel) return;


            Debug.Log("Close : " + gameObject.name);
            CanPressCancel = false;
            CloseSubMenu();
        }

        private void PressCancel(InputAction.CallbackContext context)
        {
            CanPressCancel = true;
        }

        #endregion*/
    }