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
    /// <param name="title">The title to display.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="notificationType">The type of message is being displayed.</param>
    /// <param name="duration">
    ///     How long the message should be displayed, defaulting to
    ///     <see cref="defaultNotificationDuration" />.
    /// </param>
    public void ShowToastNotification(string title, string message, NotificationType notificationType,
        TimeSpan? duration = null)
    {
        ShowToastNotification(windowNotificationManager, title, message, notificationType, duration);
    }

    /// <summary>
    ///     Display a toast notification in the target <see cref="WindowNotificationManager" />.
    /// </summary>
    /// <param name="windowNotificationManager">The <see cref="WindowNotificationManager" /> to use for the notification.</param>
    /// <param name="title">The title to display.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="notificationType">The type of message is being displayed.</param>
    /// <param name="duration">
    ///     How long the message should be displayed, defaulting to
    ///     <see cref="defaultNotificationDuration" />.
    /// </param>
    public static void ShowToastNotification(WindowNotificationManager? windowNotificationManager, string title,
        string message, NotificationType notificationType, TimeSpan? duration = null)
    {
        windowNotificationManager?.Show(new Notification(title, message, notificationType,
            duration ?? defaultNotificationDuration));
    }
    #endregion
}
