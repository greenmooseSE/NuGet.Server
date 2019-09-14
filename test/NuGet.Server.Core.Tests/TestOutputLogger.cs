// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using NLog;
using NLog.Targets;
using Xunit.Abstractions;

namespace NuGet.Server.Core.Tests
{
    public class TestOutputLogger : TargetWithLayout
    {
        private ConcurrentQueue<string> _messages;

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = Layout.Render(logEvent);

            var formattedMessage = $"[{logEvent.Level.ToString().Substring(0, 4).ToUpperInvariant()}] {logMessage}";
            _messages.Enqueue(formattedMessage);
        }

        public TestOutputLogger()
        {
            _messages = new ConcurrentQueue<string>();
        }

        public IEnumerable<string> Messages => _messages;

        public void Clear()
        {
            _messages = new ConcurrentQueue<string>();
        }

    }
}
