using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace mEditor.ViewModels;

public partial class EditorAppearance : ObservableObject
{
    
    // NOTE: temp restructuring - TODO: Find better Structure later
    [ObservableProperty] public partial IBrush Background { get; set; } = Brush.Parse("#34343D");
    [ObservableProperty] public partial IBrush Foreground { get; set; } = Brush.Parse("#E4E4E7");
    [ObservableProperty] public partial IBrush BorderBrush { get; set; } = Brush.Parse("#34343D");
    [ObservableProperty] public partial Thickness BorderThickness { get; set; } = new(1);
    [ObservableProperty] public partial CornerRadius CornerRadius { get; set; } = new(10);
    [ObservableProperty] public partial Thickness Padding { get; set; } = new(18);
    [ObservableProperty] public partial double FontSize { get; set; } = 15;
    [ObservableProperty] public partial VerticalAlignment VerticalContentAlignment { get; set; } = VerticalAlignment.Top;
    [ObservableProperty] public partial IBrush SelectionBrush { get; set; } = Brush.Parse("#5145CD");
    [ObservableProperty] public partial IBrush CaretBrush { get; set; } = Brush.Parse("#A78BFA");
}