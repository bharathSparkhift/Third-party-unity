using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DragHandler
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Unity.VisualScripting;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class Adjust
    {

        public List<DragObjectProperties> DragObject_Properties_List = new List<DragObjectProperties>();

    }

    public class AdjustControl : MonoBehaviour
    {

        [SerializeField] RectTransform currentControl;
        [SerializeField] Image currentControlImage;
        [SerializeField] DragObject currentDragObject;

        [SerializeField] Canvas currentCanvas;
        [SerializeField] Slider sizeSlider;
        [SerializeField] Button saveButton;
        [SerializeField] Button resetButton;

        [SerializeField] DragObject[] draggableObjects;
        [SerializeField] RectTransform[] nonDraggableObjects;

        Adjust _cachedAdjustJsonData;

        public static AdjustControl Instance;

        public string GamePadPropertiesFile => Application.persistentDataPath + "/GamePadProperties.json";
        public string GamePadInitialPropertiesFile => Application.persistentDataPath + "/GamePadInitialProperties.json";

        private void Awake()
        {
            Instance = this;
            _cachedAdjustJsonData = new Adjust();
            sizeSlider.onValueChanged.AddListener(OnScaleSliderValueChanged);
        }

        // Start is called before the first frame update
        void Start()
        {

            saveButton.enabled = false;
            /*saveButton.onClick.AddListener(SaveDataToLocalJsonFile);
            resetButton.onClick.AddListener(ResetUi);*/

            Debug.Log($"Persistent file path: {GamePadPropertiesFile}");

            // Check for file not exists on the local path of the device.
            if (!File.Exists(GamePadPropertiesFile) && !File.Exists(GamePadInitialPropertiesFile))
            {
                _cachedAdjustJsonData = UpdateIndividualRectProperties();

                string jsonDataText = JsonUtility.ToJson(_cachedAdjustJsonData, prettyPrint: true);

                Write_RectProperties_of_Game_Pad_Ui_To_Json_File(GamePadPropertiesFile, jsonDataText);
                Write_RectProperties_of_Game_Pad_Ui_To_Json_File(GamePadInitialPropertiesFile, jsonDataText);

                // Write_Initial_RectProperties_of_Game_Pad_Ui_To_Json_File(GamePadInitialPropertiesFile, jsonDataText);

                Debug.Log($"File created for the first time");
            }
            if (File.Exists(GamePadPropertiesFile) && File.Exists(GamePadInitialPropertiesFile))
            {
                Read_RectProperties_From_Json_File_Update_On_UI(GamePadPropertiesFile);
            }
        }

        private void OnEnable()
        {
            saveButton.onClick.AddListener(SaveDataToLocalJsonFile);
            resetButton.onClick.AddListener(ResetUi);
            // saveButton.enabled = false;
        }

        private void OnDisable()
        {
            saveButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            OnDisable();
        }


        public Adjust Read_RectProperties_From_Json_File_Update_On_UI(string filePath)
        {

            _cachedAdjustJsonData = ReadDataFromFile(filePath);

            for (int i = 0; i < _cachedAdjustJsonData.DragObject_Properties_List.Count; i++)
            {

                draggableObjects[i].DragObject_Properties.Id = _cachedAdjustJsonData.DragObject_Properties_List[i].Id;
                draggableObjects[i].Rect_Transform.anchoredPosition3D = _cachedAdjustJsonData.DragObject_Properties_List[i].Anchor3DPosition;
                draggableObjects[i].Rect_Transform.localScale = _cachedAdjustJsonData.DragObject_Properties_List[i].LocalScale;
                draggableObjects[i].Rect_Transform.anchorMin = _cachedAdjustJsonData.DragObject_Properties_List[i].AnchorMin;
                draggableObjects[i].Rect_Transform.anchorMax = _cachedAdjustJsonData.DragObject_Properties_List[i].AnchorMax;
                draggableObjects[i].Rect_Transform.pivot = _cachedAdjustJsonData.DragObject_Properties_List[i].Pivot;
                
                
                /*draggableObjects[i].Rect_Transform.anchoredPosition3D = _cachedAdjustJsonData.DragObject_Properties_List[i].Anchor3DPosition;
                draggableObjects[i].Rect_Transform.localScale = _cachedAdjustJsonData.DragObject_Properties_List[i].LocalScale;
                draggableObjects[i].Rect_Transform.anchorMin = _cachedAdjustJsonData.DragObject_Properties_List[i].AnchorMin;
                draggableObjects[i].Rect_Transform.anchorMax = _cachedAdjustJsonData.DragObject_Properties_List[i].AnchorMax;
                draggableObjects[i].Rect_Transform.pivot = _cachedAdjustJsonData.DragObject_Properties_List[i].Pivot;*/

            }

            Debug.Log($"Reading from the file");
            return _cachedAdjustJsonData;
        }


        private Adjust UpdateIndividualRectProperties()
        {
            // Clear the list.
            _cachedAdjustJsonData.DragObject_Properties_List.Clear();

            // Write an empty contents to the JSON File.
            Write_RectProperties_of_Game_Pad_Ui_To_Json_File(GamePadPropertiesFile, string.Empty);

            for (int i = 0; i < draggableObjects.Length; i++)
            {
                _cachedAdjustJsonData.DragObject_Properties_List.Add(draggableObjects[i].UpdateSerializableDragObjectProperties());
            }

            string jsonOut = JsonUtility.ToJson(_cachedAdjustJsonData.DragObject_Properties_List);
            Debug.Log($"Json out {jsonOut}");
            return _cachedAdjustJsonData;
        }



        /// <summary>
        /// Write the data to the JSON file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="jsonDataText"></param>
        void Write_RectProperties_of_Game_Pad_Ui_To_Json_File(string filePath, string jsonDataText)
        {
            
            File.WriteAllText(filePath, jsonDataText); 
        }

        void Write_Initial_RectProperties_of_Game_Pad_Ui_To_Json_File(string filePath, string jsonDataText)
        {
            File.WriteAllText(filePath, jsonDataText);
        }

        /// <summary>
        /// Read the data from the JSON file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Adjust ReadDataFromFile(string filePath)
        {
            string fromJsonData = File.ReadAllText(filePath);
            Adjust adjustData = JsonUtility.FromJson<Adjust>(fromJsonData);
            return adjustData;
        }


        /// <summary>
        /// Save the data to the Local JSON file on button click 
        /// from HasSet<T>
        /// </summary>
        public void SaveDataToLocalJsonFile()
        {
            // _cachedAdjustJsonData = UpdateSwipePanelRectProperties();
            _cachedAdjustJsonData = UpdateIndividualRectProperties();

            string toJsonDataText = JsonUtility.ToJson(_cachedAdjustJsonData, true);
            Write_RectProperties_of_Game_Pad_Ui_To_Json_File(GamePadPropertiesFile, toJsonDataText);


        }



        /// <summary>
        /// Reset UI to its initial or previous positions on button click
        /// </summary>
        void ResetUi()
        {
            sizeSlider.value = 1;
            Read_RectProperties_From_Json_File_Update_On_UI(GamePadInitialPropertiesFile);
            SaveDataToLocalJsonFile();

        }

        /// <summary>
        /// Value channge on slider move 
        /// </summary>
        /// <param name="value"></param>
        void OnScaleSliderValueChanged(float value)
        {
            saveButton.enabled = true;
            currentControl.localScale = new Vector3(value, value, value);
            currentDragObject.UpdateScaleOnValueChanged(value);
            

        }

        /*/// <summary>
        /// Update the Current UI on pointer down.
        /// </summary>
        /// <param name="selectedRectTransform"></param>
        /// <param name="id"></param>
        public void UpdateCurrentControl(RectTransform selectedRectTransform, Image selectedImage, DragObject.DraggableType draggableType)
        {
            Image previoursImage = currentControl.GetComponent<Image>();

            previoursImage.color = (currentControl.GetComponent<DragHandler.DragObject>().Draggable == DragObject.DraggableType.Joystick) ? new Color(255, 255, 255, 20) / 255f : new Color(255, 255, 255, 255) / 255f;

            currentControl = selectedRectTransform;

            currentDragObject = selectedRectTransform.GetComponent<DragHandler.DragObject>();

            selectedImage.color = (selectedImage.GetComponent<DragHandler.DragObject>().Draggable == DragObject.DraggableType.Joystick) ? new Color(255, 256, 0, 20) / 255f : new Color(255, 256, 0, 255) / 255f;

            sizeSlider.value = currentControl.localScale.x;

        }*/

        public void UpdateCurrentControl(RectTransform selectedRectTransform, Image selectedImage, DragObject dragObject)
        {
            Image previousImage = currentControlImage;
            previousImage.color = currentDragObject.Draggable == DragObject.DraggableType.Joystick ? new Color(255, 255, 255, 20) / 255f : new Color(255, 255, 255, 255) / 255f;

            currentControl = selectedRectTransform;
            currentControlImage = selectedImage;
            currentDragObject = dragObject; // currentControl.GetComponent<DragHandler.DragObject>();
            sizeSlider.value = currentControl.localScale.x;

            currentControlImage.color = currentDragObject.Draggable == DragObject.DraggableType.Joystick ? new Color(255, 256, 0, 20) / 255f : new Color(255, 256, 0, 255) / 255f;
        }

                
        public string CheckUiOverlapping(RectTransform draggingUiRectTransform)
        {
            Rect draggingUiRect;

            // Convert draggingUiRectTransform to world coordinates
            Vector3[] draggingWorldCorners = new Vector3[4];
            draggingUiRectTransform.GetWorldCorners(draggingWorldCorners);
            draggingUiRect = new Rect(draggingWorldCorners[0].x, draggingWorldCorners[0].y,
                                      draggingWorldCorners[2].x - draggingWorldCorners[0].x,
                                      draggingWorldCorners[2].y - draggingWorldCorners[0].y);

            foreach (RectTransform eachUiRect in nonDraggableObjects)
            {
                Rect targetRect;
                if (eachUiRect.gameObject.tag == "Non Draggable") 
                {
                    // Convert eachUiRect to world coordinates
                    Vector3[] worldCorners = new Vector3[4];
                    eachUiRect.GetWorldCorners(worldCorners);
                    targetRect = new Rect(worldCorners[0].x, worldCorners[0].y,
                                          worldCorners[2].x - worldCorners[0].x,
                                          worldCorners[2].y - worldCorners[0].y);

                    if (draggingUiRect.Overlaps(targetRect))
                    {
                        return eachUiRect.gameObject.tag;
                    }
                }
                if(draggingUiRectTransform.tag == "Joystick") 
                {
                    // Convert eachUiRect to world coordinates
                    Vector3[] worldCorners = new Vector3[4];
                    eachUiRect.GetWorldCorners(worldCorners);
                    targetRect = new Rect(worldCorners[0].x, worldCorners[0].y,
                                          worldCorners[2].x - worldCorners[0].x,
                                          worldCorners[2].y - worldCorners[0].y);
                    /*if (draggingUiRect.Overlaps(targetRect))
                    {
                        Debug.Log("Joystick is crossing over finger swipe panel");
                        return eachUiRect.gameObject.tag;
                    }*/
                }
                
            }
            return string.Empty;
        }

        public void LoadMainScreen()
        {
            SceneManager.LoadSceneAsync(1);
        }


    }
}



