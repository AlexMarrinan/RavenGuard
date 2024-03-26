using Game.UI;
using Game.UI.Element;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Element.View
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public abstract class RowView : UIElement
    {
        public ViewSO viewData;
        private VerticalLayoutGroup verticalLayoutGroup;

        protected override void Setup()
        {
            verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        }

        protected override void Configure()
        {
            verticalLayoutGroup.spacing = viewData.spacing;
            verticalLayoutGroup.padding = viewData.padding;
        }
    }
}