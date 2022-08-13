using System;
using System.Collections.Generic;
using System.Text;

namespace RegistrarInventario
{
    public interface IUniqueTabPage
    {
        string TabTitle { get; set; }
        int InitialIdItem { get; set; }
        Action<int> UpdateForeignItem { get; set; }

        void ReselectItem(int idItem);
        void InitializePage();
    }
}
