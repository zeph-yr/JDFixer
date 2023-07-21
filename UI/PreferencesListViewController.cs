using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using System.ComponentModel;

namespace JDFixer.UI
{
    internal sealed class PreferencesListViewController : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "JDFixer.UI.BSML.preferencesList.bsml";


        [UIComponent("njs_slider")]
        private SliderSetting NJS_Slider;

        private float New_NJS_Value = 16f;

        [UIValue("njs_value")]
        private float NJS_Value
        {
            get => New_NJS_Value;
            set
            {
                New_NJS_Value = value;
            }
        }
        [UIAction("set_njs_value")]
        private void Set_NJS_Value(float value)
        {
            NJS_Value = value;
        }


        [UIValue("min_jd_slider")]
        private float Min_JD_Slider => PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private float Max_JD_Slider => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        private SliderSetting JD_Slider;

        private float New_JD_Value = 18f;

        [UIValue("jd_value")]
        private float JD_Value
        {
            get => New_JD_Value;
            set
            {
                New_JD_Value = value;
            }
        }
        [UIAction("set_jd_value")]
        private void Set_JD_Value(float value)
        {
            JD_Value = value;
        }


        [UIComponent("pref_list")]
        private CustomListTableData Pref_List;
        private JDPref Selected_Pref = null;


        [UIAction("select_pref")]
        private void Select_Pref(TableView tableView, int row)
        {
            Selected_Pref = PluginConfig.Instance.preferredValues[row];
        }


        [UIAction("add_pressed")]
        private void Add_Pressed()
        {
            if (PluginConfig.Instance.preferredValues.Any(x => x.njs == New_NJS_Value))
            {
                PluginConfig.Instance.preferredValues.RemoveAll(x => x.njs == New_NJS_Value);
            }
            PluginConfig.Instance.preferredValues.Add(new JDPref(New_NJS_Value, New_JD_Value));
            Reload_List_From_Config();
        }


        [UIAction("remove_pressed")]
        private void Remove_Pressed()
        {
            if (Selected_Pref == null)
            {
                return;
            }
            PluginConfig.Instance.preferredValues.RemoveAll(x => x == Selected_Pref);
            Reload_List_From_Config();
        }


        private void Reload_List_From_Config()
        {
            Pref_List.data.Clear();

            if (PluginConfig.Instance.preferredValues == null)
            {
                return;
            }

            PluginConfig.Instance.preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

            foreach (var pref in PluginConfig.Instance.preferredValues)
            {
                Pref_List.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.jumpDistance} Jump Distance"));
            }

            Pref_List.tableView.ReloadData();
            Pref_List.tableView.ClearSelection();
            Selected_Pref = null;
        }


        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (!firstActivation)
            {
                Reload_List_From_Config();
            }
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }


        [UIAction("#post-parse")]
        private void PostParse()
        {
            Reload_List_From_Config();
        }
    }
}