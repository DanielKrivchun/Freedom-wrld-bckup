using System;
using UnityEngine;
using UnityEngine.Events;
using Beamable.Api.Inventory;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Inventory;

namespace Beamable.InventoryService
{
    [Serializable]
    public class RefreshedUnityEvent : UnityEvent { }

    public class BeamableInventoryManager : MonoBehaviour
    {
        public static BeamableInventoryManager instance;

        // Events
        [HideInInspector] public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

        // Fields
        private BeamContext _beamContext;
        private Api.Inventory.InventoryService _inventoryService;

        [Header("Currency Reference")]
        public CurrencyRef _currencyRefPrimary; // Add this reference in the Inspector

        // Unity Methods
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                DestroyImmediate(instance);
            }

            DontDestroyOnLoad(gameObject);
        }

        protected void Start()
        {
            SetupBeamable();
        }

        // Methods
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;

            _inventoryService = _beamContext.Api.InventoryService;

            // Add any initialization or setup code here
        }

        public async void AddCurrency(int amount)
        {
            if (_currencyRefPrimary == null)
            {
                Debug.LogError("_currencyRefPrimary is not assigned.");
                return;
            }

            var currencyContentPrimary = await _currencyRefPrimary.Resolve();
            InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
            inventoryUpdateBuilder.CurrencyChange(currencyContentPrimary.Id, + amount);

            await _inventoryService.Update(inventoryUpdateBuilder).Then(obj =>
            {
                Debug.Log($"AddCurrency() success.");
                OnRefreshed.Invoke();
            });
        }

        public void Refresh()
        {
            // Placeholder for refresh logic if needed
            Debug.Log("Inventory data refreshed.");
            OnRefreshed.Invoke();
        }

        // Event Handlers
        // Add any event handlers if needed
    }
}