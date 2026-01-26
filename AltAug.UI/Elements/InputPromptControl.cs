using AltAug.Domain.Extensions;
using AltAug.UI.Extensions;
using Avalonia.Controls;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AltAug.UI.Elements;

internal sealed class InputPromptControl
{
    private readonly Grid _root;
    private readonly TextBlock _promptTextBlock;
    private readonly TextBox _primaryTextBox;
    private readonly Option<TextBox> _secondaryTextBox;
    private readonly Button _okButton;
    private readonly Button _cancelButton;

    public Grid Control { get => _root; }

    public InputPromptControl(string prompt, string defaultPrimary, Action<string> okAction, Action cancelAction)
        : this(prompt, defaultPrimary, cancelAction)
    {
        // Initialize controls
        _okButton.Click += (_, _) => okAction(_primaryTextBox.Text ?? string.Empty);

        DefineLayout();
    }

    public InputPromptControl(string prompt, string defaultPrimary, string defaultSecondary, Action<string, string> okAction, Action cancelAction)
        : this(prompt, defaultPrimary, cancelAction)
    {
        // Initialize controls
        _secondaryTextBox = ControlsLibrary.MakeTextBox(text: defaultSecondary);

        _okButton.Click += (_, _) => okAction(
            _primaryTextBox.Text ?? string.Empty,
            _secondaryTextBox
                .Map(s => s.Text.ToOpt())
                .Flatten()
                .IfNone(() => string.Empty));

        DefineLayout();
    }

    private InputPromptControl(string prompt, string defaultPrimary, Action cancelAction)
    {
        // Initialize controls
        _root = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*, *"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto, Auto"),
        };

        _promptTextBlock = ControlsLibrary.MakeTitleTextBlock(prompt);

        _primaryTextBox = ControlsLibrary.MakeTextBox(text: defaultPrimary);

        _okButton = ControlsLibrary.MakeFixedHeightButton("Confirm");

        _cancelButton = ControlsLibrary.MakeFixedHeightButton("Cancel");
        _cancelButton.Click += (_, _) => cancelAction();
    }

    private void DefineLayout()
    {
        _root.AddControl(_promptTextBlock, row: 0, column: 0, rowSpan: None, columnSpan: 2);
        _root.AddControl(_primaryTextBox, row: 1, column: 0, rowSpan: None, columnSpan: 2);

        _secondaryTextBox.IfSome(secondary => _root.AddControl(secondary, row: 2, column: 0, rowSpan: None, columnSpan: 2));

        _root.AddControl(_okButton, row: 3, column: 0);
        _root.AddControl(_cancelButton, row: 3, column: 1);
    }
}
