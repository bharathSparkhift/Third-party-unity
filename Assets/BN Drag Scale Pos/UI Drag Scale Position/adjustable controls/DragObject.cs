using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DragHandler
{
    [Serializable]
    public class DragObjectProperties
    {
        public int Id;
        public Vector3 Anchor3DPosition;
        public Vector3 LocalScale;
        public Vector3 AnchorMin;
        public Vector3 AnchorMax;
        public Vector3 Pivot;
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class DragObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {

        /// <summary>
        /// Yes -> Objects can      be dragged
        /// No  -> Objects can't    be dragged.
        /// </summary>
        public enum DraggableType
        {
            
            Joystick,
            Draggable,
            NonDraggable,
            FingerSwipePanel
            
        }

        /*public enum SelectedEnum
        {
            No, 
            Yes
        }*/

        public enum PivotPlacementType
        {
            None,
            Left,
            Center,
            Right,

        }

        [SerializeField] private RectTransform _joystickRectTransform;

        [SerializeField] private AdjustControl adjustControl;

        [SerializeField] private Canvas canvas;

        // [SerializeField] private CanvasGroup canvasGroup;

        [field: SerializeField] public int id { get; private set; }

        //[HideInInspector]
        [SerializeField] private float minClampX;

        //[HideInInspector]
        [SerializeField] private float maxClampX;

        //[HideInInspector]
        [SerializeField] private float minClampY;

        //[HideInInspector]
        [SerializeField] private float maxClampY;

        [HideInInspector]
        /// <summary>
        /// Resetting UI, anchor position is being cached.
        /// </summary>
        [SerializeField] private Vector3 cacheRectPosition;
        [HideInInspector]
        [SerializeField] private Vector3 cacheRectLocalScale;

        [HideInInspector]
        [SerializeField] private Vector2 cacheMinAcnhor;
        [HideInInspector]
        [SerializeField] private Vector2 cacheMaxAnchor;
        [HideInInspector]
        [SerializeField] private Vector2 cachePivot;

        private RectTransform _canvasRectTransform;
        

        [field: SerializeField] public RectTransform Rect_Transform { get; private set; }

        [HideInInspector]
        public DragObjectProperties DragObject_Properties;

        /// <summary>
        /// Yes when object gets selected
        /// No when object gets deselected.
        /// </summary>
        // public SelectedEnum Selected;

        /// <summary>
        /// Is Object draggable
        /// </summary>
        public DraggableType Draggable;
        /// <summary>
        /// 
        /// </summary>
        public PivotPlacementType PivotPlacement;

        #region 
        private void Awake()
        {
            
        }


        // Start is called before the first frame update
        void Start()
        {
            _canvasRectTransform = canvas.GetComponent<RectTransform>();
            /*_canvasRectTransform.rect.width = _canvasRectTransform.rect.width;
            screenHeight = _canvasRectTransform.rect.height;*/
            Rect_Transform = GetComponent<RectTransform>();
            UpdateSerializableDragObjectProperties();
        }

        private void OnEnable()
        {
            
        }

        private void Update()
        {
            if (transform.tag == "Floating Joystick Panel")
            {
                Rect_Transform.anchoredPosition = new Vector2(_joystickRectTransform.anchoredPosition.x, Rect_Transform.anchoredPosition.y); 
            }
        }

       


        
        #endregion

        #region Drag event handlers
        public void OnPointerDown(PointerEventData eventData)
        {

            if (Draggable == DraggableType.NonDraggable && Draggable == DraggableType.FingerSwipePanel)
                return;

            // update the UI which is pressed with the finger.
            adjustControl.UpdateCurrentControl(Rect_Transform, GetComponent<Image>(), this);  



            UpdateUiRectProperties(true);

            cacheRectPosition = Rect_Transform.anchoredPosition3D;

            cacheRectLocalScale = Rect_Transform.localScale;

            //canvasGroup.blocksRaycasts = false;

            // Debug.Log($"{nameof(OnPointerDown)} \t {this.transform.name}");
        }

        



        public void OnDrag(PointerEventData eventData)
        {

            if (Draggable == DraggableType.NonDraggable || Draggable == DraggableType.FingerSwipePanel)
                return;

            var clampX = Mathf.Clamp(Rect_Transform.anchoredPosition3D.x, minClampX, maxClampX);
            var clampY = Mathf.Clamp(Rect_Transform.anchoredPosition3D.y, minClampY, maxClampY);
            
            Rect_Transform.anchoredPosition = new Vector2(clampX, clampY);
            Rect_Transform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            UpdateSerializableDragObjectProperties();
            Debug.Log($"{nameof(OnDrag)} \t {this.transform.name}");

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
            if (Draggable == DraggableType.NonDraggable && Draggable == DraggableType.FingerSwipePanel)
                return;

            //canvasGroup.blocksRaycasts = true;

            string valueOut = adjustControl.CheckUiOverlapping(this.Rect_Transform);
            if (valueOut.Equals("Non Draggable"))
            {
                Rect_Transform.anchoredPosition3D = cacheRectPosition;
                Debug.Log($"Boolean out {valueOut}");
            }


        }



        #endregion

        /// <summary>
        /// Detect the clamp values upon UI selection
        /// </summary>
        void UpdateUiRectProperties(bool onPointerDown)
        {
            if (Draggable == DraggableType.NonDraggable)
                return;
            
            if (PivotPlacement == PivotPlacementType.Center)
            {
                (minClampX, minClampY) = (-(_canvasRectTransform.rect.width - (Rect_Transform.sizeDelta.x * Rect_Transform.localScale.x)) / 2f, 
                                            (Rect_Transform.sizeDelta.y * Rect_Transform.localScale.y / 2));
                (maxClampX, maxClampY) = ((_canvasRectTransform.rect.width - (Rect_Transform.sizeDelta.x * Rect_Transform.localScale.x)) / 2f, 
                                            _canvasRectTransform.rect.height - (Rect_Transform.sizeDelta.y * Rect_Transform.localScale.y / 2f));
            }
            if (PivotPlacement == PivotPlacementType.Left)
            {
                (minClampX, minClampY) = (0, 0);
                (maxClampX, maxClampY) = (_canvasRectTransform.rect.width - (Rect_Transform.sizeDelta.x * Rect_Transform.localScale.x), 
                                            _canvasRectTransform.rect.height - (Rect_Transform.sizeDelta.y * Rect_Transform.localScale.y));
        
            }
            if (PivotPlacement == PivotPlacementType.Right)
            {
                (minClampX, minClampY) = (-(_canvasRectTransform.rect.width - (Rect_Transform.sizeDelta.x * Rect_Transform.localScale.x)), 0);
                (maxClampX, maxClampY) = (0, _canvasRectTransform.rect.height - (Rect_Transform.sizeDelta.y * Rect_Transform.localScale.y));
                // -(Rect_Transform.sizeDelta.x * Rect_Transform.localScale.x) 
            }

            if (!onPointerDown)
            {
                Rect_Transform.anchoredPosition = new Vector2(
                       Mathf.Clamp(Rect_Transform.anchoredPosition.x, minClampX, maxClampX),
                       Mathf.Clamp(Rect_Transform.anchoredPosition.y, minClampY, maxClampY)
                   );
            }


        }

        /*internal void CacheFingerSwipePanelProperties(Vector2 minAnchor, Vector2 maxAnchor, Vector2 pivot)
        {
            cacheMinAcnhor = minAnchor;
            cacheMaxAnchor = maxAnchor;
            cachePivot = pivot;
        }*/

        /// <summary>
        /// Reset the Rect transform properties to its selected state.
        /// </summary>
        internal void ResetRectProperties()
        {

            // Reset if the Current element is of Draggable or Joystick type
            if(Draggable == DraggableType.Draggable || Draggable == DraggableType.Joystick)
            {
                Rect_Transform.anchoredPosition3D = cacheRectPosition;
                Rect_Transform.localScale = cacheRectLocalScale;
            }
            /*// If the current element is of Finger Swipe panel type.
            if(Draggable == DraggableType.FingerSwipePanel)
            {
                Rect_Transform.anchorMin    = cacheMinAcnhor;
                Rect_Transform.anchorMax    = cacheMaxAnchor;
                Rect_Transform.pivot        = cachePivot;
            }*/
            // Selected = SelectedEnum.No;
            Debug.Log($"{nameof(ResetRectProperties)}");
        }

        internal void UpdateScaleOnValueChanged(float value)
        {
            /*if(Draggable == DraggableType.Joystick)
            {
                if(value < 1)
                {
                    Rect_Transform.localScale = Vector3.one; //new Vector3(value, value, value);
                    Rect_Transform.GetChild(0).localScale = new Vector3(value, value, value);
                }
                else if(value >= 1)
                {
                    Rect_Transform.localScale = Vector3.one;
                    Rect_Transform.GetChild(0).localScale = new Vector3(value, value, value);
                }
            }
            else
            {
                Rect_Transform.localScale = new Vector3(value, value, value);
            }*/
            Rect_Transform.localScale = new Vector3(value, value, value);
            UpdateUiRectProperties(false);
            UpdateSerializableDragObjectProperties();
        }


        public DragObjectProperties UpdateSerializableDragObjectProperties()
        {
            
            DragObjectProperties DragObject_Properties = new DragObjectProperties();
            Rect_Transform = GetComponent<RectTransform>();

            DragObject_Properties.Id = id;
            DragObject_Properties.Anchor3DPosition = Rect_Transform.anchoredPosition3D;
            DragObject_Properties.LocalScale = Rect_Transform.localScale;
            DragObject_Properties.AnchorMin = Rect_Transform.anchorMin;
            DragObject_Properties.AnchorMax = Rect_Transform.anchorMax;
            DragObject_Properties.Pivot = Rect_Transform.pivot;


            return DragObject_Properties;
        }

        
    }
}

