using System;
using System.Globalization;
using System.IO;

public class MessageLogger
{
    private static MessageLogger _mlObj = (MessageLogger)null;
    private bool _isEmail = false;
    private string logFilePath = string.Empty;
    private MessageLogger.Severity _level;

    protected static MessageLogger getMLInstance()
    {
        if (MessageLogger._mlObj == null)
            MessageLogger._mlObj = new MessageLogger(MessageLogger.Severity.Info);
        return MessageLogger._mlObj;
    }

    public MessageLogger(MessageLogger.Severity level)
    {
        this._level = level;
        this.logFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".txt";
    }

    public MessageLogger(MessageLogger.Severity level, bool email, string path)
    {
        this._level = level;
        this._isEmail = email;
        this.logFilePath = path;
    }

    public MessageLogger.Severity level
    {
        get
        {
            return this._level;
        }
        set
        {
            this._level = value;
        }
    }

    public void fatal(string message)
    {
        this.log(MessageLogger.Severity.Fatal, message);
    }

    public void error(string message)
    {
        this.log(MessageLogger.Severity.Error, message);
    }

    public void warning(string message)
    {
        this.log(MessageLogger.Severity.Warning, message);
    }

    public static void info(string message)
    {
        MessageLogger.getMLInstance().log(MessageLogger.Severity.Info, message);
    }

    public void debug(string message)
    {
        this.log(MessageLogger.Severity.Debug, message);
    }

    private void log(MessageLogger.Severity level, string message)
    {
        string str1 = "sa_ish";
        string str2 = "-";
        try
        {
            if (!File.Exists(this.logFilePath))
                File.Create(this.logFilePath).Close();
            using (StreamWriter streamWriter = File.AppendText(this.logFilePath))
            {
                streamWriter.WriteLine("{0};{1};{2};{3};{4}", (object)DateTime.Now.ToString((IFormatProvider)CultureInfo.InvariantCulture), (object)level, (object)message, (object)str2, (object)str1);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }
        catch (Exception ex)
        {
            string empty = string.Empty;
        }
    }

    public enum Severity
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal,
    }
}
