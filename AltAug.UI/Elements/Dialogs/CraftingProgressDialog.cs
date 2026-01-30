using AltAug.Domain.Interfaces;
using AltAug.Domain.Models;
using AltAug.UI.Extensions;
using AltAug.UI.Logging;
using Avalonia.Controls;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.Logging;

namespace AltAug.UI.Elements.Dialogs;

internal sealed class CraftingProgressDialog
{
    private record CraftingParameters(
        ICraftingStrategy Strategy,
        IReadOnlyCollection<IFilter> Conditions,
        ItemLocationParams LocationParams,
        int ItemsCount,
        int MaxAttempts);

    private const string DialogTitle = "Crafting Progress";
    private const string InstructionsText =
@"The crafting will start in a few seconds, please change focus to PoE window.
To interrupt the crafting process you can move the cursor to the top-left corner of the screen.";

    private readonly IStateManager<AppConfig> _appManager;
    private readonly IAutomationService _automationService;
    private readonly ICraftingService _craftingService;
    private readonly ILogger<CraftingProgressDialog> _logger;

    private readonly Grid _root;
    private readonly TextBlock _instructionsTextBlock;
    private readonly TextBox _logTextBox;

    private CancellationTokenSource _cts = null!;
    private ContentDialog _dialog = new();
    private CraftingParameters _craftingParameters = null!;

    public CraftingProgressDialog(IStateManager<AppConfig> appManager, IAutomationService automationService, ICraftingService craftingService, ILoggerFactory loggerFactory)
    {
        _appManager = appManager;
        _automationService = automationService;
        _craftingService = craftingService;

        // Initialize controls
        _root = new()
        {
            RowDefinitions = RowDefinitions.Parse("Auto, Auto"),
        };

        _instructionsTextBlock = ControlsLibrary.MakeVariableHeightTextBlock(text: InstructionsText);

        _logTextBox = ControlsLibrary.MakeLogTextBox();
        loggerFactory.AddProvider(new TextBoxLoggerProvider(_logTextBox, useMinimalFormat: true));

        _logger = loggerFactory.CreateLogger<CraftingProgressDialog>();

        // Define layout
        _root.AddControl(_instructionsTextBlock, row: 0, column: 0)
            .AddControl(_logTextBox, row: 1, column: 0);
    }

    public async Task<ContentDialogResult> OpenDialogAsync(
        ICraftingStrategy strategy,
        IReadOnlyCollection<IFilter> conditions,
        ItemLocationParams locationParams,
        int itemsCount,
        int maxAttempts)
    {
        _logTextBox.Text = string.Empty;
        _dialog.Content = null;

        _dialog = new()
        {
            Title = DialogTitle,
            Content = _root,
            PrimaryButtonText = "Repeat",
            IsPrimaryButtonEnabled = false,
            CloseButtonText = "Close",
        };
        _craftingParameters = new(strategy, conditions, locationParams, itemsCount, maxAttempts);

        _dialog.Opened += async (_, _) => await StartCraftingWithKillSwitchAsync().ConfigureAwait(false);
        _dialog.PrimaryButtonClick += async (_, args) =>
        {
            args.Cancel = true;
            _logTextBox.Text = string.Empty;
            await StartCraftingWithKillSwitchAsync().ConfigureAwait(false);
        };

        return await _dialog.ShowAsync();
    }

    private async Task StartCraftingWithKillSwitchAsync()
    {
        Dispatcher.UIThread.Post(() => _dialog.IsPrimaryButtonEnabled = false);

        _logger.LogInformation("Waiting for the user to switch focus to the PoE window...");
        await Task.Delay(TimeSpan.FromSeconds(_appManager.State.AutomationConfig.CraftingStartDelay)).ConfigureAwait(false);

        _cts = new();

        var timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(_appManager.State.AutomationConfig.AutoGuiPause / 4.0),
        };
        timer.Tick += (_, _) =>
        {
            var cursorPos = _automationService.GetMousePosition();
            var isInKillState = cursorPos.Match(
                pos => pos.X <= 0 && pos.Y <= 0,
                () => false);

            if (isInKillState)
            {
                _cts.Cancel();
                timer.Stop();
            }
        };

        timer.Start();

        try
        {
            _logger.LogInformation("Starting the crafting process...");

            await _craftingService.CraftItemsAsync(
                _craftingParameters.Strategy,
                _craftingParameters.Conditions,
                _craftingParameters.LocationParams,
                _craftingParameters.ItemsCount,
                _craftingParameters.MaxAttempts,
                _cts.Token)
                .ConfigureAwait(false);

            _logger.LogInformation("The crafting is done.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("The crafting operation was cancelled.");
        }
        finally
        {
            _cts.Dispose();
            timer.Stop();

            Dispatcher.UIThread.Post(() => _dialog.IsPrimaryButtonEnabled = true);
        }
    }
}
