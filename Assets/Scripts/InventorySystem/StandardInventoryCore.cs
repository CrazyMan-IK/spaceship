using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using AYellowpaper;
using VariableInventorySystem;
using System.Linq;

namespace Astetrio.Spaceship.InventorySystem
{
    public class StandardInventoryCore : VariableInventoryCore<CustomItemCellData>
    {
        [SerializeField] private InterfaceReference<IVariableInventoryCell> cellPrefab;
        [SerializeField] private StandardCaseViewPopup casePopup;
        [SerializeField] private OptionsPopup optionsPopup;
        [SerializeField] private RectTransform effectCellParent;

        protected override InterfaceReference<IVariableInventoryCell> CellPrefab => cellPrefab;
        protected override RectTransform EffectCellParent => effectCellParent;

        protected List<IStandardCaseCellData> popupList = new List<IStandardCaseCellData>();
        protected IStandardCaseCellData lastPopup = null;

        public override void OnEndDrag(PointerEventData eventData)
        {
            var oldCellData = effectCell?.CellData;
            IVariableInventoryView baseView = null;
            int effectCellID = -1;
            foreach (var view in InventoryViews)
            {
                effectCellID = view.GetEffectCellID() ?? -1;
                if (effectCellID >= 0)
                {
                    baseView = view;
                    break;
                }
            }

            base.OnEndDrag(eventData);

            if (oldCellData != null && stareCell?.CellData != oldCellData && stareCell?.CellData?.Id != oldCellData.Id)
            {
                OpenOptionsPopup(stareCell, oldCellData, new()
                {
                    { Combine, "Combine" }
                });
            }

            void Combine(ICellData stareCellData, ICellData effectCellData)
            {
                var originalEffectCellData = baseView.ViewData.GetCell(effectCellID);

                foreach (var recipe in Recipes)
                {
                    if (!recipe.Componenets.Contains(stareCellData.Information) || !recipe.Componenets.Contains(originalEffectCellData.Information))
                    {
                        continue;
                    }

                    var tempStareCellData = stareCellData.Clone(1);
                    var tempEffectCellData = originalEffectCellData.Clone(1);

                    stareCellData.Merge(tempStareCellData, new Quantity(-1));
                    originalEffectCellData.Merge(tempEffectCellData, new Quantity(-1));

                    if (stareCellData.Count < 1)
                    {
                        baseView.ViewData.InsertInventoryItem(baseView.ViewData.GetId(stareCellData) ?? 0, null);
                    }
                    if (originalEffectCellData.Count < 1)
                    {
                        baseView.ViewData.InsertInventoryItem(effectCellID, null);
                    }
                    baseView.ViewData.Apply();
                    baseView.Apply(baseView.ViewData);

                    foreach (var view in InventoryViews)
                    {
                        var id = view.ViewData.GetInsertableId(recipe.Result.AsCellData());

                        if (id.HasValue)  
                        {
                            //stareCellData.Merge(stareCellData.Clone(1), new Quantity(-1));
                            //effectCellData.Merge(effectCellData.Clone(1), new Quantity(-1));

                            //stareCell?.Apply(null);
                            //view.ViewData.InsertInventoryItem(view.ViewData.GetId(stareCellData) ?? 0, null);
                            //view.ViewData.InsertInventoryItem(view.ViewData.GetId(effectCellData) ?? 0, null);

                            view.ViewData.InsertInventoryItem(id.Value, recipe.Result.AsCellData());
                            view.Apply(view.ViewData);

                            return;
                        }
                        //else
                        //{
                            /*if (stareCellData.Count < 1)
                            {
                                view.ViewData.InsertInventoryItem(view.ViewData.GetId(stareCellData) ?? 0, null);
                            }
                            if (effectCellData.Count < 1)
                            {
                                view.ViewData.InsertInventoryItem(view.ViewData.GetId(effectCellData) ?? 0, null);
                            }*/
                        //}
                    }

                    InvokeDropedOutsideEvent(new CustomItemCellData(recipe.Result));
                }
            }
        }

        protected override void OnCellClick(IVariableInventoryCell cell)
        {
            if (cell.CellData is IStandardCaseCellData caseData)
            {
                if (lastPopup != null)
                {
                    RemoveInventoryView(casePopup.StandardCaseView);
                }

                lastPopup = caseData;

                casePopup.gameObject.SetActive(true);
                AddInventoryView(casePopup.StandardCaseView);

                casePopup.Open(caseData, () => {
                    RemoveInventoryView(casePopup.StandardCaseView);
                    casePopup.gameObject.SetActive(false);
                    lastPopup = null;
                });
            }
        }

        protected override void OnCellOptionClick(IVariableInventoryCell cell)
        {
            OpenOptionsPopup(stareCell, effectCell?.CellData, new()
            {
                { (stareCellData, effectCellData) => Debug.Log("Drop"), "Drop" }
            });
        }

        private void OpenOptionsPopup(IVariableInventoryCell stareCell, ICellData effectCellData, Dictionary<Action<ICellData, ICellData>, string> actions)
        {
            if (stareCell?.CellData != null)
            {
                optionsPopup.Open(stareCell, effectCellData, actions);
            }
        }
    }
}
