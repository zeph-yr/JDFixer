using System.Linq;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Components.Settings;
using System.ComponentModel;

namespace JDFixer.UI
{
    public class PreferencesListViewController : BSMLResourceViewController, INotifyPropertyChanged
    {
        public override string ResourceName => "JDFixer.UI.BSML.preferencesList.bsml";
        //public event PropertyChangedEventHandler PropertyChanged;


        [UIComponent("njs_slider")]
        private SliderSetting NJS_Slider;

        private float _new_njs = 16f;

        [UIValue("njs_value")]
        public float NJS_Value
        {
            get => _new_njs;
            set
            {
                _new_njs = value;
            }
        }
        [UIAction("set_njs")]
        void Set_NJS(float value)
        {
            NJS_Value = value;
        }


        [UIValue("min_jd_slider")]
        private int Min_JD_Slider => PluginConfig.Instance.minJumpDistance;
        [UIValue("max_jd_slider")]
        private int Max_JD_Slider => PluginConfig.Instance.maxJumpDistance;

        [UIComponent("jd_slider")]
        private SliderSetting JD_Slider;

        private float _new_jd = 23f;

        [UIValue("jd_value")]
        public float JD_Value
        {
            get => _new_jd;
            set
            {
                _new_jd = value;
            }
        }

        [UIAction("set_jd")]
        void Set_JD(float value)
        {
            JD_Value = value;
        }


        [UIComponent("pref_list")]
        public CustomListTableData prefList;
        private JDPref _selectedPref = null;

        /*[UIValue("prefIsSelected")]
        public bool prefIsSelected
        {
            get => _selectedPref != null;
            set
            {
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
            }
        }*/

        /*[UIComponent("leftButton")]
        private RectTransform leftButton;
        
        [UIComponent("rightButton")]
        private RectTransform rightButton;*/

        [UIAction("select_pref")]
        private void Select_Pref(TableView tableView, int row)
        {
            //Logger.log.Debug("Selected row " + row);

            _selectedPref = PluginConfig.Instance.preferredValues[row];
            //prefIsSelected = prefIsSelected;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
        }


        [UIAction("add_pressed")]
        private void Add_Pressed()
        {
            if (PluginConfig.Instance.preferredValues.Any(x => x.njs == _new_njs))
            {
                PluginConfig.Instance.preferredValues.RemoveAll(x => x.njs == _new_njs);
            }
            PluginConfig.Instance.preferredValues.Add(new JDPref(_new_njs, _new_jd));
            Reload_List_From_Config();
        }


        [UIAction("remove_pressed")]
        private void Remove_Pressed()
        {
            if (_selectedPref == null)
            {
                return;
            }
            PluginConfig.Instance.preferredValues.RemoveAll(x => x == _selectedPref);
            Reload_List_From_Config();
        }


        private void Reload_List_From_Config()
        {
            prefList.data.Clear();

            if (PluginConfig.Instance.preferredValues == null)
            {
                return;
            }

            PluginConfig.Instance.preferredValues.Sort((x, y) => y.njs.CompareTo(x.njs));

            foreach (var pref in PluginConfig.Instance.preferredValues)
            {
                prefList.data.Add(new CustomListTableData.CustomCellInfo($"{pref.njs} NJS | {pref.jumpDistance} Jump Distance"));
            }

            prefList.tableView.ReloadData();
            prefList.tableView.ClearSelection();
            _selectedPref = null;
            //prefIsSelected = prefIsSelected;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(prefIsSelected)));
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