using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;

namespace OsuScoreCheck.Controls.ControlsClasses
{
    public class StretchingStackPanel : Panel
    {
        public static readonly StyledProperty<double> SpacingProperty =
            AvaloniaProperty.Register<StretchingStackPanel, double>(nameof(Spacing), 0);

        public static readonly StyledProperty<OverflowBehavior> OverflowBehaviorProperty =
            AvaloniaProperty.Register<StretchingStackPanel, OverflowBehavior>(nameof(OverflowBehavior), OverflowBehavior.MultiRow);

        public double Spacing
        {
            get => GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public OverflowBehavior OverflowBehavior
        {
            get => GetValue(OverflowBehaviorProperty);
            set => SetValue(OverflowBehaviorProperty, value);
        }

        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        protected override Size MeasureOverride(Size availableSize)
        {
            double totalWidth = 0, totalHeight = 0, rowHeight = 0;
            bool isSingleRow = true;

            foreach (var child in Children)
            {
                child.Measure(availableSize);

                if (Orientation == Orientation.Horizontal)
                {
                    bool needsNewRow = totalWidth + child.DesiredSize.Width > availableSize.Width;

                    if ((OverflowBehavior == OverflowBehavior.MultiRow || OverflowBehavior == OverflowBehavior.Adaptive && !isSingleRow) && needsNewRow)
                    {
                        totalHeight += rowHeight + Spacing;
                        totalWidth = 0;
                        rowHeight = 0;
                    }

                    if (OverflowBehavior == OverflowBehavior.Adaptive && isSingleRow && needsNewRow)
                    {
                        isSingleRow = false;
                        totalHeight += rowHeight + Spacing;
                        totalWidth = 0;
                        rowHeight = 0;
                    }

                    totalWidth += child.HorizontalAlignment == HorizontalAlignment.Stretch && OverflowBehavior == OverflowBehavior.MultiRow
                        ? availableSize.Width
                        : child.DesiredSize.Width + Spacing;

                    rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
                }
            }

            totalHeight += rowHeight;

            return new Size(availableSize.Width, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double currentX = 0, currentY = 0, rowHeight = 0, totalStretchWidth = 0, totalNonStretchWidth = 0;
            int stretchCount = 0;
            bool isSingleRow = true;

            foreach (var child in Children)
            {
                if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    totalStretchWidth += child.DesiredSize.Width;
                    stretchCount++;
                }
                else
                    totalNonStretchWidth += child.DesiredSize.Width;
            }

            double remainingWidth = finalSize.Width - totalNonStretchWidth - (Children.Count - 1) * Spacing;
            double stretchWidth = stretchCount > 0 ? Math.Max(0, remainingWidth / stretchCount) : 0;

            foreach (var child in Children)
            {
                double childWidth = child.DesiredSize.Width;

                if (child.HorizontalAlignment == HorizontalAlignment.Stretch)
                {
                    childWidth = OverflowBehavior switch
                    {
                        OverflowBehavior.Adaptive => child is Control control
                            ? Math.Clamp(control.DesiredSize.Width, stretchWidth, finalSize.Width)
                            : stretchWidth,
                        OverflowBehavior.MultiRow => finalSize.Width,
                        _ => stretchWidth
                    };
                }

                if (currentX + childWidth + Spacing > finalSize.Width &&
                    (OverflowBehavior == OverflowBehavior.MultiRow || OverflowBehavior == OverflowBehavior.Adaptive && !isSingleRow))
                {
                    currentY += rowHeight + Spacing;
                    currentX = 0;
                    rowHeight = 0;
                }

                if (OverflowBehavior == OverflowBehavior.Adaptive && isSingleRow && currentX + childWidth > finalSize.Width)
                {
                    isSingleRow = false;
                    currentY += rowHeight + Spacing;
                    currentX = 0;
                    rowHeight = 0;
                }

                child.Arrange(new Rect(currentX, currentY, childWidth, child.DesiredSize.Height));

                currentX += childWidth + (currentX + childWidth + Spacing <= finalSize.Width ? Spacing : 0);
                rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
            }

            return new Size(finalSize.Width, currentY + rowHeight);
        }
    }

    public enum OverflowBehavior
    {
        SingleRow,  // Всё в одной строке
        MultiRow,   // Элементы могут переноситься на новую строку
        Adaptive    // Элементы растягиваются, смещая остальные
    }
}
