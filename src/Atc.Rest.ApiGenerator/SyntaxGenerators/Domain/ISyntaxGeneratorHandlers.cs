using System.Collections.Generic;
using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Domain
{
    public interface ISyntaxGeneratorHandlers
    {
        DomainProjectOptions DomainProjectOptions { get; }

        string FocusOnSegmentName { get; }

        List<SyntaxGeneratorHandler> GenerateSyntaxTrees();
    }
}