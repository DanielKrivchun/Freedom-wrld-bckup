using Beamable;
using Beamable.Common.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class UpdateContentOnEvent : MonoBehaviour
{
    [Header("Currency Reference")]
    public CurrencyRef _currencyRefPrimary; // Add this reference in the Inspector
    [Space]
    public TextMeshProUGUI coinText;

    private BeamContext _beamContext;

    private long targetAmount;
    private long currentAmount;

    protected void Start()
    {
        GetCurruncyValue();
    }

    public async void GetCurruncyValue()
    {
        _beamContext = BeamContext.Default;
        await _beamContext.OnReady;

        if (_currencyRefPrimary == null)
        {
            Debug.LogError("_currencyRefPrimary is not assigned.");
            return;
        }
        // Acquire a context
        var ctx = await BeamContext.Default.Instance;

        // Get the current balance of the user's primary currency
        var currencyContentPrimary = await _currencyRefPrimary.Resolve();
        var currencyBalances = await ctx.Api.InventoryService.GetCurrent();

        if (currencyBalances.currencies.TryGetValue(currencyContentPrimary.Id, out var userCurrencyBalance))
        {
            Debug.Log(userCurrencyBalance);
            targetAmount = userCurrencyBalance;
            currentAmount = 0;
            while (currentAmount != targetAmount)
            {
                await Utils._Waiter(10);
                coinText.text = currentAmount.ToString();
                currentAmount++;
            }

            coinText.text = targetAmount.ToString();
        }
    }
}
