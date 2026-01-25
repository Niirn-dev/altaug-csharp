using AltAug.UI.Extensions;
using Avalonia.Controls;

using static LanguageExt.Prelude;

namespace AltAug.UI.Elements;

internal sealed class InputPromptControl
{
    private readonly Grid _root;
    private readonly TextBlock _promptTextBlock;
    private readonly TextBox _inputTextBox;
    private readonly Button _okButton;
    private readonly Button _cancelButton;

    public Grid Control { get => _root; }

    public InputPromptControl(string prompt, string defaultInput, Action<string> okAction, Action cancelAction)
    {
        // Initialize controls
        _root = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*, *"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto"),
        };

        _promptTextBlock = ControlsLibrary.MakeTitleTextBlock(prompt);

        _inputTextBox = new()
        {
            Height = 30,
            Text = defaultInput,
        };

        _okButton = ControlsLibrary.MakeFixedHeightButton("Confirm");
        _okButton.Click += (_, _) => okAction(_inputTextBox.Text ?? string.Empty);

        _cancelButton = ControlsLibrary.MakeFixedHeightButton("Cancel");
        _cancelButton.Click += (_, _) => cancelAction();

        // Define layout
        _root.AddControl(_promptTextBlock, row: 0, column: 0, rowSpan: None, columnSpan: 2);
        _root.AddControl(_inputTextBox, row: 1, column: 0, rowSpan: None, columnSpan: 2);
        _root.AddControl(_okButton, row: 2, column: 0);
        _root.AddControl(_cancelButton, row: 2, column: 1);
    }
}
