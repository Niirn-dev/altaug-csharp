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
    private readonly RegexLibraryStore _savedState;
    private readonly Grid _root;

    private readonly ComboBox _itemTypeCombo;
    private readonly Button _itemTypeAddButton;
    private readonly Button _itemTypeEditButton;
    private readonly Button _itemTypeRemoveButton;

    private readonly ComboBox _itemBaseCombo;
    private readonly Button _itemBaseAddButton;
    private readonly Button _itemBaseEditButton;
    private readonly Button _itemBaseRemoveButton;

    private readonly ComboBox _regexTitleCombo;
    private readonly Button _regexTitleAddButton;
    private readonly Button _regexTitleEditButton;
    private readonly Button _regexTitleRemoveButton;

    private readonly TextBox _regexStringTextBox;
    private readonly Button _regexStringUpdateButton;

    private ContentDialog _dialog = new();

    public string RegexString { get => _regexStringTextBox.Text ?? string.Empty; }

    public RegexLibraryDialog(IStateManager<RegexLibraryStore> libraryManager)
    {
        _libraryManager = libraryManager;
        _savedState = _libraryManager.State;

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
        _itemBaseCombo.SelectionChanged += (_, _) => PopulateItemsAndDependents_RegexTitleCombo();

        _itemBaseAddButton = ControlsLibrary.MakeSquareButton(content: "+");
        _itemBaseAddButton.Click += ClickHandler_ItemBase_Add();

        _itemBaseEditButton = ControlsLibrary.MakeSquareButton(content: "E");
        _itemBaseEditButton.Click += ClickHandler_ItemBase_Edit();

        _itemBaseRemoveButton = ControlsLibrary.MakeSquareButton(content: "-");
        _itemBaseRemoveButton.Click += ClickHandler_ItemBase_Remove();

        _regexTitleCombo = ControlsLibrary.MakeComboBox();
        _regexTitleCombo.SelectionChanged += (_, _) => UpdateText_RegexString();

        _regexTitleAddButton = ControlsLibrary.MakeSquareButton(content: "+");
        _regexTitleAddButton.Click += ClickHandler_RegexTitle_Add();

        _regexTitleEditButton = ControlsLibrary.MakeSquareButton(content: "E");
        _regexTitleEditButton.Click += ClickHandler_RegexTitle_Edit();

        _regexTitleRemoveButton = ControlsLibrary.MakeSquareButton(content: "-");
        _regexTitleRemoveButton.Click += ClickHandler_RegexTitle_Remove();

        _regexStringTextBox = ControlsLibrary.MakeTextBox(text: string.Empty);

        _regexStringUpdateButton = ControlsLibrary.MakeSquareButton(content: "U");
        _regexStringTextBox.TextChanged += TextChanged_RegexString();
        _regexStringUpdateButton.Click += ClickHandler_RegexString_Update();

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

        _root.AddControl(_regexTitleCombo, row: 2, column: 0);
        _root.AddControl(_regexTitleAddButton, row: 2, column: 1);
        _root.AddControl(_regexTitleEditButton, row: 2, column: 2);
        _root.AddControl(_regexTitleRemoveButton, row: 2, column: 3);

        _root.AddControl(_regexStringTextBox, row: 3, column: 0, rowSpan: 0, columnSpan: 3);
        _root.AddControl(_regexStringUpdateButton, row: 3, column: 3);
    }

    public async Task<ContentDialogResult> OpenDialogAsync()
    {
        _dialog.Content = null;

        _dialog = new ContentDialog
        {
            Title = DialogTitle,
            Content = _root,
            IsPrimaryButtonEnabled = false,
            PrimaryButtonText = "Select",
            SecondaryButtonText = "Save",
            CloseButtonText = "Cancel",
        };

        _dialog.PrimaryButtonClick += (_, _) => _libraryManager.Save();
        _dialog.SecondaryButtonClick += (_, args) =>
        {
            _libraryManager.Save();
            args.Cancel = true;
        };
        _dialog.CloseButtonClick += (_, _) => _libraryManager.Update(_ => _savedState);

        _regexStringTextBox.TextChanged += (_, _) => _dialog.IsPrimaryButtonEnabled = RegexString.Length != 0;

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

        PopulateItemsAndDependents_RegexTitleCombo();
    }

    private void PopulateItemsAndDependents_RegexTitleCombo()
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _regexTitleCombo.Items.Clear();
        _libraryManager.State.GetRegexTitles(selectedItemType, selectedItemBase).ForEach(b => _regexTitleCombo.Items.Add(b));

        UpdateText_RegexString();
    }

    private void UpdateText_RegexString()
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedRegexTitle = _regexTitleCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _regexStringTextBox.Text = _libraryManager.State.GetRegexString(selectedItemType, selectedItemBase, selectedRegexTitle);
    }

    private void RestoreFocusWithStateUpdate(Action update)
    {
        update();
        RestoreFocus();
    }

    private void RestoreFocus() => _dialog.Content = _root;

    #region Event Handlers
    private EventHandler<RoutedEventArgs> ClickHandler_ItemType_Add() => (_, _) =>
    {
        var inputControl = new InputPromptControl(
            prompt: "Enter a new item type:",
            defaultPrimary: string.Empty,
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
            prompt: "Update the item type:",
            defaultPrimary: selectedItemType,
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
            prompt: "Enter a new item base:",
            defaultPrimary: string.Empty,
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
            prompt: "Update the item base:",
            defaultPrimary: selectedItemBase,
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

    private EventHandler<RoutedEventArgs> ClickHandler_RegexTitle_Add() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        var inputControl = new InputPromptControl(
            prompt: "Enter a new RegEx title and RegEx string:",
            defaultPrimary: string.Empty,
            defaultSecondary: string.Empty,
            okAction: (title, regex) => RestoreFocusWithStateUpdate(() =>
            {
                _libraryManager.Update(lib =>
                {
                    lib.TypeToBaseToNameToValue[selectedItemType][selectedItemBase].Add(title, regex);
                    return lib;
                });

                PopulateItemsAndDependents_RegexTitleCombo();
                _regexTitleCombo.SelectedIndex = _regexTitleCombo.Items.IndexOf(title);
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_RegexTitle_Edit() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedRegexTitle = _regexTitleCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        var inputControl = new InputPromptControl(
            prompt: "Edit the RegEx title and RegEx string:",
            defaultPrimary: selectedRegexTitle,
            defaultSecondary: _libraryManager.State.GetRegexString(selectedItemType, selectedItemBase, selectedRegexTitle),
            okAction: (title, regex) => RestoreFocusWithStateUpdate(() =>
            {
                _libraryManager.Update(lib =>
                {
                    lib.TypeToBaseToNameToValue[selectedItemType][selectedItemBase][title] = regex;
                    return lib;
                });

                PopulateItemsAndDependents_RegexTitleCombo();
            }),
            cancelAction: RestoreFocus);

        _dialog.Content = inputControl.Control;
    };

    private EventHandler<RoutedEventArgs> ClickHandler_RegexTitle_Remove() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedRegexTitle = _regexTitleCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _libraryManager.Update(lib =>
        {
            lib.TypeToBaseToNameToValue[selectedItemType][selectedItemBase].Remove(selectedRegexTitle);
            return lib;
        });

        PopulateItemsAndDependents_RegexTitleCombo();
    };

    private EventHandler<RoutedEventArgs> ClickHandler_RegexString_Update() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedRegexTitle = _regexTitleCombo.SelectedValueOrDefault(defaultValue: string.Empty);

        _libraryManager.Update(lib =>
        {
            lib.TypeToBaseToNameToValue[selectedItemType][selectedItemBase][selectedRegexTitle] = RegexString;
            return lib;
        });
    };

    private EventHandler<TextChangedEventArgs> TextChanged_RegexString() => (_, _) =>
    {
        var selectedItemType = _itemTypeCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedItemBase = _itemBaseCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var selectedRegexTitle = _regexTitleCombo.SelectedValueOrDefault(defaultValue: string.Empty);
        var libraryRegexString = _libraryManager.State.GetRegexString(selectedItemType, selectedItemBase, selectedRegexTitle);

        _regexStringUpdateButton.IsEnabled = !RegexString.Equals(libraryRegexString, StringComparison.OrdinalIgnoreCase);
    };
    #endregion
}
