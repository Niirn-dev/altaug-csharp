using AltAug.Domain.Helpers;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Extensions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using FluentAvalonia.UI.Controls;
using MoreLinq;

namespace AltAug.UI.Elements.Dialogs;

internal sealed class RegexLibraryDialog
{
    private const string DialogTitle = "RegEx Library";

    private readonly IStateManager<RegexLibraryStore> _libraryManager;
    private readonly Grid _root;

    private readonly ComboBox _itemTypeCombo;
    private readonly Button _itemTypeAddButton;
    private readonly Button _itemTypeEditButton;
    private readonly Button _itemTypeRemoveButton;

    //private readonly ComboBox _itemBaseCombo;
    //private readonly Button _itemBaseAddButton;
    //private readonly Button _itemBaseEditButton;
    //private readonly Button _itemBaseRemoveButton;

    //private readonly ComboBox _regexTitleCombo;
    //private readonly Button _regexTitleAddButton;
    //private readonly Button _regexTitleEditButton;
    //private readonly Button _regexTitleRemoveButton;

    private ContentDialog _dialog = new();

    public RegexLibraryDialog(IStateManager<RegexLibraryStore> libraryManager)
    {
        _libraryManager = libraryManager;

        // Initialize controls
        _root = new()
        {
            ColumnDefinitions = ColumnDefinitions.Parse("*, Auto, Auto, Auto"),
            RowDefinitions = RowDefinitions.Parse("Auto, Auto, Auto, Auto"),
        };

        _itemTypeCombo = ControlsLibrary.MakeComboBox();
        PopulateItemTypeCombo();

        _itemTypeAddButton = ControlsLibrary.MakeSquareButton(content: "+");
        _itemTypeAddButton.Click += ItemTypeAddClick();

        _itemTypeEditButton = ControlsLibrary.MakeSquareButton(content: "E");
        _itemTypeEditButton.Click += ItemTypeEditClick();

        _itemTypeRemoveButton = ControlsLibrary.MakeSquareButton(content: "-");
        _itemTypeRemoveButton.Click += ItemTypeRemoveClick();

        // Define layout
        _root.AddControl(_itemTypeCombo, row: 0, column: 0);
        _root.AddControl(_itemTypeAddButton, row: 0, column: 1);
        _root.AddControl(_itemTypeEditButton, row: 0, column: 2);
        _root.AddControl(_itemTypeRemoveButton, row: 0, column: 3);
    }

    public async Task<ContentDialogResult> OpenDialogAsync()
    {
        _dialog = new ContentDialog
        {
            Title = DialogTitle,
            Content = _root,
            IsPrimaryButtonEnabled = false,
            PrimaryButtonText = "Select",
            CloseButtonText = "Close",
        };

        return await _dialog.ShowAsync();
    }

    private void PopulateItemTypeCombo()
    {
        _itemTypeCombo.Items.Clear();
        _libraryManager.State.GetItemTypes().ForEach(t => _itemTypeCombo.Items.Add(t));
    }

    private void RestoreFocusWithStateUpdate(Action update)
    {
        update();
        RestoreFocus();
    }

    private void RestoreFocus() => _dialog.Content = _root;

    private EventHandler<RoutedEventArgs> ItemTypeAddClick() => (_, _) =>
    {
        var inputControl = new InputPromptControl(
            prompt: "Enter new item type:",
            defaultInput: string.Empty,
            okAction: s => RestoreFocusWithStateUpdate(() =>
            {
                _libraryManager.Update(lib =>
                {
                    lib.TypeToBaseToNameToValue.Add(s, []);
                    return lib;
                });

                PopulateItemTypeCombo();
                _itemTypeCombo.SelectedIndex = _itemTypeCombo.Items.IndexOf(s);
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ItemTypeEditClick() => (_, _) =>
    {
        var selectedItemType = (string?)_itemTypeCombo.SelectedValue ?? string.Empty;

        var inputControl = new InputPromptControl(
            prompt: "Update item type:",
            defaultInput: selectedItemType,
            okAction: s => RestoreFocusWithStateUpdate(() =>
            {
                _libraryManager.Update(lib =>
                {
                    var values = lib.TypeToBaseToNameToValue[selectedItemType];
                    lib.TypeToBaseToNameToValue.Remove(selectedItemType);
                    lib.TypeToBaseToNameToValue.Add(s, values);
                    return lib;
                });

                PopulateItemTypeCombo();
                _itemTypeCombo.SelectedIndex = _itemTypeCombo.Items.IndexOf(s);
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ItemTypeRemoveClick() => (_, _) =>
    {
        var selectedItemType = (string?)_itemTypeCombo.SelectedValue ?? string.Empty;

        _libraryManager.Update(lib =>
        {
            lib.TypeToBaseToNameToValue.Remove(selectedItemType);
            return lib;
        });

        PopulateItemTypeCombo();
    };
}
