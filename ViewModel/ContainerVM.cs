using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Wpf.Ui.Appearance;
namespace Calculator.ViewModel;
internal class ContainerVM
{
    #region переменные
    private ThemeType _currentTheme;
    public ICommand ThemeSet => new RelayCommand(UpdateTheme);
    #endregion
    #region методы
    private void UpdateTheme()
    {
        _currentTheme = _currentTheme == ThemeType.Light ? ThemeType.Dark : ThemeType.Light;
        Theme.Apply(_currentTheme);
        AppSettings.Default.Theme = _currentTheme.ToString();
        AppSettings.Default.Save();
    }

    private void GetCurentTheme()
    {
        _currentTheme = AppSettings.Default.Theme == "Dark" ? ThemeType.Dark : ThemeType.Light;
        Theme.Apply(_currentTheme);
    }

    public ContainerVM()
    {
        GetCurentTheme();
    }
    #endregion
}
