using System;
using UnityEngine;

public static class MessagingHandler
{
    private static Message CurrentMessage = new (string.Empty);

    public static event Action OnMessageChanged;

    public static void DisplayMessage(Message message)
    {
        CurrentMessage.Active = false;
        CurrentMessage = message;
        CurrentMessage.Active = true;
        OnMessageChanged?.Invoke();
    }

    public static void ClearMessage()
    {
        if (!CurrentMessage.Active) return;
        CurrentMessage.Active = false;
        OnMessageChanged?.Invoke();
    }

    public static bool GetMessage(out string message)
    {
        message = CurrentMessage.Text;
        return CurrentMessage.Active;
    }
}

public class Message
{
    public bool Active;
    public string Text { get; private set; }
    public Message(string text)
    {
        Active = false;
        Text = text;
    }
}