﻿using System;
using System.CodeDom.Compiler;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.124.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Generated.Contracts.Tasks
{
    /// <summary>
    /// Describes a single task.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.124.0")]
    public class Task
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}