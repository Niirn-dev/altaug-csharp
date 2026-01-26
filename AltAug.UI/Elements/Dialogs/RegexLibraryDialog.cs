using AltAug.Domain.Helpers;
using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Extensions;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private readonly ComboBox _itemBaseCombo;
    private readonly Button _itemBaseAddButton;
    private readonly Button _itemBaseEditButton;
    private readonly Button _itemBaseRemoveButton;

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
        _itemTypeCombo.SelectionChanged += (_, _) => PopulateItemsAndDependents_ItemBaseCombo();

        _itemTypeAddButton = ControlsLibrary.MakeSquareButton(content: "+");
        _itemTypeAddButton.Click += ClickHandler_ItemType_Add();

        _itemTypeEditButton = ControlsLibrary.MakeSquareButton(content: "E");
        _itemTypeEditButton.Click += ClickHandler_ItemType_Edit();

        _itemTypeRemoveButton = ControlsLibrary.MakeSquareButton(content: "-");
        _itemTypeRemoveButton.Click += ClickHandler_ItemType_Remove();

        _itemBaseCombo = ControlsLibrary.MakeComboBox();

        _itemBaseAddButton = ControlsLibrary.MakeSquareButton(content: "+");
        _itemBaseAddButton.Click += ClickHandler_ItemBase_Add();

        _itemBaseEditButton = ControlsLibrary.MakeSquareButton(content: "E");
        _itemBaseEditButton.Click += ClickHandler_ItemBase_Edit();

        _itemBaseRemoveButton = ControlsLibrary.MakeSquareButton(content: "-");
        _itemBaseRemoveButton.Click += ClickHandler_ItemBase_Remove();

        PopulateItemsAndDependents_ItemTypeCombo();

        // Define layout
        _root.AddControl(_itemTypeCombo, row: 0, column: 0);
        _root.AddControl(_itemTypeAddButton, row: 0, column: 1);
        _root.AddControl(_itemTypeEditButton, row: 0, column: 2);
        _root.AddControl(_itemTypeRemoveButton, row: 0, column: 3);
        _root.AddControl(_itemBaseCombo, row: 1, column: 0);
        _root.AddControl(_itemBaseAddButton, row: 1, column: 1);
        _root.AddControl(_itemBaseEditButton, row: 1, column: 2);
        _root.AddControl(_itemBaseRemoveButton, row: 1, column: 3);
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

    private void PopulateItemsAndDependents_ItemTypeCombo()
    {
        _itemTypeCombo.Items.Clear();
        _libraryManager.State.GetItemTypes().ForEach(t => _itemTypeCombo.Items.Add(t));

        PopulateItemsAndDependents_ItemBaseCombo();
    }

    private void PopulateItemsAndDependents_ItemBaseCombo()
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _itemBaseCombo.Items.Clear();
        _libraryManager.State.GetItemBases(selectedItemType).ForEach(b => _itemBaseCombo.Items.Add(b));
    }

    private void RestoreFocusWithStateUpdate(Action update)
    {
        update();
        RestoreFocus();
    }

    private void RestoreFocus() => _dialog.Content = _root;

    #region Button Click Event Handlers
    private EventHandler<RoutedEventArgs> ClickHandler_ItemType_Add() => (_, _) =>
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

                PopulateItemsAndDependents_ItemTypeCombo();
                _itemTypeCombo.SelectedIndex = _itemTypeCombo.Items.IndexOf(s);
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_ItemType_Edit() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);

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

                PopulateItemsAndDependents_ItemTypeCombo();
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_ItemType_Remove() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _libraryManager.Update(lib =>
        {
            lib.TypeToBaseToNameToValue.Remove(selectedItemType);
            return lib;
        });

        PopulateItemsAndDependents_ItemTypeCombo();
    };

    private EventHandler<RoutedEventArgs> ClickHandler_ItemBase_Add() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        var inputControl = new InputPromptControl(
        prompt: "Enter new item base:",
        defaultInput: string.Empty,
        okAction: s => RestoreFocusWithStateUpdate(() =>
        {
            _libraryManager.Update(lib =>
            {
                lib.TypeToBaseToNameToValue[selectedItemType].Add(s, []);
                return lib;
            });

            PopulateItemsAndDependents_ItemBaseCombo();
            _itemBaseCombo.SelectedIndex = _itemBaseCombo.Items.IndexOf(s);
        }),
        cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_ItemBase_Edit() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        var inputControl = new InputPromptControl(
        prompt: "Update item base:",
        defaultInput: selectedItemBase,
        okAction: s => RestoreFocusWithStateUpdate(() =>
        {
            _libraryManager.Update(lib =>
            {
                lib.TypeToBaseToNameToValue[selectedItemType].Remove(selectedItemBase);
                lib.TypeToBaseToNameToValue[selectedItemType].Add(s, []);
                return lib;
            });

            PopulateItemsAndDependents_ItemBaseCombo();
        }),
        cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_ItemBase_Remove() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _libraryManager.Update(lib =>
        {
            lib.TypeToBaseToNameToValue[selectedItemType].Remove(selectedItemBase);
            return lib;
        });

        PopulateItemsAndDependents_ItemBaseCombo();
    };
    #endregion
}
