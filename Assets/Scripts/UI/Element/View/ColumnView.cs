using Game.UI;
using Game.UI.Element;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Element.View
{
    [RequireComponent(typeof(HorizontalLayoutGroup))]
    public class ColumnView : UIElement
    {
        public ViewSO viewData;
        private HorizontalLayoutGroup horizontalLayoutGroup;

        protected override void Setup()
        {
            horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        protected override void Configure()
        {
            horizontalLayoutGroup.spacing = viewData.spacing;
            horizontalLayoutGroup.padding = viewData.padding;
        }
    }
}