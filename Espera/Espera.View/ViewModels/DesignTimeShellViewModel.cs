﻿using System.Collections.Generic;
using System.Reactive.Linq;
using Caliburn.Micro;
using Espera.Core.Mobile;
using Espera.Core.Settings;

namespace Espera.View.ViewModels
{
    internal class DesignTimeShellViewModel : ShellViewModel
    {
        public DesignTimeShellViewModel()
            : base(DesignTime.LoadLibrary(), new ViewSettings(), new CoreSettings(), new WindowManager(),
                new MobileApiInfo(Observable.Return(new List<MobileClient>()), Observable.Return(false)))
        { }
    }
}