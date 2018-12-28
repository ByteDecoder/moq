﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class DefaultValueBehaviorTests
    {
        [Fact]
        public void DoesNotSetRefValue()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.VoidWithRef));
            IStuntBehavior behavior = new DefaultValueBehavior();
            var value = new object();

            var result = behavior.Execute(new MethodInvocation(new object(), method, value), () => null);

            Assert.Equal(1, result.Outputs.Count);
            Assert.NotNull(result.Outputs[0]);
            Assert.Same(result.Outputs[0], value);
        }

        [Fact]
        public void DoesNotSetsRefEnumValue()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.VoidWithRefEnum));
            IStuntBehavior behavior = new DefaultValueBehavior();
            var platform = PlatformID.Xbox;

            var result = behavior.Execute(new MethodInvocation(new object(), method, platform), () => null);

            Assert.Equal(1, result.Outputs.Count);
            Assert.NotNull(result.Outputs[0]);
            Assert.Equal(platform, result.Outputs[0]);
        }

        [Fact]
        public void SetsOutValue()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.VoidWithOut));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[1]), () => null);

            Assert.Equal(1, result.Outputs.Count);
            Assert.NotNull(result.Outputs[0]);
            Assert.True(result.Outputs[0] is object[]);
        }

        [Fact]
        public void SetsReturnEnum()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnEnum));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.Equal(default(PlatformID), result.ReturnValue);
        }

        [Fact]
        public void SetsReturnNullableEnum()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnNullableEnum));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.Null(result.ReturnValue);
        }


        [Fact]
        public void SetsReturnArray()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnArray));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is object[]);
            Assert.Empty(((Array)result.ReturnValue));
        }

        [Fact]
        public void SetsReturnTask()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnTask));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is Task);
            Assert.True(((Task)result.ReturnValue).IsCompleted);
        }

        [Fact]
        public void SetsReturnGenericTask()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnGenericTask));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is Task<object>);
            Assert.True(((Task)result.ReturnValue).IsCompleted);
        }

        [Fact]
        public void SetsReturnGenericTaskEnum()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnGenericTaskEnum));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is Task<PlatformID>);
            Assert.True(((Task)result.ReturnValue).IsCompleted);
            Assert.Equal(default, ((Task<PlatformID>)result.ReturnValue).Result);
        }

        [Fact]
        public void SetsCustomGenericEnumerable()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnGenericEnumerable));
            var behavior = new DefaultValueBehavior();

            behavior.Provider.Register(typeof(IEnumerable<object>), _ => new object[] { 5, 10 });

            var result = ((IStuntBehavior)behavior).Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is object[]);
            Assert.Equal(2, ((object[])result.ReturnValue).Length);
        }

        [Fact]
        public void SetsReturnEnumerable()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnEnumerable));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is IEnumerable);
        }

        [Fact]
        public void SetsReturnGenericEnumerable()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnGenericEnumerable));
            IStuntBehavior behavior = new DefaultValueBehavior();

            var result = behavior.Execute(new MethodInvocation(new object(), method, new object[0]), () => null);

            Assert.NotNull(result.ReturnValue);
            Assert.True(result.ReturnValue is IEnumerable<object>);
        }

        [Fact]
        public void DefaultValueForRef()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.VoidWithRef));
            var parameter = method.GetParameters()[0];

            Assert.True(parameter.ParameterType.IsByRef);

            var value = new DefaultValueProvider().For(parameter.ParameterType);

            Assert.NotNull(value);
            Assert.True(value is object[]);
            Assert.Empty((object[])value);
        }

        [Fact]
        public void DefaultValueForArrayWithRank()
        {
            var array = new int[0][][];
            var value = new DefaultValueProvider().For(array.GetType());

            Assert.Equal(array.GetType().GetArrayRank(), value.GetType().GetArrayRank());
        }

        [Fact]
        public void DefaultValueForValueTuple()
        {
            var method = typeof(IDefaultValues).GetMethod(nameof(IDefaultValues.ReturnsValueTuple));
            var value = new DefaultValueProvider().For(method.ReturnType);

            var (providers, formatter) = ((IServiceProvider[], Task<IFormatProvider>))value;

            Assert.NotNull(providers);
            Assert.Empty(providers);
            Assert.NotNull(formatter);
            Assert.True(formatter.IsCompleted);
            Assert.Equal(default(IFormatProvider), formatter.Result);
        }

        public interface IDefaultValues
        {
            void VoidWithRef(ref object[] refValue);

            void VoidWithRefEnum(ref PlatformID refEnum);

            void VoidWithOut(out object[] refValue);

            PlatformID ReturnEnum();

            PlatformID? ReturnNullableEnum();

            object[] ReturnArray();

            Task ReturnTask();

            Task<object> ReturnGenericTask();

            Task<PlatformID> ReturnGenericTaskEnum();

            IEnumerable ReturnEnumerable();

            IEnumerable<object> ReturnGenericEnumerable();

            (IServiceProvider[], Task<IFormatProvider>) ReturnsValueTuple();
        }
    }
}
