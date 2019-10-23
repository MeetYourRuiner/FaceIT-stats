﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace faceitwpf
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Contains("-updated"))
            {
                System.Diagnostics.Process.Start("CMD.exe", "/C DEL old.exe");
            }
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}