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
    internal class RTPreferencesListViewController : BSMLResourceViewController
    {
        public override string ResourceName => "JDFixer.UI.BSML.rtPreferencesList.bsml";

        [UIValue("minRT")]
        private int minRT => PluginConfig.Instance.minReactionTime;
        [UIValue("maxRT")]
        private int maxRT => PluginConfig.Instance.maxReactionTime;


        [UIComponent("prefList")]
        public CustomListTableData prefList;
        private RTPref _selectedPref = null;
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

        [UIComponent("rtSlider")]
        private SliderSetting rtSlider;

        private float _newReactionTime = 500f;
        [UIValue("reactionTimeValue")]
        public float reactionTimeValue
        {
            get => _newReactionTime;
            set
            {
                _newReactionTime = value;
            }
        }
        [UIAction("setReactionTime")]
        void SetReactionTime(float value)
        {
            reactionTimeValue = value;
        }

        [UIAction("prefSelect")]
        private void SelectedPref(TableView tableView, int row)
        {
            Logger.log.Debug("Selected row " + row);
            _selectedPref = PluginConfig.Instance.rt_preferredValues[row];
            prefIsSelected = prefIsSelected;
        }
        [UIAction("addPressed")]
        private void AddNewValue()
        {
            if (PluginConfig.Instance.rt_preferredValues.Any(x => x.njs == _newNjs))
            {
                PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x.njs == _newNjs);
            }
            PluginConfig.Instance.rt_preferredValues.Add(new RTPref(_newNjs, _newReactionTime));
            //Config.Write();
            ReloadListFromConfig();
        }
        [UIAction("removePressed")]
        private void RemoveSelectedPref()
        {
            if (_selectedPref == null) return;
            PluginConfig.Instance.rt_preferredValues.RemoveAll(x => x == _selectedPref);
            //Config.Write();
            ReloadListFromConfig();
        }
        private void ReloadListFromConfig()
        {
            prefList.data.Clear();

            if (PluginConfig.Instance.rt_preferredValues == null)
                return;

            PluginConfig.Instance.rt_preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

            foreach (var pref in PluginConfig.Instance.rt_preferredValues)
            {
                prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.reactionTime} ms"));
            }
            prefList.tableView.ReloadData();
            prefList.tableView.ClearSelection();
            _selectedPref = null;
            prefIsSelected = prefIsSelected;
        }


        //----------------------------------------------------------------------------
        // Reusing Unchanged
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
            SliderButton.Register(GameObject.Instantiate(leftButton), GameObject.Instantiate(rightButton), rtSlider, 1f);
            GameObject.Destroy(leftButton.gameObject);
            GameObject.Destroy(rightButton.gameObject);
        }
    }
}
