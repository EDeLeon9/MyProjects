using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RegistrarInventario
{
    public class VM_MainWindow : NotifyPropertyChangedBase
    {
        #region MEMBERS
        private ObservableCollection<TabData> _colTabData;
        private TabData _selectedTabData;
        #endregion

        #region PROPERTIES
        public RelayCommand CommandCerrarTab { get; set; }
        public ObservableCollection<TabData> ColTabData { get => _colTabData; set { _colTabData = value; NotifyPropertyChanged(); } }
        public TabData SelectedTabData { get => _selectedTabData; set { _selectedTabData = value; NotifyPropertyChanged(); } }
        #endregion


        public VM_MainWindow()
        {
            CommandCerrarTab = new RelayCommand(CerrarTab);
            ColTabData = new ObservableCollection<TabData>();
            TabManager.InitTabManager(new IngresarInventario(), ColTabData, () => SelectedTabData, (tabData) => SelectedTabData = tabData);
        }

        #region METHODS
        private void CerrarTab(object parameter)
        {
            TabManager.CloseTab((TabData)parameter);
        }
        #endregion
    }
}
