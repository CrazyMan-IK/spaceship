using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public class StandardCaseViewPopup : MonoBehaviour
    {
        [SerializeField] StandardCaseView standardCaseView;
        [SerializeField] Image icon;
        [SerializeField] StandardButton closeButton;

        [SerializeField] RectTransform sizeSampleTarget;
        [SerializeField] RectTransform sizeTarget;
        [SerializeField] Vector2 sizeTargetOffset;

        public StandardCaseView StandardCaseView => standardCaseView;

        protected virtual StandardAssetLoader Loader { get; set; } = new StandardAssetLoader();

        public virtual void Open(IStandardCaseCellData caseData, Action onCloseButton)
        {
            standardCaseView.Apply(caseData.CaseData);
            StartCoroutine(Loader.LoadAsync(caseData.ImageAsset, sprite => icon.sprite = sprite));
            closeButton.SetCallback(() => onCloseButton());

            // wait for relayout
            StartCoroutine(Relayout());
        }

        IEnumerator Relayout()
        {
            yield return null;

            sizeTarget.sizeDelta = sizeSampleTarget.rect.size + sizeTargetOffset;
        }
    }
}
