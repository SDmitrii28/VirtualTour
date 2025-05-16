public interface IState
{
    void Enter();    // Вход в состояние
    void Execute();   // Логика обновления состояния
    void Exit();     // Выход из состояния
}