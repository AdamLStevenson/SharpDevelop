// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// 

using System;
using System.ComponentModel;

namespace ICSharpCode.SharpDevelop.Gui
{
    /// <summary>
    /// GlobalizedObject implements ICustomTypeDescriptor to enable
    /// required functionality to describe a type (class).<br></br>
    /// The main task of this class is to instantiate our own property descriptor
    /// of type GlobalizedPropertyDescriptor.
    /// </summary>
    public interface ILocalizedObject : ICustomTypeDescriptor
    {
        void InformSetValue(PropertyDescriptor propertyDescriptor, object component, object value);
    }
}
