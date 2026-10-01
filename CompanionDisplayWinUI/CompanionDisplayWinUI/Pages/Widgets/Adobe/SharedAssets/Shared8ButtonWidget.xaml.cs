using CompanionDisplayWinUI.API;
using CompanionDisplayWinUI.Pages.Widgets.Twitch.SharedAssets;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CompanionDisplayWinUI.Pages.Widgets.Adobe.SharedAssets
{
    // [ContentProperty] allows you to drop strings directly inside the control in XAML
    [ContentProperty(Name = nameof(ButtonContents))]
    public sealed partial class Shared8ButtonWidget : UserControl
    {
        public Shared8ButtonWidget()
        {
            // Initialize the list right away so XAML can immediately .Add() to it
            ButtonContents = new List<string>();
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonContentsProperty =
            DependencyProperty.Register(
                nameof(ButtonContents),
                typeof(List<string>),
                typeof(Shared8ButtonWidget),
                new PropertyMetadata(null, OnButtonContentsChanged));

        public List<string> ButtonContents
        {
            get => (List<string>)GetValue(ButtonContentsProperty);
            set => SetValue(ButtonContentsProperty, value);
        }
        public static readonly DependencyProperty ButtonTagsProperty =
            DependencyProperty.Register(
                nameof(ButtonTags),
                typeof(List<string>),
                typeof(Shared8ButtonWidget),
                new PropertyMetadata(null, OnButtonTagsChanged));

        public List<string> ButtonTags
        {
            get => (List<string>)GetValue(ButtonTagsProperty);
            set => SetValue(ButtonTagsProperty, value);
        }
        public static readonly DependencyProperty ButtonModifierProperty =
            DependencyProperty.Register(
                nameof(ButtonModifier),
                typeof(List<bool>),
                typeof(Shared8ButtonWidget),
                new PropertyMetadata(null, OnButtonModifiersChanged));

        public List<bool> ButtonModifier
        {
            get => (List<bool>)GetValue(ButtonModifierProperty);
            set => SetValue(ButtonModifierProperty, value);
        }

        private static void OnButtonContentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Shared8ButtonWidget widget && e.NewValue is List<string> stringList)
            {
                for (int i = 0; i < stringList.Count; i++)
                {
                    widget.SetButtonContent(i, stringList[i]);
                }
            }
        }
        private static void OnButtonModifiersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Shared8ButtonWidget widget && e.NewValue is List<bool> stringList)
            {
                for (int i = 0; i < stringList.Count; i++)
                {
                    widget.SetButtonModifier(i, stringList[i]);
                }
            }
        }
        private static void OnButtonTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Shared8ButtonWidget widget && e.NewValue is List<string> stringList)
            {
                for (int i = 0; i < stringList.Count; i++)
                {
                    widget.SetButtonTag(i, stringList[i]);
                }
            }
        }
        private Button GetButtonFromPos(int position)
        {
            int row = position / 2;
            int pos = position % 2;
            return (Button)((StackPanel)ButtonHost.Children[row]).Children[pos];
        }

        private void SetButtonContent(int position, string content)
        {
            GetButtonFromPos(position).Content = content;
        }

        private void SetButtonTag(int position, string tag)
        {
            GetButtonFromPos(position).Tag = tag;
        }

        private void SetButtonModifier(int position, bool enableCTRL)
        {
            GetButtonFromPos(position).Tapped += enableCTRL ? PressKeyCTRL : PressKeyNoModifiers;
        }

        protected void PressKeyCTRL(object sender, TappedRoutedEventArgs _1)
        {
            KeyPressAPI.CallKeys(int.Parse((string)(sender as Button).Tag), 17);
        }

        protected void PressKeyNoModifiers(object sender, TappedRoutedEventArgs _1)
        {
            KeyPressAPI.CallKeys(int.Parse((string)(sender as Button).Tag), -1);
        }
    }
}