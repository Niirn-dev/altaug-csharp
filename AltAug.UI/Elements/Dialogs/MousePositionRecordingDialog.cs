using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using LanguageExt;

namespace AltAug.UI.Elements.Dialogs;

internal sealed class MousePositionRecordingDialog
{
    private const string DialogTitle = "Select point...";
    private const string InstructionsTextFirst = @"Hover cursor over the ";
    private const string InstructionTextLast = @" and press 'E' to capture position.
Press 'Q' or 'Esc' to cancel";

    private readonly IAutomationService _automationService;

    private readonly StackPanel _root;
    private readonly TextBlock _instructionsTextBox;

    private ContentDialog _dialog = new();

    public MousePositionRecordingDialog(IAutomationService automationService)
    {
        _automationService = automationService;

        // Initialize controls
        _root = new()
        {
            Orientation = Orientation.Vertical,
        };

        _instructionsTextBox = ControlsLibrary.MakeVariableHeightTextBlock(text: string.Empty);

        // Define layout
        _root.Children.Add(_instructionsTextBox);
    }

    public async Task<Option<Vec2>> ShowDialogAsync(string hoverTarget)
    {
        _dialog.Content = null;

        _instructionsTextBox.Inlines =
        [
            new Run { Text = InstructionsTextFirst },
            new Run { Text = hoverTarget, FontWeight = FontWeight.Bold },
            new Run { Text = InstructionTextLast },
        ];
        _dialog = new()
        {
            Title = DialogTitle,
            Content = _root,
        };

        var result = Option<Vec2>.None;
        _dialog.Opened += async (_, _) =>
        {
            result = await _automationService.RecordMousePositionAsync().ConfigureAwait(false);

            Dispatcher.UIThread.Post(() => _dialog.Hide());
        };

        await _dialog.ShowAsync();

        return result;
    }
}
