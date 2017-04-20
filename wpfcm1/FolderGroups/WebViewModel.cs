using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using wpfcm1.DataAccess;
using wpfcm1.Events;
using wpfcm1.FolderTypes;
using wpfcm1.Preview;

namespace wpfcm1.FolderGroups
{
    public class WebViewModel : Screen, IDisposable
    {

        public WebViewModel(string name)
        {
            DisplayName = name;
  
        }

        public virtual void Dispose()
        {

        }
    }
}
