using AvariMaui.View;
using AvariMaui.ViewModel;
using AvariModel.Model;
using AvariModel.Persistence;

namespace AvariMaui;

public partial class AppShell : Shell
{

    private readonly AwariGameModel awariGameModel;
    private readonly AwariDataAcces awariDataAccess;
    private readonly AwariViewModel awariViewModel;

    private readonly IStore _store;
    private readonly StoredGameBrowserModel _storedGameBrowserModel;
    private readonly StoredGameBrowserViewModel _storedGameBrowserViewModel;

    public AppShell(IStore awariStore, AwariDataAcces awariDataAccess, AwariGameModel awariGameModel, AwariViewModel awariViewModel)
	{
        InitializeComponent();

        _store = awariStore;
        this.awariDataAccess = awariDataAccess;
        this.awariViewModel = awariViewModel;
        this.awariGameModel = awariGameModel;

        awariGameModel.GameOver += AwariGameModel_GameOver;

        awariViewModel.NewMap += AwariViewModel_NewMap;
        awariViewModel.SaveGame += AwariViewModel_SaveGame;
        awariViewModel.LoadGame += AwariViewModel_LoadGame;
        awariViewModel.ExitGame += AwariViewModel_ExitGame;

        // a játékmentések kezelésének összeállítása
        _storedGameBrowserModel = new StoredGameBrowserModel(_store);
        _storedGameBrowserViewModel = new StoredGameBrowserViewModel(_storedGameBrowserModel);
        _storedGameBrowserViewModel.GameLoading += StoredGameBrowserViewModel_GameLoading;
        _storedGameBrowserViewModel.GameSaving += StoredGameBrowserViewModel_GameSaving;

    }

    private async void AwariGameModel_GameOver(object sender, AwariEventArgs e)
    {
        if (e.WhoWon == "red") // győzelemtől függő üzenet megjelenítése
        {
            await DisplayAlert("Awari játék",
                "Gratulálok kettes játékos, győztél!" + Environment.NewLine,
                "OK");
        }
        else if (e.WhoWon == "blue")
        {
            await DisplayAlert("Awari játék",
                "Gratulálok egges játékos, győztél!" + Environment.NewLine,
                "OK");
        }
        else
        {
            await DisplayAlert("Awari játék",
                "Ez döntetlen lett!" + Environment.NewLine,
                "OK");
        }
    }

    private void AwariViewModel_NewMap(object sender, EventArgs e)
    {
        awariGameModel.NewGame();
    }

    private async void AwariViewModel_SaveGame(object sender, EventArgs e)
    {
        await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
        await Navigation.PushAsync(new LoadGamePage
        {
            BindingContext = _storedGameBrowserViewModel
        }); // átnavigálunk a lapra
    }

    /// <summary>
    ///     Játék mentésének eseménykezelője.
    /// </summary>
    private async void AwariViewModel_LoadGame(object sender, EventArgs e)
    {
        await _storedGameBrowserModel.UpdateAsync(); // frissítjük a tárolt játékok listáját
        await Navigation.PushAsync(new SaveGamePage
        {
            BindingContext = _storedGameBrowserViewModel
        }); // átnavigálunk a lapra
    }

    private async void AwariViewModel_ExitGame(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage
        {
            BindingContext = awariViewModel
        }); // átnavigálunk a beállítások lapra
    }

    /// <summary>
    ///     Betöltés végrehajtásának eseménykezelője.
    /// </summary>
    private async void StoredGameBrowserViewModel_GameLoading(object sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync(); // visszanavigálunk

        // betöltjük az elmentett játékot, amennyiben van
        try
        {
            await awariGameModel.LoadGameAsync(e.Name);
            awariViewModel.RefreshTable();

            // sikeres betöltés
            await Navigation.PopAsync(); // visszanavigálunk a játék táblára
            await DisplayAlert("Awari játék", "Sikeres betöltés.", "OK");

        }
        catch
        {
            await DisplayAlert("Awari játék", "Sikertelen betöltés.", "OK");
        }
    }

    /// <summary>
    ///     Mentés végrehajtásának eseménykezelője.
    /// </summary>
    private async void StoredGameBrowserViewModel_GameSaving(object sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync(); // visszanavigálunk

        try
        {
            // elmentjük a játékot
            await awariGameModel.SaveGameAsync(e.Name);
            await DisplayAlert("Awari játék", "Sikeres mentés.", "OK");
        }
        catch
        {
            await DisplayAlert("Awari játék", "Sikertelen mentés.", "OK");
        }
    }
}
