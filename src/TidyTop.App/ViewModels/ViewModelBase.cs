using System;
using System.Reactive;
using ReactiveUI;

namespace TidyTop.App.ViewModels;

public class ViewModelBase : ReactiveObject, IDisposable
{
    private bool _isBusy;
    private string _statusMessage = string.Empty;
    private bool _hasErrors;
    private string _errorMessage = string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        protected set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        protected set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public bool HasErrors
    {
        get => _hasErrors;
        protected set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        protected set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    protected void SetBusy(string statusMessage = null)
    {
        IsBusy = true;
        StatusMessage = statusMessage;
        HasErrors = false;
        ErrorMessage = null;
    }

    protected void SetIdle(string statusMessage = null)
    {
        IsBusy = false;
        StatusMessage = statusMessage;
    }

    protected void SetError(string errorMessage)
    {
        HasErrors = true;
        ErrorMessage = errorMessage;
        IsBusy = false;
    }

    protected void ClearError()
    {
        HasErrors = false;
        ErrorMessage = null;
    }

    public virtual void Dispose()
    {
        // Clean up resources if needed
    }
}