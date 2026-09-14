using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Taadol.Controls
{

    public static class NumberFontBehavior
    {
        private const string ProcessedMarker = "NumberFontBehavior.Processed";

        public static readonly DependencyProperty ApplyProperty =
            DependencyProperty.RegisterAttached(
                "Apply",
                typeof(bool),
                typeof(NumberFontBehavior),
                new PropertyMetadata(false, OnApplyChanged));

        public static void SetApply(DependencyObject obj, bool value) => obj.SetValue(ApplyProperty, value);
        public static bool GetApply(DependencyObject obj) => (bool)obj.GetValue(ApplyProperty);

        private static readonly Regex NumberPattern =
            new Regex(@"[\d۰-۹]+(?:[\/\.\-,:٬][\d۰-۹]+)*", RegexOptions.Compiled);

        private static FontFamily _iranSans;
        private static FontFamily _vazir;

        private static FontFamily IranSans =>
            _iranSans ??= ResolveFont("IRANSans", "pack://application:,,,/Fonts/IRANSans/#IRANSansXFaNum");

        private static FontFamily Vazir =>
            _vazir ??= ResolveFont("Vazir", "pack://application:,,,/Fonts/Vazir/#Vazir");

        private static readonly ConditionalWeakTable<TextBlock, string> _appliedText = new();

        private static FontFamily ResolveFont(string key, string fallbackUri)
        {
            if (Application.Current?.TryFindResource(key) is FontFamily ff)
                return ff;
            return new FontFamily(fallbackUri);
        }

        private static void OnApplyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock && (bool)e.NewValue)
            {

                var binding = textBlock.GetBindingExpression(TextBlock.TextProperty)?.ParentBinding;

                textBlock.Loaded += (_, _) => ApplyFont(textBlock);
                textBlock.IsVisibleChanged += (_, _) =>
                {
                    if (textBlock.IsVisible) ApplyFont(textBlock);
                };

                var descriptor = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
                descriptor.AddValueChanged(textBlock, (_, _) => ApplyFont(textBlock));

                if (binding != null)
                {
                    textBlock.DataContextChanged += (_, _) =>
                    {

                        if (textBlock.Inlines.Count == 0) return;

                        textBlock.Inlines.Clear();
                        textBlock.SetBinding(TextBlock.TextProperty, binding);

                        textBlock.Dispatcher.BeginInvoke(new Action(() => ApplyFont(textBlock)),
                            System.Windows.Threading.DispatcherPriority.DataBind);
                    };
                }

                ApplyFont(textBlock);
            }
        }

        private static void ApplyFont(TextBlock textBlock)
        {
            if (textBlock.Tag is bool isUpdating && isUpdating) return;
            if (HasCustomInlines(textBlock)) return;

            var text = textBlock.Text;
            if (string.IsNullOrEmpty(text)) return;

            if (textBlock.Inlines.Count > 0 &&
                _appliedText.TryGetValue(textBlock, out var applied) &&
                applied == text)
                return;

            textBlock.Tag = true;
            try
            {
                textBlock.Inlines.Clear();

                int last = 0;
                foreach (Match m in NumberPattern.Matches(text))
                {
                    if (m.Index > last)
                        AddRun(textBlock, text.Substring(last, m.Index - last), IranSans);

                    AddRun(textBlock, ConvertToPersianDigits(m.Value), Vazir);
                    last = m.Index + m.Length;
                }

                if (last < text.Length)
                    AddRun(textBlock, text.Substring(last), IranSans);
            }
            finally
            {
                textBlock.Tag = false;
            }

            _appliedText.Remove(textBlock);
            _appliedText.Add(textBlock, text);
        }

        private static void AddRun(TextBlock textBlock, string content, FontFamily family)
        {

            textBlock.Inlines.Add(new Run(content)
            {
                FontFamily = family,
                Tag = ProcessedMarker
            });
        }

        private static bool HasCustomInlines(TextBlock textBlock)
        {
            foreach (var inline in textBlock.Inlines)
            {
                if (inline.Tag is string marker && marker == ProcessedMarker)
                    continue;

                if (inline is Run run)
                {
                    if (run.ReadLocalValue(TextElement.ForegroundProperty) != DependencyProperty.UnsetValue) return true;
                    if (run.ReadLocalValue(TextElement.FontFamilyProperty) != DependencyProperty.UnsetValue) return true;
                    if (run.ReadLocalValue(TextElement.FontWeightProperty) != DependencyProperty.UnsetValue) return true;
                    if (run.ReadLocalValue(TextElement.FontStyleProperty) != DependencyProperty.UnsetValue) return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private static string ConvertToPersianDigits(string input)
        {
            var result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= '0' && c <= '9')
                    result[i] = (char)('۰' + (c - '0'));
                else
                    result[i] = c;
            }
            return new string(result);
        }
    }
}
