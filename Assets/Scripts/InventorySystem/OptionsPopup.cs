using Astetrio.Spaceship.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using VariableInventorySystem;

namespace Astetrio.Spaceship.InventorySystem
{
    public class OptionsPopup : MonoBehaviour
    {
        [SerializeField] private PopupItem _itemPrefab = null;
        [SerializeField] private RectTransform _content = null;
        //[SerializeField] StandardCaseView standardCaseView;
        //[SerializeField] Image icon;
        //[SerializeField] StandardButton closeButton;

        //[SerializeField] RectTransform sizeSampleTarget;
        //[SerializeField] RectTransform sizeTarget;
        //[SerializeField] Vector2 sizeTargetOffset;

        //public StandardCaseView StandardCaseView => standardCaseView;

        //protected virtual StandardAssetLoader Loader { get; set; } = new StandardAssetLoader();

        private void Update()
        {
            var localPosition = _content.InverseTransformPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0) && !_content.rect.Contains(localPosition))
            {
                gameObject.SetActive(false);
            }
        }

        public void Open(IVariableInventoryCell stareCell, ICellData effectCellData, Dictionary<Action<ICellData, ICellData>, string> actions)
        {
            /*if (stareCell?.CellData != null && effectCellData != null)
            {

            }*/

            foreach (Transform child in _content)
            {
                Destroy(child.gameObject);
            }

            gameObject.SetActive(true);

            foreach (var action in actions)
            {
                var item = Instantiate(_itemPrefab, _content);
                item.Initialize(() => {
                    action.Key?.Invoke(stareCell.CellData, effectCellData);

                    gameObject.SetActive(false);
                }, action.Value);
            }

            transform.position = Input.mousePosition; //cell.RectTransform.position;

            //standardCaseView.Apply(caseData.CaseData);
            //StartCoroutine(Loader.LoadAsync(caseData.ImageAsset, sprite => icon.sprite = sprite));
            //closeButton.SetCallback(() => onCloseButton());

            // wait for relayout
            //StartCoroutine(DelayFrame(() => sizeTarget.sizeDelta = sizeSampleTarget.rect.size + sizeTargetOffset));
        }

        /*public void Close()
        {
            _onClose?.Invoke();
            gameObject.SetActive(false);
        }*/

        /*IEnumerator DelayFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }*/
    }
}
