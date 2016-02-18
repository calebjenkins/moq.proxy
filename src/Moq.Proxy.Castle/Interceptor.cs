﻿using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace Moq.Proxy.Castle
{
	internal class Interceptor : IInterceptor,
		/* Interface implemented so that we can detect breaking changes 
		*  and update our Intercept implementation accordingly */
		IProxy
	{
		BehaviorPipeline pipeline;

		internal Interceptor ()
		{
			pipeline = new BehaviorPipeline ();
		}

		public IList<IProxyBehavior> Behaviors => pipeline.Behaviors;

		public void Intercept (IInvocation invocation)
		{
			if (invocation.Method.DeclaringType == typeof (IProxy)) {
				invocation.ReturnValue = Behaviors;
				return;
			}

			var input = new MethodInvocation(invocation.InvocationTarget, invocation.Method, invocation.Arguments);
			var returns = pipeline.Invoke(input, (i, next) => {
				try  {
					invocation.Proceed();
					var returnValue = invocation.ReturnValue;
					return input.CreateValueReturn(returnValue, invocation.Arguments);
				} catch (Exception ex) {
					return input.CreateExceptionReturn(ex);
				}
			});

			var exception = returns.Exception;
			if (exception != null)
				throw exception;

			invocation.ReturnValue = returns.ReturnValue;
			for (int i = 0; i < returns.Outputs.Count; i++) {
				var name = returns.Outputs.GetName(i);
				var index = input.Arguments.IndexOf (name);
				invocation.SetArgumentValue (index, returns.Outputs[index]);
			}
		}
	}
}
