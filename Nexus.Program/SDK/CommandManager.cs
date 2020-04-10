﻿﻿namespace Nexus.SDK
{
    using System;

    public static class CommandManager
    {
        public static void WritePrompt()
        {
            Console.CursorLeft = 0;
            const string msg = "Nexus #";
            Console.Write(msg);
            Console.CursorLeft = checked(msg.Length + 1);
        }
    }
}