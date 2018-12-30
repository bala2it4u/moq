﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockExtensionsTests
    {
        [Fact]
        public void WhenAddingMockBehavior_ThenCanAddLambda()
        {
            var mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior((m, n) => null, m => true);

            Assert.Equal(count + 1, mock.Mock.Behaviors.Count);
        }

        [Fact]
        public void WhenAddingMockBehavior_ThenCanAddInterface()
        {
            var mock = new Mocked();
            var behavior = new TestMockBehavior();

            mock.AddBehavior(behavior);

            Assert.Contains(behavior, mock.Mock.Behaviors);
        }

        [Fact]
        public void WhenAddingMockBehaviorToObject_ThenCanAddLambda()
        {
            object mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior((m, n) => null, m => true);

            Assert.Equal(count + 1, ((IMocked)mock).Mock.Behaviors.Count);
        }

        [Fact]
        public void WhenAddingMockBehaviorToObject_ThenCanAddInterface()
        {
            object mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior(new TestMockBehavior());

            Assert.Equal(count + 1, ((IMocked)mock).Mock.Behaviors.Count);
        }

        [Fact]
        public void WhenInsertingMockBehavior_ThenCanAddLambda()
        {
            var mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior((m, n) => null, m => true);
            mock.InsertBehavior(0, (m, n) => throw new NotImplementedException(), m => true);

            Assert.Equal(count + 2, mock.Mock.Behaviors.Count);
            Assert.Throws<NotImplementedException>(() => mock.Mock.Behaviors[0].Execute(null, null));
        }

        [Fact]
        public void WhenInsertingMockBehavior_ThenCanAddInterface()
        {
            var mock = new Mocked();
            var behavior = new TestMockBehavior();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior((m, n) => null, m => true);
            mock.InsertBehavior(0, behavior);

            Assert.Equal(count + 2, mock.Mock.Behaviors.Count);
            Assert.Same(behavior, mock.Mock.Behaviors[0]);
        }

        [Fact]
        public void WhenInsertingMockBehaviorToObject_ThenCanAddLambda()
        {
            object mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;

            mock.AddBehavior((m, n) => null, m => true);
            mock.InsertBehavior(0, (m, n) => throw new NotImplementedException(), m => true);

            Assert.Equal(count + 2, ((IMocked)mock).Mock.Behaviors.Count);
            Assert.Throws<NotImplementedException>(() => ((IMocked)mock).Mock.Behaviors[0].Execute(null, null));
        }

        [Fact]
        public void WhenInsertingMockBehaviorToObject_ThenCanAddInterface()
        {
            object mock = new Mocked();
            var count = mock.AsMock().Behaviors.Count;
            var behavior = new TestMockBehavior();

            mock.AddBehavior((m, n) => null, m => true);
            mock.InsertBehavior(0, behavior);

            Assert.Equal(count + 2, ((IMocked)mock).Mock.Behaviors.Count);
            Assert.Same(behavior, ((IMocked)mock).Mock.Behaviors[0]);
        }

        [Fact]
        public void WhenAddingMockBehaviorToObjectWithLambda_ThenThrowsIfNotMock() =>
            Assert.Throws<ArgumentException>(() => new object().AddBehavior((m, n) => null, m => true));

        [Fact]
        public void WhenAddingMockBehaviorToObjectWithInterface_ThenThrowsIfNotMock() =>
            Assert.Throws<ArgumentException>(() => new object().AddBehavior(new TestMockBehavior()));

        [Fact]
        public void WhenInsertingMockBehaviorToObjectWithLambda_ThenThrowsIfNotMock() =>
            Assert.Throws<ArgumentException>(() => new object().InsertBehavior(0, (m, n) => null, m => true));

        [Fact]
        public void WhenInsertingMockBehaviorToObjectWithInterface_ThenThrowsIfNotMock() =>
            Assert.Throws<ArgumentException>(() => new object().InsertBehavior(0, new TestMockBehavior()));

        class TestMockBehavior : IMockBehaviorPipeline
        {
            public IMockSetup Setup { get; set; } = new MockSetup(new FakeInvocation(), new IArgumentMatcher[0]);

            public ObservableCollection<IMockBehavior> Behaviors => null;

            public bool AppliesTo(IMethodInvocation invocation) => false;

            public IMethodReturn Execute(IMethodInvocation invocation, GetNextBehavior next) => null;
        }

        class Mock : IMock
        {
            public ObservableCollection<IStuntBehavior> Behaviors { get; } = new ObservableCollection<IStuntBehavior>();

            public ICollection<IMethodInvocation> Invocations { get; } = new HashSet<IMethodInvocation>();

            public object Object { get; set; }

            public MockState State => new MockState();

            public IMockSetup LastSetup { get; set; }

            public IEnumerable<IMockBehaviorPipeline> Setups => Behaviors.OfType<IMockBehaviorPipeline>();

            public IMockBehaviorPipeline GetPipeline(IMockSetup setup) => throw new NotImplementedException();
        }
    }
}
