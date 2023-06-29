using AvariMaui.Persistence;
using AvariMaui.ViewModel;
using AvariModel.Model;
using AvariModel.Persistence;

namespace AvariMaui;

public partial class App : Application
{

    private readonly AppShell _appShell;
	private readonly IStore awariStore;
	private readonly AwariViewModel awariViewModel;
	private readonly AwariGameModel awariGameModel;
	private readonly AwariDataAcces awariDataAccess;

    public App()
	{
        InitializeComponent();

        awariStore = new AwariStore();
		awariDataAccess = new AwariDataAccesser();

		awariGameModel = new AwariGameModel(awariDataAccess);
		awariViewModel = new AwariViewModel(awariGameModel);

		_appShell = new AppShell(awariStore, awariDataAccess, awariGameModel, awariViewModel)
		{
            BindingContext = awariViewModel
        };
        MainPage = _appShell;
    }

    protected override void OnStart()
    {
        awariGameModel.NewGame();
        awariViewModel.RefreshTable();
        
    }

    protected override void OnSleep()
    {
        Task.Run(async () =>
        {
            try
            {
                // elmentjük a jelenleg folyó játékot
                
                await awariGameModel.SaveGameAsync(AwariDataAcces.SuspendedGameSavePath);
            }
            catch
            {
            }
        });
    }

    protected override void OnResume()
    {
        Task.Run(async () =>
        {
            // betöltjük a felfüggesztett játékot, amennyiben van
            try
            {
                await awariGameModel.LoadGameAsync(AwariDataAcces.SuspendedGameSavePath);
                awariViewModel.RefreshTable();
            }
            catch
            {
            }
        });
    }
}
