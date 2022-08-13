using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace RegistrarInventario
{
    public class TabData
    {
        public string TabTitle { get; set; }
        public Page FramePage { get; set; }
        public bool Closeable { get; set; } = true;
    }

    public static class TabManager
    {
        private static Func<TabData> GetSelectedTabData;
        private static Action<TabData> SetSelectedTabData;
        private static ObservableCollection<TabData> _colTabData;

        public static void InitTabManager(Page mainPage, ObservableCollection<TabData> tabDataCollection, Func<TabData> getSelectedTabData, Action<TabData> setSelectedTabData)
        {
            _colTabData = tabDataCollection;
            GetSelectedTabData = getSelectedTabData;
            SetSelectedTabData = setSelectedTabData;

            ShowTab((IUniqueTabPage)mainPage);
            _colTabData[^1].Closeable = false;  //First tab can't be closed
        }

        public static void ShowTab(IUniqueTabPage uniqueTabPage)
        {
            TabData foundTabData = _colTabData.Where(x => x.TabTitle == uniqueTabPage.TabTitle).FirstOrDefault();
            if (foundTabData == null)
            {
                uniqueTabPage.InitializePage();
                _colTabData.Add(new TabData() { TabTitle = uniqueTabPage.TabTitle, FramePage = (Page)uniqueTabPage });
                SetSelectedTabData.Invoke(_colTabData[^1]);
            }
            else
            {
                SetSelectedTabData.Invoke(foundTabData);
                (foundTabData.FramePage as IUniqueTabPage).ReselectItem(uniqueTabPage.InitialIdItem);
            }
        }

        public static void CloseTab(TabData tabData = null)
        {
            if (tabData == null)
                tabData = _colTabData.Where(x => x.TabTitle == GetSelectedTabData.Invoke()?.FramePage.Title).FirstOrDefault();

            if (tabData != null)
            {
                tabData.FramePage = null;
                _colTabData.Remove(tabData);
            }
        }
    }
}
