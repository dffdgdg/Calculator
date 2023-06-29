using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Appearance;
namespace Calculator.ViewModel;
internal partial class CalculatorVM : ObservableObject
{
    private ThemeType _currentTheme;
    public ICommand ThemeSet { get; }
    private string _inputContent = "";
    private string _calculatedResult = "0";
    private bool _isSciOpWaiting = false;
    private Visibility _ucVisibility = Visibility.Collapsed;
    private List<string> _calculationHistory = new();
    private RelayCommand _viewHistoryCommand, _close, _calculateCommand, _backspaceCommand, _resetCommand;
    private RelayCommand<string> _numberInputCommand, _mathOperatorCommand, _regionOperatorCommand, _scientificOperatorCommand;
    public CalculatorVM()
    {
        ThemeSet = new RelayCommand(UpdateTheme);
        _resetCommand = new RelayCommand(Reset);
        _viewHistoryCommand = new RelayCommand(ViewHistory);
        _close = new RelayCommand(CloseUC);
        _calculateCommand = new RelayCommand(Calculate);
        _backspaceCommand = new RelayCommand(Backspace);
        _numberInputCommand = new RelayCommand<string>(NumberInput);
        _mathOperatorCommand = new RelayCommand<string>(MathOperator);
        _regionOperatorCommand = new RelayCommand<string>(RegionOperator);
        _scientificOperatorCommand = new RelayCommand<string>(ScientificOperator);
        GetCurentTheme();
    }
    public string InputContent { get => _inputContent; set => SetProperty(ref _inputContent, value); }
    public Visibility UCVisibility { get => _ucVisibility; set => SetProperty(ref _ucVisibility, value); }
    public string CalculatedResult { get => _calculatedResult; set => SetProperty(ref _calculatedResult, value); }
    public List<string> CalculationHistory { get => _calculationHistory; set => SetProperty(ref _calculationHistory, value); }
    public ICommand ResetCommand => _resetCommand;
    public ICommand ViewHistoryCommand => _viewHistoryCommand;
    public ICommand Close => _close;
    public ICommand CalculateCommand => _calculateCommand;
    public ICommand BackspaceCommand => _backspaceCommand;
    public ICommand NumberInputCommand => _numberInputCommand;
    public ICommand MathOperatorCommand => _mathOperatorCommand;
    public ICommand RegionOperatorCommand => _regionOperatorCommand;
    public ICommand ScientificOperatorCommand => _scientificOperatorCommand;
    private void Reset()
    {
        CalculatedResult = "0";
        InputContent = "";
        _isSciOpWaiting = false;
    }
    private void Calculate()
    {
        if (InputContent.Length == 0) return;
        if (_isSciOpWaiting)
        {
            InputContent += ")";
            _isSciOpWaiting = false;
        }
        try
        {
            var inputString = NormalizeInputString();
            string historyString = InputContent; // Сохраняем исходный ввод для истории
            // Если ввод начинается с оператора, добавляем к нему последний результат
            if (!char.IsDigit(inputString[0]) && CalculatedResult != null)
            {
                inputString = CalculatedResult.ToString() + inputString;
                historyString = CalculatedResult + historyString; // Обновляем историю
            }
            var expression = new NCalc.Expression(inputString);
            var result = expression.Evaluate();
            CalculatedResult = result.ToString();
            CalculationHistory.Add($"{historyString} = {CalculatedResult}"); // Используем обновленную историю
            InputContent = ""; // Очищаем ввод после вычисления
        }
        catch { CalculatedResult = "NaN"; }
    }
    private void CloseUC() => UCVisibility = Visibility.Collapsed;
    private string NormalizeInputString()
    {
        Dictionary<string, string> _opMapper = new() { { "×", "*" }, { "÷", "/" }, { "SIN", "Sin" }, { "COS", "Cos" }, { "TAN", "Tan" }, { "ASIN", "Asin" }, { "ACOS", "Acos" }, { "ATAN", "Atan" }, { "LOG", "Log" }, { "EXP", "Exp" }, { "LOG10", "Log10" }, { "POW", "Pow" }, { "SQRT", "Sqrt" }, { "ABS", "Abs" }, };
        var retString = InputContent;
        foreach (var key in _opMapper.Keys) { retString = retString.Replace(key, _opMapper[key]); }
        return retString;
    }
    private void Backspace()
    {
        if (InputContent.Length > 0) InputContent = InputContent.Substring(0, InputContent.Length - 1);
    }
    private void NumberInput(string key) => InputContent += key;
    private void MathOperator(string op)
    {
        if (_isSciOpWaiting)
        {
            InputContent += ")";
            _isSciOpWaiting = false;
        }
        InputContent += $" {op} ";
    }
    private void RegionOperator(string op)
    {
        if (op == ")") _isSciOpWaiting = false;
        InputContent += op;
    }
    private void ViewHistory() => UCVisibility = Visibility.Visible;
    private void ScientificOperator(string op)
    {
        InputContent += $"{op}(";
        _isSciOpWaiting = true;
    }
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
}