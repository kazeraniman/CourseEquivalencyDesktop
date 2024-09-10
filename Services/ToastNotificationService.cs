using System;
using Avalonia.Controls.Notifications;

namespace CourseEquivalencyDesktop.Services;

/// <summary>
///     Allows for the display of toast notifications.
/// </summary>
public class ToastNotificationService(WindowNotificationManager windowNotificationManager)
{
    #region Constants / Static Readonly
    private static readonly TimeSpan defaultNotificationDuration = TimeSpan.FromSeconds(5);
    #endregion

    #region Public Interface
    /// <summary>
    ///     Display a toast notification.
    /// </summary>
    /// <param name="message">The message to display.</param>
    /// <param name="notificationType">The type of message is being displayed.</param>
    /// <param name="duration">
    ///     How long the message should be displayed, defaulting to
    ///     <see cref="defaultNotificationDuration" />.
    /// </param>
    public void ShowToastNotification(string message, NotificationType notificationType, TimeSpan? duration = null)
    {
        windowNotificationManager.Show(message, notificationType, duration ?? defaultNotificationDuration);
    }
    #endregion
}
