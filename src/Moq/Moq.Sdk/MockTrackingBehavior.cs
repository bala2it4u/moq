﻿using System;
using Stunts;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using System.Text;

namespace Moq.Sdk
{
    /// <summary>
    /// Core behavior that allows tracking invocations and 
    /// building setups from them. 
    /// <para>
    /// Sets the <see cref="CallContext{IMethodInvocation}"/> 
    /// as well as the <see cref="CallContext{IMockSetup}"/>, used in 
    /// <see cref="MockContext.CurrentInvocation"/> and <see cref="MockContext.CurrentSetup"/> 
    /// respectively.
    /// </para>
    /// </summary>
    public class MockTrackingBehavior : IStuntBehavior
    {
        /// <summary>
        /// Returns <see langword="true"/> since it tracks all invocations.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation) => true;

        /// <summary>
        /// Implements the tracking of invocations for the excuted invocations.
        /// </summary>
        public IMethodReturn Execute(IMethodInvocation invocation, GetNextBehavior next)
        {
            // Allows subsequent extension methods on the fluent API to retrieve the 
            // current invocation being performed via the MockContext.
            MockContext.CurrentInvocation = invocation;

            // Determines the current setup according to contextual 
            // matchers that may have been pushed to the MockSetup context. 
            // Allows subsequent extension methods on the fluent API to retrieve the 
            // current setup being performed via the MockContext.
            MockContext.CurrentSetup = MockSetup.Freeze(invocation);

            // Only record the invocation if it's *not* performed within a setup scope.
            if (!SetupScope.IsActive)
                invocation.Target.AsMock().Invocations.Add(invocation);

            // While debugging, capture invocation stack traces for easier 
            // troubleshooting
            if (Debugger.IsAttached)
                invocation.Context[nameof(Environment.StackTrace)] = invocation.GetStackTrace();

            return next().Invoke(invocation, next);
        }
    }
}