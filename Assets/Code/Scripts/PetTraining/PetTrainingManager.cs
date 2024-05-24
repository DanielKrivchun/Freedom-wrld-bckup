using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetTrainingManager : MonoBehaviour
{
    [Header("Pet Train Panel UI")]
    public GameObject petTrainPanel;
    public List<GameObject> trainingSelectBtns;
    public GameObject notSelectedTrainingMain;
    public List<GameObject> trainingStatsMain;
    public GameObject startTrainingBtn;

    [Header("Ongoing Training Popup UI")]
    public GameObject ongoingTrainingPopup;
    public Text ongoingTrainingMsgTxt;

    [Header("Completed Training Panel UI")]
    public GameObject completedTrainingPanel;
    public Text completedTrainingMsgTxt;
    public Text earnedStatsTxt;
    public Text niceWorkTxt;

    public void ShowPetTrainPanel()
    {
        //If any training going on then can't open Pet Train Panel
        if(!ongoingTrainingPopup.activeInHierarchy)
        {
            petTrainPanel.SetActive(true);
        }
    }

    public void CheckForSelectedTrainingStats(int index)
    {
        for (int i = 0; i < trainingSelectBtns.Count; i++)
        {
            if(i == index)
            {
                trainingSelectBtns[i].GetComponent<Image>().color = Color.green;
                trainingSelectBtns[i].GetComponentInChildren<Text>().text = "Selected";
                trainingStatsMain[i].SetActive(true);
            }
            else
            {
                trainingSelectBtns[i].GetComponent<Image>().color = Color.yellow;
                trainingSelectBtns[i].GetComponentInChildren<Text>().text = "Select";
                trainingStatsMain[i].SetActive(false);
            }
        }

        notSelectedTrainingMain.SetActive(false);
        startTrainingBtn.SetActive(true);
    }
}
