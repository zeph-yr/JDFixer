using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine;
using BeatSaberMarkupLanguage.Components.Settings;

namespace JDFixer.UI
{
    internal class PreferencesListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "JDFixer.UI.BSML.preferencesList.bsml";

        [UIValue("minJump")]
        private int minJump => PluginConfig.Instance.minJumpDistance;
        [UIValue("maxJump")]
        private int maxJump => PluginConfig.Instance.maxJumpDistance;


        [UIComponent("prefList")]
        public CustomListTableData prefList;
        private JDPref _selectedPref = null;
        [UIValue("prefIsSelected")]
        public bool prefIsSelected
        {
            get => _selectedPref != null;
            set
            {
                NotifyPropertyChanged();
            }
        }


        [UIComponent("leftButton")]
        private RectTransform leftButton;
        [UIComponent("rightButton")]
        private RectTransform rightButton;


        [UIComponent("njsSlider")]
        private SliderSetting njsSlider;

        private float _newNjs = 16f;
        [UIValue("njsValue")]
        public float njsValue
        {
            get => _newNjs;
            set
            {
                _newNjs = value;
            }
        }
        [UIAction("setNjs")]
        void SetNjs(float value)
        {
            njsValue = value;
        }

        [UIComponent("jdSlider")]
        private SliderSetting jdSlider;

        private float _newJumpDis = 23f;
        [UIValue("jumpDisValue")]
        public float jumpDisValue
        {
            get => _newJumpDis;
            set
            {
                _newJumpDis = value;
            }
        }
        [UIAction("setJumpDis")]
        void SetJumpDis(float value)
        {
            jumpDisValue = value;
        }
        
        [UIAction("prefSelect")]
        private void SelectedPref(TableView tableView, int row)
        {
            Logger.log.Debug("Selected row " + row);
            _selectedPref = PluginConfig.Instance.preferredValues[row];
            prefIsSelected = prefIsSelected;
        }
        [UIAction("addPressed")]
        private void AddNewValue()
        {
            if(PluginConfig.Instance.preferredValues.Any(x => x.njs == _newNjs))
            {
                PluginConfig.Instance.preferredValues.RemoveAll(x => x.njs == _newNjs);
            }
            PluginConfig.Instance.preferredValues.Add(new JDPref(_newNjs, _newJumpDis));
            //Config.Write();
            ReloadListFromConfig();
        }
        [UIAction("removePressed")]
        private void RemoveSelectedPref()
        {
            if (_selectedPref == null) return;
            PluginConfig.Instance.preferredValues.RemoveAll(x => x == _selectedPref);
            //Config.Write();
            ReloadListFromConfig();
        }
        private void ReloadListFromConfig()
        {
            prefList.data.Clear();

            if (PluginConfig.Instance.preferredValues == null)
                return;

            PluginConfig.Instance.preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

            foreach (var pref in PluginConfig.Instance.preferredValues)
            {
                prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.jumpDistance} Jump Distance"));
            }
            prefList.tableView.ReloadData();
            prefList.tableView.ClearSelection();
            _selectedPref = null;
            prefIsSelected = prefIsSelected;
        }
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (!firstActivation)
            {
                ReloadListFromConfig();
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            ReloadListFromConfig();
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), njsSlider, 1f);
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), jdSlider, 0.1f);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }
    }
}
