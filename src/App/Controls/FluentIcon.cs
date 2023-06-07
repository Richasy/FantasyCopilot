// Copyright (c) Fantasy Copilot. All rights reserved.

namespace FantasyCopilot.App.Controls
{
    /// <summary>
    /// Fluent icon.
    /// </summary>
    public sealed class FluentIcon : FontIcon
    {
        /// <summary>
        /// <see cref="Symbol"/> 的依赖属性.
        /// </summary>
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register(nameof(Symbol), typeof(FluentSymbol), typeof(FluentIcon), new PropertyMetadata(default, OnSymbolChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentIcon"/> class.
        /// </summary>
        public FluentIcon()
            => FontFamily = new FontFamily("/Assets/FluentIcon.ttf#FluentSystemIcons-Resizable");

        /// <summary>
        /// 图标.
        /// </summary>
        public FluentSymbol Symbol
        {
            get => (FluentSymbol)GetValue(SymbolProperty);
            set => SetValue(SymbolProperty, value);
        }

        private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FluentSymbol symbol)
            {
                var icon = d as FluentIcon;
                icon.Glyph = ((char)symbol).ToString();
            }
        }
    }
}
