using SqliteExample.Views;

namespace SqliteExample
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PcDetailPage), typeof(PcDetailPage));
        }
    }
}
