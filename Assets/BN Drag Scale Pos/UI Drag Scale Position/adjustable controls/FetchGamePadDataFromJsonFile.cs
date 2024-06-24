using DragHandler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FetchGamePadDataFromJsonFile : MonoBehaviour
{

    [SerializeField] RectTransform[] groupedUi;

    [SerializeField] RectTransform[] ui;

    public string GamePadPropertiesFile => Application.persistentDataPath + "/GamePadProperties.json";

    AdjustControl _adjustControl;
    Adjust _adjust;

    private void Awake()
    {
        _adjustControl = new AdjustControl();
        _adjust = new Adjust();
    }

    void Start()
    {
        if(File.Exists(GamePadPropertiesFile))
        {
            _adjust = _adjustControl.ReadDataFromFile(GamePadPropertiesFile);
            for (int i = 0; i < _adjust.DragObject_Properties_List.Count; i++)
            {
                ui[i].anchoredPosition3D = _adjust.DragObject_Properties_List[i].Anchor3DPosition;
                ui[i].localScale = _adjust.DragObject_Properties_List[i].LocalScale;
                ui[i].anchorMin = _adjust.DragObject_Properties_List[i].AnchorMin;
                ui[i].anchorMax = _adjust.DragObject_Properties_List[i].AnchorMax;
                ui[i].pivot = _adjust.DragObject_Properties_List[i].Pivot;
                Debug.Log($"ID {_adjust.DragObject_Properties_List[i].Id}");
            }

            // Update right group of punch, grenade using fire UI 
            groupedUi[1].anchoredPosition3D = groupedUi[2].anchoredPosition3D = groupedUi[0].anchoredPosition3D;
            groupedUi[1].localScale = groupedUi[2].localScale = groupedUi[0].localScale;
            groupedUi[1].anchorMin = groupedUi[2].anchorMin = groupedUi[0].anchorMin;
            groupedUi[1].anchorMax = groupedUi[2].anchorMax = groupedUi[0].anchorMax;
            groupedUi[1].pivot = groupedUi[2].pivot = groupedUi[0].pivot;

            // Update left group of punch, grenade using fire UI 
            groupedUi[4].anchoredPosition3D = groupedUi[5].anchoredPosition3D = groupedUi[3].anchoredPosition3D;
            groupedUi[4].localScale = groupedUi[5].localScale = groupedUi[3].localScale;
            groupedUi[4].anchorMin = groupedUi[5].anchorMin = groupedUi[3].anchorMin;
            groupedUi[4].anchorMax = groupedUi[5].anchorMax = groupedUi[3].anchorMax;
            groupedUi[4].pivot = groupedUi[5].pivot = groupedUi[3].pivot;
        }
        else
        {
            Debug.Log("File does not exits");
        }
    }

    public void LoadUiDragScene()
    {
        SceneManager.LoadSceneAsync(0);
    }


}
