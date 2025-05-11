using System.Collections.Generic;
using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using System.ComponentModel;

namespace JDFixer.UI
{
    internal sealed class RTPreferencesListViewController : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "JDFixer.UI.BSML.rtPreferencesList.bsml";


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


        [UIValue("min_rt_slider")]
        private float Min_RT_Slider => PluginConfig.Instance.minReactionTime;
        [UIValue("max_rt_slider")]
        private float Max_RT_Slider => PluginConfig.Instance.maxReactionTime;

        [UIComponent("rt_slider")]
        private SliderSetting RT_Slider;

        private float New_RT_Value = 500f;

        [UIValue("rt_value")]
        private float RT_Value
        {
            get => New_RT_Value;
            set
            {
                New_RT_Value = value;
            }
        }
        [UIAction("set_rt_value")]
        private void Set_RT_Value(float value)
        {
            RT_Value = value;
        }

        [UIAction("rt_slider_formatter")]
        private string RT_Slider_Formatter(float value) => value.ToString("0") + " ms";


        [UIComponent("pref_list")]
        private CustomListTableData Pref_List;
        private RTPref Selected_Pref = null;

        [UIAction("select_pref")]
        private void Select_Pref(TableView tableView, int row)
        {
            Selected_Pref = PluginConfig.Instance.preferredValues_rt[PluginConfig.Instance.use_rt_pref][row];
        }


        [UIAction("add_pressed")]
        private void Add_Pressed()
        {
            if (PluginConfig.Instance.preferredValues_rt[PluginConfig.Instance.use_rt_pref].Any(x => x.njs == New_NJS_Value))
            {
                PluginConfig.Instance.preferredValues_rt[PluginConfig.Instance.use_rt_pref].RemoveAll(x => x.njs == New_NJS_Value);
            }
            PluginConfig.Instance.preferredValues_rt[PluginConfig.Instance.use_rt_pref].Add(new RTPref(New_NJS_Value, New_RT_Value));
            Reload_List_From_Config();
        }


        [UIAction("remove_pressed")]
        private void Remove_Pressed()
        {
            if (Selected_Pref == null)
                return;

            PluginConfig.Instance.preferredValues_rt[PluginConfig.Instance.use_rt_pref].RemoveAll(x => x == Selected_Pref);
            Reload_List_From_Config();
        }


        private void Reload_List_From_Config()
        {
            Pref_List.Data.Clear();

            var index = PluginConfig.Instance.use_rt_pref;
            
            if (PluginConfig.Instance.preferredValues_rt == null)
                return;
            
            // First element to start with
            if (PluginConfig.Instance.preferredValues_rt.Count == 0)
            {
                PluginConfig.Instance.preferredValues_rt.Add(new List<RTPref>());
            }

            // Handle out of bounds config
            if (PluginConfig.Instance.preferredValues_rt.Count <= index)
            {
                PluginConfig.Instance.use_rt_pref = PluginConfig.Instance.preferredValues_rt.Count-1;
            }

            PluginConfig.Instance.preferredValues_rt[index].Sort((x, y) => y.njs.CompareTo(x.njs));
            
            foreach (var pref in PluginConfig.Instance.preferredValues_rt[index])
            {
                Pref_List.Data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.reactionTime} ms"));
            }

            Pref_List.TableView.ReloadData();
            Pref_List.TableView.ClearSelection();
            Selected_Pref = null;
        }


        //----------------------------------------------------------------------------

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
