using System;
using System.Windows.Input;

/// <summary>
/// Implementación reutilizable de ICommand para enlazar acciones del ViewModel con la Vista.
/// - _execute: acción a ejecutar cuando el comando se invoca.
/// - _canExecute: función opcional que determina si el comando puede ejecutarse.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Devuelve si el comando puede ejecutarse. Si no hay función, devuelve true por defecto.
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

    // Ejecuta la acción asociada al comando.
    public void Execute(object parameter) => _execute();

    // Eventos que WPF escucha para reevaluar el estado del comando (habilitado/deshabilitado).
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}