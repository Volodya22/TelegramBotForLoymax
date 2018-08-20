namespace TelegramBotForLoymax.Infrastructure
{
    /// <summary>
    /// Возможные состояния работы сервиса
    /// </summary>
    public enum State
    {
        // Ждет команды
        Waiting,
        // Регистрация - ждет ввода фамилии
        SignUpLastName,
        // Регистрация - ждет ввода имени
        SignUpFirstName,
        // Регистрация - ждет ввода отчества
        SignUpPatronymic,
        // Регистрация - ждет ввода даты рождения
        SignUpBirthDate,
        // Ждет ввода идентификатора для отображения информации о пользователе
        Show,
        // Ждет воода информации для удаления данных о пользователе
        Remove
    }
}
