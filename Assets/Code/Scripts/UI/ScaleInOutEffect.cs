using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleInOutEffect : MonoBehaviour
{
    [SerializeField] GameObject panelObj;

    private void OnEnable()
    {
        ScaleUpPanel();
    }

    #region SCALE EFFECT PANEL UI
    void ScaleUpPanel()
    {
        panelObj.transform.DOScale(1f, 0.35f);
    }

    public void ScaleDownAndOffPanel()
    {
        panelObj.transform.DOScale(0.2f, 0.25f).OnComplete(() => gameObject.SetActive(false));
    }
    #endregion
}
