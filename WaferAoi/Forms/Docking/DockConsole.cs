using YiNing.UI.Controls;
using YiNing.UI.Docking;

namespace WaferAoi
{
    public partial class DockConsole : DarkToolWindow
    {
        #region Constructor Region

        public DockConsole()
        {
            InitializeComponent();

            // Build dummy list data
            for (var i = 0; i < 100; i++)
            {
                var item = new DarkListItem($"List item #{i}");
                lstConsole.Items.Add(item);
            }
        }

        #endregion
    }
}
