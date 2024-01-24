using Hub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hub.Blacksmith
{
    public class DetailView:MonoBehaviour
    {
        [SerializeField] private GameObject windowParent;
        [SerializeField] private DetailViewWindow oldView;
        [SerializeField] private DetailViewWindow newView;
        [SerializeField] private TextMeshProUGUI skillCost;
        [SerializeField] private Button confirmUpgradeButton;
        [SerializeField] private Button backButton;

        private BlacksmithStoreView view;
        private BlacksmithStoreController controller;
        private UpgradableSkill currentSkills;

        public void Init(BlacksmithStoreController blacksmithStoreController,BlacksmithStoreView blacksmithStoreView)
        {
            controller = blacksmithStoreController;
            view = blacksmithStoreView;
            ResetCurrentSkill();
            confirmUpgradeButton.onClick.AddListener(delegate { view.ConfirmSkillUpgrade(currentSkills); });
            backButton.onClick.AddListener(delegate { HideDetailView(); });
        }
        
        public void ToggleDetailView(UpgradableSkill skill)
        {
            if ( skill != null && currentSkills != skill)
            {
                currentSkills = skill;
                ShowDetailView();
            }
            else
            {
                HideDetailView();
            }
        }

        public void ResetCurrentSkill()
        {
            currentSkills = null;
        }
        
        /// <summary>
        /// Open the detail view and set its info
        /// </summary>
        private void ShowDetailView()
        {
            //Updates skill info
            oldView.SetItem(currentSkills.oldSkill);
            newView.SetItem(currentSkills.newSkill);
            skillCost.text = currentSkills.cost+"G";
            
            //If the player has enough money, they can upgrade the skill
            confirmUpgradeButton.interactable=controller.GetPlayerBalance()>=currentSkills.cost;
            
            // Show the detail view
            windowParent.SetActive(true);
        }

        /// <summary>
        /// Hides the detail view
        /// </summary>
        public void HideDetailView()
        {
            print("HideDetailView()");
            ResetCurrentSkill();
            windowParent.SetActive(false);
        }
    }
}