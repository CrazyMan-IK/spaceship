using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using VariableInventorySystem;

namespace Astetrio.Spaceship.InventorySystem
{
    public class StandardInventoryCore : VariableInventoryCore<CustomItemCellData>
    {
        [SerializeField] private InterfaceReference<IVariableInventoryCell> cellPrefab;
        [SerializeField] private StandardCaseViewPopup casePopup;
        [SerializeField] private RectTransform effectCellParent;

        protected override InterfaceReference<IVariableInventoryCell> CellPrefab => cellPrefab;
        protected override RectTransform EffectCellParent => effectCellParent;

        protected List<IStandardCaseCellData> popupList = new List<IStandardCaseCellData>();
        protected IStandardCaseCellData lastPopup = null;

        protected override void OnCellClick(IVariableInventoryCell cell)
        {
            if (cell.CellData is IStandardCaseCellData caseData)
            {
                /*if (popupList.Contains(caseData))
                {
                    return;
                }*/
                if (lastPopup != null)
                {
                    RemoveInventoryView(casePopup.StandardCaseView);
                }

                //popupList.Add(caseData);
                lastPopup = caseData;

                //var standardCaseViewPopup = Instantiate(casePopupPrefab, caseParent).GetComponent<StandardCaseViewPopup>();
                //casePopup = Instantiate(casePopup, caseParent).GetComponent<StandardCaseViewPopup>();
                casePopup.gameObject.SetActive(true);
                AddInventoryView(casePopup.StandardCaseView);

                casePopup.Open(caseData, () => {
                    RemoveInventoryView(casePopup.StandardCaseView);
                    //Destroy(casePopup.gameObject);
                    casePopup.gameObject.SetActive(false);
                    //popupList.Remove(caseData);
                    lastPopup = null;
                });
            }
        }
    }
}
