using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using wpfcm1.Preview;

namespace wpfcm1.FolderGroups
{
    public partial class FolderGroupView : UserControl
    {
        public FolderGroupView()
        {
            InitializeComponent();
        }

        private void Preview_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
            {
                PreviewColumn.Width = GridLength.Auto;
            }
            if ((bool) e.NewValue)
            {
                var cc = sender as ContentControl;
                var pv = cc.Content as PreviewView;
                FolderColumn.Width = new GridLength(60, GridUnitType.Star); ;
                PreviewColumn.Width = new GridLength(40, GridUnitType.Star); ;
                
                var pdfBrowser = pv.PdfBrowser;
                if (pdfBrowser == null) return;

                var fgvm = cc.DataContext as FolderGroupViewModel;
                var preview = fgvm.Preview;
                pdfBrowser.Navigate(preview.CurrentDocument);
            }
        }
    }
}
