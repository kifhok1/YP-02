using CommunityToolkit.Mvvm.Input;

namespace VKR.ViewModels;

public partial class DatabaseRecoveryViewModel : ViewModelBase
{
    private bool _isVisibleImport =  false;
    private bool _isVisibleStructure = true;

    public bool IsVisibleImport
    {
        get { return _isVisibleImport; }
        set { SetProperty(ref _isVisibleImport, value); }
    }

    public bool IsVisibleStructure
    {
        get { return _isVisibleStructure; }
        set { SetProperty(ref _isVisibleStructure, value); }
    }

    [RelayCommand]
    private void ShowImport()
    {
        IsVisibleImport = true;
        IsVisibleStructure = false;
    }

    [RelayCommand]
    private void ShowStructure()
    {
        IsVisibleImport = false;
        IsVisibleStructure = true;
    }
}