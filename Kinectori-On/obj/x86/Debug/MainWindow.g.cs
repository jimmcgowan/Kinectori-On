﻿#pragma checksum "..\..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "BC156B95E220A70949CB766AC5B2F5AC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Kinectori_On {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel KinectoriBackground;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel TopToolBar;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ControlAreaLayoutGrid;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SampleControlsGrid;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid MetronomeGrid;
        
        #line default
        #line hidden
        
        
        #line 145 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid ControlGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Kinectori-On;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.KinectoriBackground = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.TopToolBar = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 4:
            
            #line 16 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Primitives.RepeatButton)(target)).Click += new System.Windows.RoutedEventHandler(this.IncreaseBPM);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 19 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Primitives.RepeatButton)(target)).Click += new System.Windows.RoutedEventHandler(this.DecreaseBPM);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 24 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Reset);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 25 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Stop);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 26 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Start);
            
            #line default
            #line hidden
            return;
            case 9:
            this.ControlAreaLayoutGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 10:
            this.SampleControlsGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 11:
            
            #line 67 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 70 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 73 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 76 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 79 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 16:
            
            #line 82 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 85 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 88 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 91 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 94 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 21:
            
            #line 97 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 22:
            
            #line 100 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 103 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 106 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 25:
            
            #line 109 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 112 "..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.LoadSample);
            
            #line default
            #line hidden
            return;
            case 27:
            this.MetronomeGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 28:
            this.ControlGrid = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

