﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Moq.Proxy.Castle
{
	/// <summary>
	/// Hook used to tells Castle which methods to proxy in mocked classes.
	/// 
	/// Here we proxy the default methods Castle suggests (everything Object's methods)
	/// plus Object.ToString(), so we can give mocks useful default names.
	/// 
	/// This is required to allow Moq to mock ToString on proxy *class* implementations.
	/// </summary>
	internal class ProxyMethodHook : AllMethodsHook
	{
		/// <summary>
		/// Extends AllMethodsHook.ShouldInterceptMethod to also intercept Object.ToString().
		/// </summary>
		public override bool ShouldInterceptMethod (Type type, MethodInfo methodInfo)
		{
			var isObjectToString = methodInfo.DeclaringType == typeof(Object) && methodInfo.Name == "ToString";

			return base.ShouldInterceptMethod (type, methodInfo) || isObjectToString;
		}
	}
}
