using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinRTHelper.SharingDataApi;

namespace SharingDataWinTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnShareText1(object sender, RoutedEventArgs e)
        {
            ShareIt it = new ShareIt(new WindowInteropHelper(this).Handle, "My text", "Share my text");

            it.Share(tb1.Text, ShareMode.Text);
        }

        private void OnShareImage1(object sender, RoutedEventArgs e)
        {
            ShareIt it = new ShareIt(new WindowInteropHelper(this).Handle, "My image", "Share my image");

            it.Share("image01.jpg", ShareMode.File);
        }
    }
}
