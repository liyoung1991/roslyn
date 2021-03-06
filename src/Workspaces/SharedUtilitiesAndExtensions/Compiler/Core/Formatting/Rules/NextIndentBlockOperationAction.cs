﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Formatting.Rules
{
    [NonDefaultable]
    internal readonly struct NextIndentBlockOperationAction
    {
        private readonly ImmutableArray<AbstractFormattingRule> _formattingRules;
        private readonly int _index;
        private readonly SyntaxNode _node;
        private readonly AnalyzerConfigOptions _options;
        private readonly List<IndentBlockOperation> _list;

        public NextIndentBlockOperationAction(
            ImmutableArray<AbstractFormattingRule> formattingRules,
            int index,
            SyntaxNode node,
            AnalyzerConfigOptions options,
            List<IndentBlockOperation> list)
        {
            _formattingRules = formattingRules;
            _index = index;
            _node = node;
            _options = options;
            _list = list;
        }

        private NextIndentBlockOperationAction NextAction
            => new NextIndentBlockOperationAction(_formattingRules, _index + 1, _node, _options, _list);

        public void Invoke()
        {
            // If we have no remaining handlers to execute, then we'll execute our last handler
            if (_index >= _formattingRules.Length)
            {
                return;
            }
            else
            {
                // Call the handler at the index, passing a continuation that will come back to here with index + 1
                _formattingRules[_index].AddIndentBlockOperations(_list, _node, _options, NextAction);
                return;
            }
        }
    }
}
