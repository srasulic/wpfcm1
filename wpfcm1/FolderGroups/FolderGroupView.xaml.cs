using System.Windows;
using System.Windows.Controls;
using wpfcm1.Preview;

namespace wpfcm1.FolderGroups
{
    public partial class FolderGroupView : UserControl
    {
        private bool _firstPass;

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
                FolderColumn.Width = new GridLength(60, GridUnitType.Star); ;
                PreviewColumn.Width = new GridLength(40, GridUnitType.Star); ;

                //TODO: ovo je uzasno ali trenutno ne znam kako drugacije
                // ovoje potrebno samo kada browser postane vidljiv po prvi put, 
                //tad Handle(MessageShowPdf) ne radi jer pdfBrowser jos uvek nije vidljiv
                if (!_firstPass)
                {
                    var cc = sender as ContentControl;
                    var pv = cc.Content as PreviewView;
                    var pdfBrowser = pv.PdfBrowser;
                    if (pdfBrowser == null) return;

                    var fgvm = cc.DataContext as FolderGroupViewModel;
                    var preview = fgvm.Preview;
                    pdfBrowser.Navigate(new System.Uri(preview.CurrentDocument));
                    _firstPass = true;
                    
                }
            }
        }
    }
}
